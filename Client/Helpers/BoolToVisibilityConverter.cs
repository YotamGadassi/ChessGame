using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client.Helpers
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object      value
                            , Type        targetType
                            , object      parameter
                            , CultureInfo culture)
        {
            if (null == value)
            {
                return Visibility.Hidden;
            }
            
            if (value is string str)
            {
                if (string.IsNullOrEmpty(str))
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object      value
                                , Type        targetType
                                , object      parameter
                                , CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
