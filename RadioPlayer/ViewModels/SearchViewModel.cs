using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RadioPlayer.Models;

namespace RadioPlayer.ViewModels {
    public partial class SearchViewModel : ViewModelBase {
        private const string BaseApiUrl = "http://de1.api.radio-browser.info/json";
        private CancellationTokenSource? _searchCts;

        [Reactive]
        private string _searchQuery = string.Empty;

        [Reactive]
        private bool _isLoading = false;

        [Reactive]
        private bool _hasSearched = false;

        public ObservableCollection<RadioStationViewModel> SearchResults { get; } = new();

        public bool HasResults => SearchResults.Count > 0;
        public bool ShowNoResults => HasSearched && !HasResults && !IsLoading;
        public bool ShowPlaceholder => !HasSearched && !IsLoading;

        public ICommand SearchCommand { get; }

        public SearchViewModel() {
            SearchCommand = ReactiveCommand.CreateFromTask(
                ExecuteSearchAsync,
                this.WhenAnyValue(x => x.SearchQuery, q => !string.IsNullOrWhiteSpace(q)));

            // Mise à jour des propriétés dérivées
            SearchResults.CollectionChanged += (_, _) => {
                this.RaisePropertyChanged(nameof(HasResults));
                this.RaisePropertyChanged(nameof(ShowNoResults));
            };

            this.WhenAnyValue(x => x.HasSearched, x => x.IsLoading)
                .Subscribe(_ => {
                    this.RaisePropertyChanged(nameof(ShowNoResults));
                    this.RaisePropertyChanged(nameof(ShowPlaceholder));
                });
        }

        private async Task ExecuteSearchAsync() {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;

            // Annuler la recherche précédente
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            IsLoading = true;
            HasSearched = true;
            SearchResults.Clear();

            try {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "RadioPlayer/1.0");

                var url = $"{BaseApiUrl}/stations/search?name={Uri.EscapeDataString(SearchQuery.Trim())}&order=clickcount&reverse=true&limit=50";

                var response = await httpClient.GetAsync(url, token);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync(token);
                var stations = JsonSerializer.Deserialize<List<RadioStation>>(jsonResponse);

                if (stations != null && !token.IsCancellationRequested) {
                    foreach (var station in stations) {
                        SearchResults.Add(new RadioStationViewModel(station));
                    }
                }

                Debug.WriteLine($"[Search] Found {SearchResults.Count} results for '{SearchQuery}'");
            } catch (OperationCanceledException) {
                // Recherche annulée
            } catch (Exception ex) {
                Debug.WriteLine($"[Search] Error: {ex.Message}");
            } finally {
                if (!token.IsCancellationRequested) {
                    IsLoading = false;
                }
            }
        }
    }
}
