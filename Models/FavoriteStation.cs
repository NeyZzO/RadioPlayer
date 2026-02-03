using System.Text.Json.Serialization;

namespace RadioPlayer.Models {
    /// <summary>
    /// Modèle simplifié pour le stockage des favoris
    /// </summary>
    public class FavoriteStation {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("countrycode")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("favicon")]
        public string Favicon { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = [];

        [JsonPropertyName("url")]
        public string StreamUrl { get; set; } = string.Empty;

        /// <summary>
        /// Crée un FavoriteStation à partir d'une RadioStation
        /// </summary>
        public static FavoriteStation FromRadioStation(RadioStation station) {
            return new FavoriteStation {
                Uuid = station.stationuuid,
                Name = station.name,
                Country = station.country,
                CountryCode = station.countrycode,
                Favicon = station.favicon,
                Tags = station.tags,
                StreamUrl = station.url
            };
        }

        /// <summary>
        /// Convertit en RadioStation pour compatibilité avec les ViewModels existants
        /// </summary>
        public RadioStation ToRadioStation() {
            return new RadioStation {
                stationuuid = Uuid,
                name = Name,
                country = Country,
                countrycode = CountryCode,
                favicon = Favicon,
                TagsString = string.Join(",", Tags),
                url = StreamUrl,
                IsFavorite = true
            };
        }
    }
}
