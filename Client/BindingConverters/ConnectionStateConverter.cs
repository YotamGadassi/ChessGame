using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.BindingConverters
{
    public class ConnectionStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectionState connectionState = (ConnectionState)value;

            switch(connectionState)
            {
                case ConnectionState.Connected:
                    return "Connected";
                case ConnectionState.Disconnected:
                    return "Disconnected";
                default:
                    throw new ArgumentException(string.Format("Connection State {0} is not value", connectionState));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
