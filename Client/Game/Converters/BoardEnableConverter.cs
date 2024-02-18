using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Common;
using log4net;

namespace Client.Game;

public class BoardEnableConverter : IMultiValueConverter
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(BoardEnableConverter));

    public object Convert(object[]    values
                        , Type        targetType
                        , object      parameter
                        , CultureInfo culture)
    {
        if (values.Length != 2)
        {
            s_log.Error($"The amount of values if incorrect. expected: 2, received: {values.Length}");
            return Binding.DoNothing;
        }

        if (values[0] is Visibility messageVisibility
         && values[1] is GameStateEnum gameState)
        {
            return messageVisibility != Visibility.Visible && gameState == GameStateEnum.Running;
        }

        s_log.Error($"The values types are incorrect. [[0]: {values[0].GetType()} | [1]: {values[1].GetType()}");
        return Binding.DoNothing;
    }

    public object[] ConvertBack(object      value
                              , Type[]      targetTypes
                              , object      parameter
                              , CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}