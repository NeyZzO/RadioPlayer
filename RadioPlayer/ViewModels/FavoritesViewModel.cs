using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;
using RadioPlayer.Models;
using RadioPlayer.Services;

namespace RadioPlayer.ViewModels {
    public class FavoritesViewModel : ViewModelBase {
        private readonly FavoritesService _favoritesService;

        public ObservableCollection<RadioStationViewModel> Favorites { get; } = new();

        public FavoritesViewModel() {
            _favoritesService = FavoritesService.Instance;

            // Charger les favoris initiaux
            LoadFavorites();

            // Observer les changements de la collection de favoris
            _favoritesService.Favorites.CollectionChanged += (_, _) => {
                LoadFavorites();
                this.RaisePropertyChanged(nameof(FavoritesCount));
                this.RaisePropertyChanged(nameof(HasFavorites));
            };
        }

        public int FavoritesCount => _favoritesService.Count;

        public bool HasFavorites => FavoritesCount > 0;

        public string Title => $"Mes favoris ({FavoritesCount})";

        private void LoadFavorites() {
            Favorites.Clear();
            foreach (var fav in _favoritesService.Favorites) {
                var station = fav.ToRadioStation();
                Favorites.Add(new RadioStationViewModel(station));
            }
        }
    }
}
