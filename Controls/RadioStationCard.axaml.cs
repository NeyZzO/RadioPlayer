using Avalonia.Controls;
using Avalonia.Input;
using RadioPlayer.ViewModels;

namespace RadioPlayer.Controls;

public partial class RadioStationCard : UserControl {
    public RadioStationCard() {
        InitializeComponent();
    }

    private void OnCardPressed(object? sender, PointerPressedEventArgs e) {
        // Ignorer si c'est le bouton favori qui a été cliqué
        if (e.Source is Button) return;

        if (DataContext is RadioStationViewModel vm) {
            vm.PlayCommand.Execute(null);
        }
    }
}
