using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using RadioPlayer.Models;

namespace RadioPlayer.Services {
    /// <summary>
    /// Service singleton pour la gestion des favoris
    /// </summary>
    public sealed class FavoritesService {
        private static readonly Lazy<FavoritesService> _instance = new(() => new FavoritesService());
        public static FavoritesService Instance => _instance.Value;

        private readonly string _favoritesFilePath;
        private readonly Subject<(string Uuid, bool IsFavorite)> _favoriteChangedSubject = new();
        private readonly HashSet<string> _favoriteUuids = new();

        /// <summary>
        /// Collection observable des favoris
        /// </summary>
        public ObservableCollection<FavoriteStation> Favorites { get; } = new();

        /// <summary>
        /// Observable émettant les changements de favoris
        /// </summary>
        public IObservable<(string Uuid, bool IsFavorite)> FavoriteChanged => _favoriteChangedSubject.AsObservable();

        /// <summary>
        /// Nombre de favoris
        /// </summary>
        public int Count => Favorites.Count;

        private FavoritesService() {
            // Chemin du fichier de favoris dans le dossier AppData
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var radioPlayerFolder = Path.Combine(appDataPath, "RadioPlayer");
            Directory.CreateDirectory(radioPlayerFolder);
            _favoritesFilePath = Path.Combine(radioPlayerFolder, "favorites.json");

            Load();
        }

        /// <summary>
        /// Vérifie si une station est en favori
        /// </summary>
        public bool IsFavorite(string? uuid) {
            if (string.IsNullOrEmpty(uuid)) return false;
            return _favoriteUuids.Contains(uuid);
        }

        /// <summary>
        /// Ajoute une station aux favoris
        /// </summary>
        public void AddFavorite(RadioStation station) {
            if (string.IsNullOrEmpty(station.stationuuid)) return;
            if (IsFavorite(station.stationuuid)) return;

            var favorite = FavoriteStation.FromRadioStation(station);
            Favorites.Add(favorite);
            _favoriteUuids.Add(station.stationuuid);

            _favoriteChangedSubject.OnNext((station.stationuuid, true));
            Save();

            Debug.WriteLine($"[FavoritesService] Added: {station.name}");
        }

        /// <summary>
        /// Retire une station des favoris
        /// </summary>
        public void RemoveFavorite(string uuid) {
            if (string.IsNullOrEmpty(uuid)) return;
            if (!IsFavorite(uuid)) return;

            var favorite = Favorites.FirstOrDefault(f => f.Uuid == uuid);
            if (favorite != null) {
                Favorites.Remove(favorite);
                _favoriteUuids.Remove(uuid);

                _favoriteChangedSubject.OnNext((uuid, false));
                Save();

                Debug.WriteLine($"[FavoritesService] Removed: {favorite.Name}");
            }
        }

        /// <summary>
        /// Bascule l'état favori d'une station
        /// </summary>
        public bool Toggle(RadioStation station) {
            if (IsFavorite(station.stationuuid)) {
                RemoveFavorite(station.stationuuid);
                return false;
            } else {
                AddFavorite(station);
                return true;
            }
        }

        /// <summary>
        /// Charge les favoris depuis le fichier JSON
        /// </summary>
        private void Load() {
            try {
                if (!File.Exists(_favoritesFilePath)) {
                    Debug.WriteLine("[FavoritesService] No favorites file found, starting fresh.");
                    return;
                }

                var json = File.ReadAllText(_favoritesFilePath);
                var favorites = JsonSerializer.Deserialize<List<FavoriteStation>>(json);

                if (favorites != null) {
                    Favorites.Clear();
                    _favoriteUuids.Clear();

                    foreach (var fav in favorites) {
                        Favorites.Add(fav);
                        _favoriteUuids.Add(fav.Uuid);
                    }

                    Debug.WriteLine($"[FavoritesService] Loaded {Favorites.Count} favorites.");
                }
            } catch (Exception ex) {
                Debug.WriteLine($"[FavoritesService] Error loading favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde les favoris dans le fichier JSON
        /// </summary>
        private void Save() {
            try {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Favorites.ToList(), options);
                File.WriteAllText(_favoritesFilePath, json);

                Debug.WriteLine($"[FavoritesService] Saved {Favorites.Count} favorites.");
            } catch (Exception ex) {
                Debug.WriteLine($"[FavoritesService] Error saving favorites: {ex.Message}");
            }
        }
    }
}
