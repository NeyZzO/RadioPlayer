using ReactiveUI;
using RadioPlayer.Models;

namespace RadioPlayer.ViewModels;

public class RadioStationViewModel : ViewModelBase {
    private readonly RadioStation _station;

    public RadioStationViewModel(RadioStation station) {
        _station = station;
    }

    public string Name => _station.name;
    public string Genre => _station.Genre;
    public string ImageUrl => _station.favicon;
    public string[] Tags => _station.tags;
    public string StreamUrl => _station.url;
    public string CountryCode => _station.countrycode;
    public string Country => _station.country;

    private bool _isFavorite;
    public bool IsFavorite {
        get => _isFavorite;
        set => this.RaiseAndSetIfChanged(ref _isFavorite, value);
    }
}
