using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    public partial class AllStationsViewModel : ViewModelBase {
        private const int PageSize = 30;
        private const string BaseApiUrl = "http://de1.api.radio-browser.info/json";
        private CancellationTokenSource? _searchCts;

        public ObservableCollection<RadioStationViewModel> Stations { get; } = new();

        // Pagination
        [Reactive] private int _currentPage = 1;
        [Reactive] private int _totalPages = 1;
        [Reactive] private int _totalStations = 0;
        [Reactive] private bool _isLoading = false;

        // Filtres
        [Reactive] private string _searchTags = string.Empty;
        [Reactive] private string _searchCountry = string.Empty;

        // Tri
        [Reactive] private string _selectedSort = "clickcount";
        [Reactive] private bool _isSortDescending = true;

        public List<SortOption> SortOptions { get; } = [
            new("Clics", "clickcount"),
            new("Votes", "votes"),
            new("Nom", "name"),
            new("Pays", "country")
        ];

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleSortOrderCommand { get; }

        public AllStationsViewModel() {
            NextPageCommand = ReactiveCommand.CreateFromTask(
                NextPageAsync,
                this.WhenAnyValue(x => x.CurrentPage, x => x.TotalPages, (current, total) => current < total));

            PreviousPageCommand = ReactiveCommand.CreateFromTask(
                PreviousPageAsync,
                this.WhenAnyValue(x => x.CurrentPage, current => current > 1));

            FirstPageCommand = ReactiveCommand.CreateFromTask(
                () => GoToPageAsync(1),
                this.WhenAnyValue(x => x.CurrentPage, current => current > 1));

            LastPageCommand = ReactiveCommand.CreateFromTask(
                () => GoToPageAsync(TotalPages),
                this.WhenAnyValue(x => x.CurrentPage, x => x.TotalPages, (current, total) => current < total));

            SearchCommand = ReactiveCommand.CreateFromTask(
                () => GoToPageAsync(1));

            ToggleSortOrderCommand = ReactiveCommand.Create(() => {
                IsSortDescending = !IsSortDescending;
            });

            // Debounce sur les filtres (500ms)
            this.WhenAnyValue(x => x.SearchTags, x => x.SearchCountry)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async _ => await LoadStationsAsync());

            // Recharger immédiatement quand le tri change
            this.WhenAnyValue(x => x.SelectedSort, x => x.IsSortDescending)
                .Skip(1) // Ignorer l'initialisation
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async _ => await LoadStationsAsync());

            // Charger les données initiales
            _ = InitializeAsync();
        }

        private async Task InitializeAsync() {
            await LoadTotalStationsCountAsync();
            await LoadStationsAsync();
        }

        private async Task LoadTotalStationsCountAsync() {
            try {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "RadioPlayer/1.0");
                var response = await httpClient.GetStringAsync($"{BaseApiUrl}/stats");
                var stats = JsonSerializer.Deserialize<JsonElement>(response);
                if (stats.TryGetProperty("stations", out var stationsCount)) {
                    TotalStations = stationsCount.GetInt32();
                    TotalPages = (int)Math.Ceiling((double)TotalStations / PageSize);
                }
            } catch (Exception ex) {
                Debug.WriteLine($"Erreur lors du chargement des stats : {ex.Message}");
            }
        }

        private async Task LoadStationsAsync() {
            // Annuler la recherche précédente
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            IsLoading = true;
            Stations.Clear();

            try {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "RadioPlayer/1.0");

                var offset = (CurrentPage - 1) * PageSize;
                var order = IsSortDescending ? "reverse=true" : "reverse=false";

                // Construction de l'URL avec les filtres
                var url = $"{BaseApiUrl}/stations/search?order={SelectedSort}&{order}&limit={PageSize}&offset={offset}";

                if (!string.IsNullOrWhiteSpace(SearchTags)) {
                    url += $"&tag={Uri.EscapeDataString(SearchTags.Trim())}";
                }

                if (!string.IsNullOrWhiteSpace(SearchCountry)) {
                    url += $"&country={Uri.EscapeDataString(SearchCountry.Trim())}";
                }

                var response = await httpClient.GetAsync(url, token);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync(token);
                var stations = JsonSerializer.Deserialize<List<RadioStation>>(jsonResponse);

                if (stations != null && !token.IsCancellationRequested) {
                    foreach (var station in stations) {
                        Stations.Add(new RadioStationViewModel(station));
                    }
                }

                // Mettre à jour le nombre total si on a des filtres
                if (!string.IsNullOrWhiteSpace(SearchTags) || !string.IsNullOrWhiteSpace(SearchCountry)) {
                    await UpdateFilteredCountAsync(token);
                } else {
                    await LoadTotalStationsCountAsync();
                }
            } catch (OperationCanceledException) {
                // Recherche annulée, ignorer
            } catch (Exception ex) {
                Debug.WriteLine($"Erreur lors du chargement des stations : {ex.Message}");
            } finally {
                if (!token.IsCancellationRequested) {
                    IsLoading = false;
                }
            }
        }

        private async Task UpdateFilteredCountAsync(CancellationToken token) {
            try {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "RadioPlayer/1.0");

                // TODO: L'API radio-browser ne fournit pas facilement un count filtré
                // Pour l'instant, on estime basé sur les résultats retournés
                // Si on a moins de PageSize résultats, c'est la dernière page
                if (Stations.Count < PageSize) {
                    var estimatedTotal = (CurrentPage - 1) * PageSize + Stations.Count;
                    TotalStations = estimatedTotal;
                    TotalPages = CurrentPage;
                }
            } catch (Exception ex) {
                Debug.WriteLine($"Erreur lors de la mise à jour du count : {ex.Message}");
            }
        }

        private async Task NextPageAsync() {
            if (CurrentPage < TotalPages) {
                CurrentPage++;
                await LoadStationsAsync();
            }
        }

        private async Task PreviousPageAsync() {
            if (CurrentPage > 1) {
                CurrentPage--;
                await LoadStationsAsync();
            }
        }

        private async Task GoToPageAsync(int page) {
            if (page >= 1 && page <= TotalPages) {
                CurrentPage = page;
                await LoadStationsAsync();
            }
        }
    }

    public record SortOption(string Label, string Value);
}
