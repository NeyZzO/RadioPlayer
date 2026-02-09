using Avalonia.Controls;
using Avalonia.Input;
using RadioPlayer.ViewModels;

namespace RadioPlayer.Views {
    public partial class FavoritesView : UserControl {
        public FavoritesView() {
            InitializeComponent();
        }

        private void OnStationCardPressed(object? sender, PointerPressedEventArgs e) {
            if (e.Source is Button) return;

            if (sender is Border border && border.DataContext is RadioStationViewModel vm) {
                vm.PlayCommand.Execute(null);
            }
        }
    }
}
