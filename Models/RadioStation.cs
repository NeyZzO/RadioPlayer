using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace RadioPlayer.Models;

public class RadioStation {
    public string Genre { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }

    public string changeuuid { get; set; }
    public string stationuuid { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string url_resolved { get; set; }
    public string homepage { get; set; }
    public string favicon { get; set; }
    
    [JsonPropertyName("tags")]
    public string TagsString { get; set; }
    
    [JsonIgnore]
    public string[] tags
    {
        get => string.IsNullOrEmpty(TagsString) ? Array.Empty<string>() : TagsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        set => TagsString = value != null ? string.Join(",", value) : string.Empty;
    }
    
    public string country { get; set; }
    public string countrycode { get; set; }
    public string iso_3166_2 { get; set; }
    public string state { get; set; }
    public string language { get; set; }
    public string languagecodes { get; set; }
    public int votes { get; set; }
    public string lastchangetime { get; set; }
    public DateTime lastchangetime_iso8601 { get; set; }
    public string codec { get; set; }
    public int bitrate { get; set; }
    public int hls { get; set; }
    public int lastcheckok { get; set; }
    public string lastchecktime { get; set; }
    public DateTime lastchecktime_iso8601 { get; set; }
    public string lastcheckoktime { get; set; }
    public DateTime lastcheckoktime_iso8601 { get; set; }
    public string lastlocalchecktime { get; set; }
    public DateTime lastlocalchecktime_iso8601 { get; set; }
    public string clicktimestamp { get; set; }
    public object clicktimestamp_iso8601 { get; set; }
    public int clickcount { get; set; }
    public int clicktrend { get; set; }
    public int ssl_error { get; set; }
    public bool has_extended_info { get; set; }
}
