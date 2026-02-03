using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace RadioPlayer.ViewModels {
    public partial class SearchViewModel : ViewModelBase {
        [Reactive]
        private string _searchQuery = string.Empty;

        public ICommand SearchCommand { get; }

        public SearchViewModel() {
            SearchCommand = ReactiveCommand.Create(ExecuteSearch);
        }

        private void ExecuteSearch() {
            // TODO: Implémenter la recherche
        }
    }
}
