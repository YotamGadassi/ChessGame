using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Board;
using Common;

namespace Client.Board
{
    public class SquareConverter : IMultiValueConverter
    {
        public object   Convert(object[]   values, Type   targetType,  object parameter, CultureInfo culture)
        {
            UIElement uiElement  = (UIElement)values[0];
            int       elementCol = Grid.GetColumn(uiElement);
            int       elementRow = Grid.GetRow(uiElement);

            if (elementRow < 1 || elementCol < 1 ||elementRow > 8 || elementCol > 8)
            {
                //TODO: log error
                return Binding.DoNothing;
            }

            Dictionary<BoardPosition, SquareViewModel> dict     = (Dictionary<BoardPosition, SquareViewModel>)values[1];
            BoardPosition                              position = getBoardPosition(elementCol, elementRow);
            if (false == dict.TryGetValue(position, out SquareViewModel squareVM))
            {
                // TODO: log
                return Binding.DoNothing;
            }

            return squareVM;
        }

        private BoardPosition getBoardPosition(int elementCol, int elementRow)
        {
            int reversedCol = 9 - elementCol;
            int reveresRow  = 9 - elementRow;
            return new BoardPosition(reversedCol, reveresRow);
        }

        public object[] ConvertBack(object value,  Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
