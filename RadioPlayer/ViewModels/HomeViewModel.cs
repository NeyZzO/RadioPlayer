using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using RadioPlayer.Models;

namespace RadioPlayer.ViewModels {
    public class HomeViewModel : ViewModelBase {
        public RadioStationSliderViewModel FrenchRadiosSlider { get; }
        public RadioStationSliderViewModel WorldRadiosSlider { get; }

        public HomeViewModel() {
            FrenchRadiosSlider = new RadioStationSliderViewModel(
                "Radios populaires en France",
                "🇫🇷",
                new ObservableCollection<RadioStationViewModel>());

            WorldRadiosSlider = new RadioStationSliderViewModel(
                "Radios populaires dans le monde",
                "🌍",
                new ObservableCollection<RadioStationViewModel>());

            _ = LoadRadiosAsync();
        }

        private async Task LoadRadiosAsync() {
            await Task.WhenAll(SetFrenchRadios(), SetWorldRadios());
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
                        FrenchRadiosSlider.Stations.Add(new(station));
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
