using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using RadioPlayer.Models;

namespace RadioPlayer.Services {
    public sealed class PlayerService : IDisposable {
        private static readonly Lazy<PlayerService> _instance = new(() => new());
        public static PlayerService Instance => _instance.Value;

        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _player;
        private Media? _currentMedia;

        // Subjects pour les notifications réactives
        private readonly BehaviorSubject<RadioStation?> _currentStationSubject = new(null);
        private readonly BehaviorSubject<bool> _isPlayingSubject = new(false);
        private readonly BehaviorSubject<int> _volumeSubject = new(50);

        // Observables publics
        public IObservable<RadioStation?> CurrentStationChanged => _currentStationSubject.AsObservable();
        public IObservable<bool> IsPlayingChanged => _isPlayingSubject.AsObservable();
        public IObservable<int> VolumeChanged => _volumeSubject.AsObservable();

        // Propriétés d'accès direct
        public RadioStation? CurrentStation => _currentStationSubject.Value;
        public bool IsPlaying => _isPlayingSubject.Value;
        public int Volume {
            get => _volumeSubject.Value;
            set => SetVolume(value);
        }

        private PlayerService() {
            Core.Initialize();
            _libVLC = new LibVLC("--no-video");
            _player = new MediaPlayer(_libVLC);

            // Écouter les événements du player
            _player.Playing += (_, _) => _isPlayingSubject.OnNext(true);
            _player.Stopped += (_, _) => _isPlayingSubject.OnNext(false);
            _player.EndReached += (_, _) => _isPlayingSubject.OnNext(false);
            _player.EncounteredError += (_, _) => {
                Debug.WriteLine("[PlayerService] Playback error encountered");
                _isPlayingSubject.OnNext(false);
            };

            // Volume initial
            _player.Volume = 50;
        }

        public void Play(RadioStation station) {
            if (string.IsNullOrEmpty(station.url_resolved)) {
                Debug.WriteLine("[PlayerService] Station URL is empty");
                return;
            }

            // Arrêter la lecture précédente
            Stop();

            // Créer le nouveau média
            _currentMedia?.Dispose();
            _currentMedia = new Media(_libVLC, new Uri(station.url_resolved));

            // Lancer la lecture
            _player.Media = _currentMedia;
            _player.Play();

            // Mettre à jour l'état
            _currentStationSubject.OnNext(station);

            Debug.WriteLine($"[PlayerService] Playing: {station.name}");

            // Enregistrer le clic auprès de l'API (fire and forget)
            _ = RegisterClickAsync(station.stationuuid);
        }

        public void Stop() {
            if (_player.IsPlaying) {
                _player.Stop();
            }
            _currentStationSubject.OnNext(null);
        }

        public void SetVolume(int volume) {
            var clampedVolume = Math.Clamp(volume, 0, 100);
            _player.Volume = clampedVolume;
            _volumeSubject.OnNext(clampedVolume);
        }

        private async Task RegisterClickAsync(string stationUuid) {
            if (string.IsNullOrEmpty(stationUuid)) return;

            try {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "RadioPlayer/1.0");
                var response = await httpClient.GetAsync($"http://de1.api.radio-browser.info/json/url/{stationUuid}");
                Debug.WriteLine($"[PlayerService] Click registered: {response.StatusCode}");
            } catch (Exception ex) {
                Debug.WriteLine($"[PlayerService] Failed to register click: {ex.Message}");
            }
        }

        public void Dispose() {
            _player.Stop();
            _currentMedia?.Dispose();
            _player.Dispose();
            _libVLC.Dispose();

            _currentStationSubject.Dispose();
            _isPlayingSubject.Dispose();
            _volumeSubject.Dispose();
        }
    }
}
