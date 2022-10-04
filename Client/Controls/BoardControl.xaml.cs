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
using ChessGame;

namespace Client
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private GameManager m_gameEngine;
        private ToolHelper m_toolsHelper;
        bool isGameRunning = false;

        public BoardControl(GameManager Engine)
        {
            InitializeComponent();
            m_toolsHelper = new ToolHelper();
            m_mainGrid.Drop += new DragEventHandler(grid_OnDrop);

            m_gameEngine = Engine;
            m_gameEngine.EndGameEvent += EndGameHandler;
            m_gameEngine.StartGameEvent += StartGameHandler;
        }

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
            ChessToolUI toolUI = m_toolsHelper.CreateUITool(tool);

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

        public void EndGameHandler(object sender, EventArgs args)
        {
            if (!isGameRunning)
            {
                return;
            }

            isGameRunning = false;
            StopGame();
            ClearBoard();
        }

        public void StartGameHandler(object sender, EventArgs args)
        {
            if (isGameRunning)
            {
                return;
            }

            isGameRunning = true;
            StartGame();
        }

        public void StartGame()
        {
            BoardState boardState = m_gameEngine.GetBoardState();

            m_gameEngine.StateChangeEvent += boardChangeHandler;
            m_gameEngine.CheckmateEvent += checkmateHandler;

            m_gameGrid.AllowDrop = true;

            if (m_gameGrid.IsLoaded)
            {
                CreateGameGrid(this, null);
            }

            foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
            {
                addToolToMap(pair);
            }
        }

        private void checkmateHandler(object sender, EventArgs args)
        {
            StopGame();
        }

        private void StopGame()
        {
            m_gameGrid.AllowDrop = false;
            m_gameEngine.StateChangeEvent -= boardChangeHandler;
            m_gameEngine.CheckmateEvent -= checkmateHandler;

            UIElementCollection elementCollection = m_gameGrid.Children;
            foreach (UIElement element in elementCollection)
            {
                if (element is ChessToolUI)
                {
                    element.MouseMove -= tool_MouseMove;
                }
            }
        }

        public void ClearBoard()
        {
            int childrenCount = m_gameGrid.Children.Count;

            IList<ChessToolUI> toolsList = new List<ChessToolUI>();

            foreach(UIElement element in m_gameGrid.Children)
            {
                if (element is ChessToolUI)
                {
                    toolsList.Add(element as ChessToolUI);
                }
            }

            foreach(ChessToolUI tool in toolsList)
            {
                m_gameGrid.Children.Remove(tool);
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

        private static int m_gridCellSize = 50;

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
                    Uri imgSource;
                    if ((i + j) % 2 == 1)
                    {
                        imgSource = new Uri("pack://application:,,,/Resources/square_brown_dark.png");
                    }
                    else
                    {
                        imgSource = new Uri("pack://application:,,,/Resources/square_brown_light.png");
                    }

                    BitmapImage imgBitmap = new BitmapImage(imgSource);
                    Image squareImg = new Image();
                    squareImg.Source = imgBitmap;

                    m_gameGrid.Children.Add(squareImg);
                    Grid.SetRow(squareImg, i);
                    Grid.SetColumn(squareImg, j);
                }
            }
        }

        private void addToolToMap(KeyValuePair<BoardPosition, ITool> pair)
        {
            ITool tool = pair.Value;

            ChessToolUI uiTool = m_toolsHelper.CreateUITool(tool);
            uiTool.MouseMove += tool_MouseMove;

            BoardPosition toolPosition = pair.Key;

            int column = toolPosition.Column;
            int raw = toolPosition.Row;

            m_gameGrid.Children.Add(uiTool);
            Grid.SetColumn(uiTool, column);
            Grid.SetRow(uiTool, raw);
        }

        private void tool_MouseMove(object sender, MouseEventArgs e)
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

        private void grid_OnDrop(object sender, DragEventArgs args)
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
