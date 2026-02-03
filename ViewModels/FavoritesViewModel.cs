using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace RadioPlayer.ViewModels {
    public partial class FavoritesViewModel : ViewModelBase {
        [Reactive]
        private int _favoritesCount = 0;

        public string Title => $"Mes favoris ({FavoritesCount})";
    }
}
