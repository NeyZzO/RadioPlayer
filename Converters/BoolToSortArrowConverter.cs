using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RadioPlayer.Converters {
    public class BoolToSortArrowConverter : IValueConverter {
        public static readonly BoolToSortArrowConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is bool isDescending) {
                return isDescending ? "↓" : "↑";
            }
            return "↓";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
