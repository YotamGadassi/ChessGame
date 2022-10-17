using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Board
{
    internal class ColumnLabelConverter : IValueConverter
    {
        public object Convert(object     value, Type targetType, object parameter, CultureInfo culture)
        {
            int  column = (int)value;
            char c      = (char)(column + 0x40);
            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
