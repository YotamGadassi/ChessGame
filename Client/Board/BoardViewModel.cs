using System;
using System.Collections.Generic;
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
        Chosen = 2,
        Hinted = 3
    }

    public class SquareViewModel : DependencyObject
    {
        public static readonly DependencyProperty BackgroundImageSourceProperty = DependencyProperty.Register("BackgroundImageSource", typeof(BitmapImage), typeof(SquareViewModel));
        public static readonly DependencyProperty ToolImageSourceProperty = DependencyProperty.Register("ToolImageSource", typeof(BitmapImage), typeof(SquareViewModel));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(SquareState), typeof(SquareViewModel), new UIPropertyMetadata(SquareState.Regular));
        private Action<BoardPosition> m_clickHandler;

        public SquareViewModel(Action<BoardPosition> clickHandler, BoardPosition position)
        {
            Position       = position;
            m_clickHandler = clickHandler;
            clickCommand   = new WpfCommand(clickCommandExecute);
            setBackground(position);
        }

        private void setBackground(BoardPosition position)
        {
            Uri imageURI;
            int sumColAndRow = position.Row + position.Column;
            if (sumColAndRow % 2 == 1)
            {
                imageURI = new Uri("pack://application:,,,/Resources/square_brown_dark.png");
            }
            else
            {
                imageURI = new Uri("pack://application:,,,/Resources/square_brown_dark.png");
            }
            
            BackgroundImageSource = new BitmapImage(imageURI);
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
        private ITool m_tool;
        public ITool Tool
        {
            get => m_tool;
            set
            {
                m_tool = value;
                setImageSource(m_tool);
            }
        }
        private void setImageSource(ITool tool)
        {
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
        public SquareState State
        {
            get => (SquareState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }
        public  BoardPosition   Position   { get; }
        private ICommand clickCommand { get; }
        
        private void clickCommandExecute(object parameter)
        {
            m_clickHandler.Invoke(Position);
        }
    }

    public class BoardViewModel : DependencyObject
    {
        public readonly Dictionary<BoardPosition, SquareViewModel> SquaresDictionary;

        public event EventHandler<EventArgs> ClickCommandEvent;

        public BoardViewModel()
        {
            SquaresDictionary = new Dictionary<BoardPosition, SquareViewModel>();
            initSquares();
        }

        private void initSquares()
        {
            BoardPosition pos = new BoardPosition(0, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));
            
            pos = new BoardPosition(1, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

            pos = new BoardPosition(2, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

            pos = new BoardPosition(3, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

            pos = new BoardPosition(4, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

            pos = new BoardPosition(5, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

            pos = new BoardPosition(6, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

            pos = new BoardPosition(7, 0);
            SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));

        }

        public void ClickCommandExecute(BoardPosition position)
        {
            
        }

    }

}
