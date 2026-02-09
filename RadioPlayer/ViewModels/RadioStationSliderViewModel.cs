using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace RadioPlayer.ViewModels;

public class RadioStationSliderViewModel : ViewModelBase {
    public RadioStationSliderViewModel(string title, string icon, ObservableCollection<RadioStationViewModel> stations) {
        Title = title;
        Icon = icon;
        Stations = stations;

        ScrollLeftCommand = ReactiveCommand.Create(() => { /* Scroll logic handled in view */ });
        ScrollRightCommand = ReactiveCommand.Create(() => { /* Scroll logic handled in view */ });
    }

    public string Title { get; }
    public string Icon { get; }
    public ObservableCollection<RadioStationViewModel> Stations { get; set; }

    public ReactiveCommand<Unit, Unit> ScrollLeftCommand { get; }
    public ReactiveCommand<Unit, Unit> ScrollRightCommand { get; }
}
