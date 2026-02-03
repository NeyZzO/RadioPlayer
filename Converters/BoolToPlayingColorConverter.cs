using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace RadioPlayer.Converters {
    public class BoolToPlayingColorConverter : IValueConverter {
        public static readonly BoolToPlayingColorConverter Instance = new();

        private static readonly IBrush PlayingBrush = new SolidColorBrush(Color.Parse("#1DB954"));
        private static readonly IBrush DefaultBrush = new SolidColorBrush(Colors.White);

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is bool isPlaying && isPlaying) {
                return PlayingBrush;
            }
            return DefaultBrush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
