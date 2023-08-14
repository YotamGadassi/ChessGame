using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Board
{
    internal class RowLabelConverter : IValueConverter
    {
        public object Convert(object     value, Type targetType, object parameter, CultureInfo culture)
        {
            int row = (int)value;
            return 9 - row;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
