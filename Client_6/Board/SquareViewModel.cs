using System;
using System.Windows;
using System.Windows.Input;
using Common;

namespace Client.Board
{
    public enum SquareState
    {
        Regular = 1,
        Chosen  = 2,
        Hinted  = 3
    }

    public enum SquareShadeEnum
    {
        Light = 1,
        Dark = 2,
    }

    public class SquareViewModel : DependencyObject
    {
        public static readonly DependencyProperty SquareShadeProperty = DependencyProperty.Register("SquareShade", typeof(SquareShadeEnum), typeof(SquareViewModel));
        public static readonly DependencyProperty ToolProperty = DependencyProperty.Register("Tool", typeof(ITool), typeof(SquareViewModel));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(SquareState), typeof(SquareViewModel), new UIPropertyMetadata(SquareState.Regular));
        private Action<BoardPosition, ITool> m_clickHandler;
        private ITool m_tool;

        public SquareViewModel(Action<BoardPosition, ITool> clickHandler, Func<object,bool> canExecuteClick,BoardPosition position)
        {
            Position       = position;
            m_clickHandler = clickHandler;
            ClickCommand   = new WpfCommand(clickCommandExecute, canExecuteClick);
            SquareShade = ResolveBackgroundShade(position);
        }

        public SquareShadeEnum SquareShade
        {
            get => (SquareShadeEnum)GetValue(SquareShadeProperty);
            set => SetValue(SquareShadeProperty, value);
        }
        public ITool Tool
        {
            get => (ITool)GetValue(ToolProperty);
            set => SetValue(ToolProperty, value);
        }
    
        public SquareState State
        {
            get => (SquareState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }
        public  BoardPosition Position     { get; }

        public ICommand      ClickCommand { get; }
        private SquareShadeEnum ResolveBackgroundShade(BoardPosition position)
        {
            int sumColAndRow = position.Row + position.Column;
            if (sumColAndRow % 2 == 1)
            {
                return SquareShadeEnum.Dark;
            }

            return SquareShadeEnum.Light;

        }
        private void clickCommandExecute(object parameter)
        {
            m_clickHandler.Invoke(Position, Tool);
        }
    }
}