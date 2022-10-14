using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessBoard;
using Client.Command;
using Client.Helpers;
using Common;

namespace Client.Board
{
    public enum SquareState
    {
        Regular = 1,
        Chosen  = 2,
        Hinted  = 3
    }

    public class SquareViewModel : DependencyObject
    {
        public static readonly DependencyProperty BackgroundImageSourceProperty = DependencyProperty.Register("BackgroundImageSource", typeof(BitmapImage), typeof(SquareViewModel));
        public static readonly DependencyProperty ToolImageSourceProperty = DependencyProperty.Register("ToolImageSource", typeof(BitmapImage), typeof(SquareViewModel));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(SquareState), typeof(SquareViewModel), new UIPropertyMetadata(SquareState.Regular));
        private Action<BoardPosition, ITool> m_clickHandler;
        private ITool m_tool;

        public SquareViewModel(Action<BoardPosition, ITool> clickHandler, BoardPosition position)
        {
            Position       = position;
            m_clickHandler = clickHandler;
            ClickCommand   = new WpfCommand(clickCommandExecute);
            setBackground(position);
        }

        public BitmapImage BackgroundImageSource
        {
            get => (BitmapImage)GetValue(BackgroundImageSourceProperty);
            set => SetValue(BackgroundImageSourceProperty, value);
        }
        public BitmapImage ToolImageSource
        {
            get => (BitmapImage)GetValue(ToolImageSourceProperty);
            set => SetValue(ToolImageSourceProperty, value);
        }
        public ITool Tool
        {
            get => m_tool;
            set
            {
                m_tool = value;
                setImageSource(m_tool);
            }
        }
        public SquareState State
        {
            get => (SquareState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }
        public  BoardPosition Position     { get; }
        private void setImageSource(ITool tool)
        {
            if (null == tool)
            {
                ToolImageSource = null;
                return;
            }
            Type  toolType      = tool.GetType();
            Color toolColor     = tool.Color;
            bool  isImageExists = ToolsImageHelper.TryGetBitmapImage(toolType, toolColor, out BitmapImage bitmapImage);
            if (false == isImageExists)
            {
                ToolImageSource = null;
                //log
            }

            ToolImageSource = bitmapImage;

        }
        public ICommand      ClickCommand { get; }
        private void setBackground(BoardPosition position)
        {
            Uri imageUri;
            int sumColAndRow = position.Row + position.Column;
            if (sumColAndRow % 2 == 1)
            {
                imageUri = new Uri("pack://application:,,,/Client;component/Resources/square_brown_dark.png");
            }
            else
            {
                imageUri = new Uri("pack://application:,,,/Client;component/Resources/square_brown_light.png");
            }
            
            BackgroundImageSource = new BitmapImage(imageUri);
        }
        private void clickCommandExecute(object parameter)
        {
            m_clickHandler.Invoke(Position, Tool);
        }
    }
}