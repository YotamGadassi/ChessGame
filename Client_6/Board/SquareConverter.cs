using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Common_6;

namespace Client.Board
{
    public class SquareConverter : IMultiValueConverter
    {
        public object   Convert(object[]   values, Type   targetType,  object parameter, CultureInfo culture)
        {
            UIElement uiElement  = (UIElement)values[0];
            int       elementCol = Grid.GetColumn(uiElement);
            int       elementRow = Grid.GetRow(uiElement);

            if (elementRow < 0 || elementCol < 0)
            {
                //TODO: log error
                return Binding.DoNothing;
            }

            Dictionary<BoardPosition, SquareViewModel> dict     = (Dictionary<BoardPosition, SquareViewModel>)values[1];
            BoardPosition                              position = new BoardPosition(elementCol, elementRow);
            if (false == dict.TryGetValue(position, out SquareViewModel squareVM))
            {
                // TODO: log
                return Binding.DoNothing;
            }

            return squareVM;
        }

        public object[] ConvertBack(object value,  Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
