using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RadioPlayer.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace RadioPlayer.ViewModels {
    public partial class MainWindowViewModel : ViewModelBase {

        // Variables bindings   
        private string? _radioStationName = null;
        public string RadioStationName {
            get => _radioStationName ??= "Chargement...";
            set => this.RaiseAndSetIfChanged(ref _radioStationName, value);
        }

        private string? _radioStationurl = null;
        public string? RadioStationurl {
            get => this._radioStationurl;
            set => this.RaiseAndSetIfChanged(ref _radioStationurl, value);
        }

        private string? _radioStationGenre = null;
        public string RadioStationGenre {
            get => this._radioStationGenre ?? "En attente...";
            set => this.RaiseAndSetIfChanged(ref _radioStationGenre, value);
        }

        [Reactive]
        private string? _radioStationImageUrl;

        [Reactive]
        private int _volume = 50;

        // Sliders de radios
        public RadioStationSliderViewModel FrenchRadiosSlider { get; }
        public RadioStationSliderViewModel WorldRadiosSlider { get; }

        public MainWindowViewModel() {
            FrenchRadiosSlider = new RadioStationSliderViewModel(
                "Radios populaires en France",
                "🇫🇷",
                new ObservableCollection<RadioStationViewModel>());

            SetFrenchRadios();

            WorldRadiosSlider = new RadioStationSliderViewModel(
                "Radios populaires dans le monde",
                "🌍",
                new ObservableCollection<RadioStationViewModel>());
            SetWorldRadios();
        }

        private async Task SetFrenchRadios() {
            const string apiUrl = "http://de1.api.radio-browser.info/json/stations/search?order=clickcount&countrycode=FR&limit=10&reverse=true";
            try {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var stations = System.Text.Json.JsonSerializer.Deserialize<List<RadioStation>>(jsonResponse);
                if (stations != null) {
                    foreach (var station in stations) {
                        FrenchRadiosSlider.Stations.Add(new (station));
                    }
                }

            } catch (Exception ex) {
                Debug.WriteLine($"Erreur lors de la récupération des radios françaises : {ex.Message}");
            }

        }

        private async Task SetWorldRadios() {
            const string apiUrl = "http://de1.api.radio-browser.info/json/stations/search?order=clickcount&limit=10&reverse=true";
            try {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var stations = System.Text.Json.JsonSerializer.Deserialize<List<RadioStation>>(jsonResponse);
                if (stations != null) {
                    foreach (var station in stations) {
                        WorldRadiosSlider.Stations.Add(new(station));
                    }
                }

            } catch (Exception ex) {
                Debug.WriteLine($"Erreur lors de la récupération des radios mondiales : {ex.Message}");
            }
        }
    }
}
