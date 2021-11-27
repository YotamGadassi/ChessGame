using Common;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Specialized;

using Rectangle = System.Windows.Shapes.Rectangle;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Game;
using ChessBoard;

namespace Client
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private GameEngine m_gameEngine;

        public void boardChangeHandler(object sender, ChessBoardEventArgs args)
        {
            if (args.Type == EventType.Remove)
            {
                removeToolHandler(sender, args.StartPosition);
                return;
            }

            if (args.Type == EventType.Add)
            {
                addToolHandler(sender, args.StartPosition, args.ToolAtStartPosition);
                return;
            }

            if (args.Type == EventType.Move)
            {
                moveToolHandel(args.StartPosition, args.EndPosition);
                return;
            }

        }

        public void addToolHandler(object sender, BoardPosition position, ITool tool)
        {
            ChessToolUI toolUI = createUITool(tool);

            m_gameGrid.Children.Add(toolUI);
            Grid.SetColumn(toolUI, position.Column);
            Grid.SetRow(toolUI, position.Row);

        }

        public void removeToolHandler(object sender, BoardPosition positionToRemove)
        {
            ChessToolUI toolToRemove = getTool(positionToRemove);
            if(null != toolToRemove)
            {
                m_gameGrid.Children.Remove(toolToRemove);
            }
        }

        private ChessToolUI getTool(BoardPosition position)
        {
            UIElementCollection elementCollection = m_gameGrid.Children;
            ChessToolUI tool = null;

            foreach (UIElement element in elementCollection)
            {
                if (Grid.GetColumn(element) == position.Column && Grid.GetRow(element) == position.Row)
                {
                    if (element is ChessToolUI)
                    {
                        return tool = (ChessToolUI)element;
                    }
                }
            }

            return tool;
        }

        public void moveToolHandel(BoardPosition start, BoardPosition end)
        {
            ChessToolUI toolAtStartPoint = getTool(start);
            ChessToolUI toolAtEndPoint = getTool(end);

            if (null != toolAtEndPoint)
            {
                m_gameGrid.Children.Remove(toolAtEndPoint);
            }

            Grid.SetColumn(toolAtStartPoint, end.Column);
            Grid.SetRow(toolAtStartPoint, end.Row);
        }

        private Dictionary<string, BitmapImage> m_ToolToImageSource;

        private static int m_gridCellSize = 50;

        public BoardControl(GameEngine Engine)
        {
            InitializeComponent();

            m_ToolToImageSource = new Dictionary<string, BitmapImage>();
            m_mainGrid.Drop += new DragEventHandler(grid_OnDrop);

            m_gameEngine = Engine;

            loadImages();

            m_gameEngine.StateChangeEvent += boardChangeHandler;
        }

        private void loadImages()
        {
            BitmapImage src = new BitmapImage(new Uri("pack://application:,,,/Resources/pawn.png"));
            m_ToolToImageSource["Pawn"] = src;
        }

        private void CreateMainGrid(object sender, RoutedEventArgs args)
        {
            AddColRowToGrid(m_mainGrid, 9, m_gridCellSize, 9, m_gridCellSize);

            for (int i = 1; i < 9; ++i)
            {
                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Content = i.ToString();
                m_mainGrid.Children.Add(label);
                Grid.SetColumn(label, i);

                Label label1 = new Label();
                label1.HorizontalAlignment = HorizontalAlignment.Center;
                label1.VerticalAlignment = VerticalAlignment.Center;
                label1.Content = ((char)('a' + i - 1)).ToString();
                m_mainGrid.Children.Add(label1);
                Grid.SetColumn(label1, 0);
                Grid.SetRow(label1, i);
            }
        }

        private void CreateGameGrid(object sender, RoutedEventArgs args)
        {
            Grid.SetColumn(m_gameGrid, 1);
            Grid.SetRow(m_gameGrid, 1);
            Grid.SetColumnSpan(m_gameGrid, 8);
            Grid.SetRowSpan(m_gameGrid, 8);

            AddColRowToGrid(m_gameGrid, 8, m_gridCellSize, 8, m_gridCellSize);
            ColorGameGrid();
        }
        
        private void ColorGameGrid()
        {
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                    Color color = Colors.White;
                    if ((i + j) % 2 == 1)
                        color = Colors.Black;

                    m_gameGrid.Children.Add(rectangle);
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    rectangle.Fill = new SolidColorBrush(color);
                }
            }
        }

        private void addToolToMap(KeyValuePair<BoardPosition, ITool> pair)
        {
            ITool tool = pair.Value;

            ChessToolUI uiTool = createUITool(tool);

            BoardPosition toolPosition = uiTool.Position;

            int column = toolPosition.Column;
            int raw = toolPosition.Row;

            m_gameGrid.Children.Add(uiTool);
            Grid.SetColumn(uiTool, column);
            Grid.SetRow(uiTool, raw);
        }

        private ChessToolUI createUITool(ITool tool)
        {
            Image img = new Image();
            img.Source = m_ToolToImageSource[tool.Type];
            ChessToolUI newTool = new ChessToolUI(img, tool);
            
            newTool.MouseMove += new MouseEventHandler(tool_MouseMove);

            return newTool;
        }

        public void StartGame()
        {
            BoardState boardState = m_gameEngine.GetBoardState();

            foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
            {
                addToolToMap(pair);
            }
        }

        public void tool_MouseMove(object sender, MouseEventArgs e)
        {
            ChessToolUI uiTool = sender as ChessToolUI;
            if(uiTool == null || m_gameEngine.CurrentTeamTurn.Color != uiTool.Color)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject dataObject = new DataObject(uiTool);

                DragDrop.DoDragDrop(uiTool, dataObject, DragDropEffects.Move);
            }
        }

        public void grid_OnDrop(object sender, DragEventArgs args)
        {
            ChessToolUI uiTool = (ChessToolUI)args.Data.GetData(typeof(ChessToolUI));
            
            Point position = args.GetPosition((Grid)sender);
            int column = (int)position.X / m_gridCellSize - 1;
            int row = (int)position.Y / m_gridCellSize - 1;

            BoardPosition newPosition = new BoardPosition(column, row);

            int oldColumn = Grid.GetColumn(uiTool);
            int oldRow = Grid.GetRow(uiTool);
            BoardPosition oldPosition = new BoardPosition(oldColumn, oldRow);

            m_gameEngine.Move(oldPosition, newPosition);
        }

        private void AddColRowToGrid(Grid grid, int columns, int columnSize, int rows, int rowSize)
        {
            for (int i = 0; i < columns; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(columnSize);

                grid.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(rowSize);

                grid.RowDefinitions.Add(row);
            }
        }
    }
}
