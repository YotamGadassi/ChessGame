using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Game
{
    internal class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object     value, Type targetType, object parameter, CultureInfo culture)
        {
            Color inputColor  = (Color)value;
            Brush outputBrush = new SolidColorBrush(inputColor);

            return outputBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
