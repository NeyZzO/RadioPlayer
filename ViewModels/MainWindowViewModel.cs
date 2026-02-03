using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace RadioPlayer.ViewModels {
    public partial class MainWindowViewModel : ViewModelBase {

        // Variables bindings pour le player
        private string? _radioStationName = null;
        public string RadioStationName {
            get => _radioStationName ??= "Aucun média";
            set => this.RaiseAndSetIfChanged(ref _radioStationName, value);
        }

        private string? _radioStationurl = null;
        public string? RadioStationurl {
            get => this._radioStationurl;
            set => this.RaiseAndSetIfChanged(ref _radioStationurl, value);
        }

        private string? _radioStationGenre = null;
        public string RadioStationGenre {
            get => this._radioStationGenre ?? "Choisissez une station pour commencer";
            set => this.RaiseAndSetIfChanged(ref _radioStationGenre, value);
        }

        [Reactive]
        private string? _radioStationImageUrl;

        [Reactive]
        private int _volume = 50;

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

        public MainWindowViewModel() {
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
        }
    }
}
