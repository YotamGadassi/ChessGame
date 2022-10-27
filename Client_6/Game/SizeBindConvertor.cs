using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Game
{
    public class SizeBindConverter : IValueConverter
    {
        public object Convert(object      value
                            , Type        targetType
                            , object      parameter
                            , CultureInfo culture)
        {
            if (value is double size)
            {
                if (double.TryParse(parameter.ToString(), out double factor))
                {
                    return size * factor;
                }
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
