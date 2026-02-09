using System;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using RadioPlayer.Models;
using RadioPlayer.Services;

namespace RadioPlayer.ViewModels;

public class RadioStationViewModel : ViewModelBase, IDisposable {
    private readonly RadioStation _station;
    private readonly IDisposable _favoriteSubscription;
    private readonly IDisposable _playerSubscription;

    public RadioStationViewModel(RadioStation station) {
        _station = station;

        // Vérifier si la station est en favori au chargement
        _isFavorite = FavoritesService.Instance.IsFavorite(station.stationuuid);

        // Vérifier si la station est en cours de lecture
        _isCurrentlyPlaying = PlayerService.Instance.CurrentStation?.stationuuid == station.stationuuid;

        // S'abonner aux changements de favoris
        _favoriteSubscription = FavoritesService.Instance.FavoriteChanged
            .Where(change => change.Uuid == station.stationuuid)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(change => IsFavorite = change.IsFavorite);

        // S'abonner aux changements de station en cours
        _playerSubscription = PlayerService.Instance.CurrentStationChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(currentStation => {
                IsCurrentlyPlaying = currentStation?.stationuuid == station.stationuuid;
            });

        // Commandes
        ToggleFavoriteCommand = ReactiveCommand.Create(ToggleFavorite);
        PlayCommand = ReactiveCommand.Create(Play);
    }

    public string Uuid => _station.stationuuid;
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

    private bool _isCurrentlyPlaying;
    public bool IsCurrentlyPlaying {
        get => _isCurrentlyPlaying;
        set => this.RaiseAndSetIfChanged(ref _isCurrentlyPlaying, value);
    }

    public ICommand ToggleFavoriteCommand { get; }
    public ICommand PlayCommand { get; }

    /// <summary>
    /// Retourne le modèle RadioStation sous-jacent
    /// </summary>
    public RadioStation GetStation() => _station;

    private void ToggleFavorite() {
        FavoritesService.Instance.Toggle(_station);
    }

    private void Play() {
        PlayerService.Instance.Play(_station);
    }

    public void Dispose() {
        _favoriteSubscription.Dispose();
        _playerSubscription.Dispose();
    }
}
