using System;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RadioPlayer.Services;

namespace RadioPlayer.ViewModels {
    public partial class MainWindowViewModel : ViewModelBase {
        private readonly PlayerService _playerService;

        // Variables bindings pour le player
        private string _radioStationName = "Aucun média";
        public string RadioStationName {
            get => _radioStationName;
            set => this.RaiseAndSetIfChanged(ref _radioStationName, value);
        }

        private string _radioStationGenre = "Choisissez une station pour commencer";
        public string RadioStationGenre {
            get => _radioStationGenre;
            set => this.RaiseAndSetIfChanged(ref _radioStationGenre, value);
        }

        [Reactive]
        private string? _radioStationImageUrl;

        [Reactive]
        private int _volume = 50;

        [Reactive]
        private bool _isPlaying = false;

        // Navigation
        [Reactive]
        private ViewModelBase _currentPage;

        public HomeViewModel HomeViewModel { get; }
        public FavoritesViewModel FavoritesViewModel { get; }
        public SearchViewModel SearchViewModel { get; }
        public AllStationsViewModel AllStationsViewModel { get; }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateFavoritesCommand { get; }
        public ICommand NavigateSearchCommand { get; }
        public ICommand NavigateAllStationsCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand StopCommand { get; }

        public MainWindowViewModel() {
            _playerService = PlayerService.Instance;

            // Initialiser les ViewModels des pages
            HomeViewModel = new HomeViewModel();
            FavoritesViewModel = new FavoritesViewModel();
            SearchViewModel = new SearchViewModel();
            AllStationsViewModel = new AllStationsViewModel();

            // Page par défaut
            _currentPage = HomeViewModel;

            // Commandes de navigation
            NavigateHomeCommand = ReactiveCommand.Create(() => CurrentPage = HomeViewModel);
            NavigateFavoritesCommand = ReactiveCommand.Create(() => CurrentPage = FavoritesViewModel);
            NavigateSearchCommand = ReactiveCommand.Create(() => CurrentPage = SearchViewModel);
            NavigateAllStationsCommand = ReactiveCommand.Create(() => CurrentPage = AllStationsViewModel);

            // Commandes du player
            StopCommand = ReactiveCommand.Create(() => _playerService.Stop());

            // S'abonner aux changements du PlayerService
            _playerService.CurrentStationChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(station => {
                    if (station != null) {
                        RadioStationName = station.name;
                        RadioStationGenre = string.IsNullOrEmpty(station.TagsString) 
                            ? station.country 
                            : station.TagsString.Split(',')[0];
                        RadioStationImageUrl = station.favicon;
                    } else {
                        RadioStationName = "Aucun média";
                        RadioStationGenre = "Choisissez une station pour commencer";
                        RadioStationImageUrl = null;
                    }
                });

            _playerService.IsPlayingChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(isPlaying => IsPlaying = isPlaying);

            _playerService.VolumeChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(volume => Volume = volume);

            // Synchroniser le volume bidirectionnellement
            this.WhenAnyValue(x => x.Volume)
                .Skip(1) // Ignorer la valeur initiale
                .Subscribe(volume => _playerService.SetVolume(volume));
        }
    }
}
