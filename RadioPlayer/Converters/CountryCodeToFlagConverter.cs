using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RadioPlayer.Converters;

public class CountryCodeToFlagConverter : IValueConverter
{
    public static readonly CountryCodeToFlagConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string countryCode || string.IsNullOrWhiteSpace(countryCode))
            return "🌍";

        countryCode = countryCode.ToUpperInvariant();

        if (countryCode.Length != 2)
            return "🌍";

        // Convertit le code pays ISO 3166-1 alpha-2 en emoji drapeau
        // Les drapeaux emoji sont composés de deux "regional indicator symbols"
        // A = U+1F1E6, B = U+1F1E7, etc.
        var firstChar = char.ConvertFromUtf32(0x1F1E6 + (countryCode[0] - 'A'));
        var secondChar = char.ConvertFromUtf32(0x1F1E6 + (countryCode[1] - 'A'));

        return $"{firstChar}{secondChar}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
