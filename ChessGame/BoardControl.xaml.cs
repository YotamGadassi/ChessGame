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

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private ObservableDictionary<BoardPosition, ToolType> m_boardState;
        private Dictionary<ToolType, BitmapImage> m_ToolToImageSource;

        private static int m_gridCellSize = 50;

        public BoardControl()
        {
            InitializeComponent();

            m_ToolToImageSource = new Dictionary<ToolType, BitmapImage>();

            m_mainGrid.Drop += new DragEventHandler(grid_OnDrop);

            BitmapImage src = new BitmapImage(new Uri("pack://application:,,,/Resources/pawn.jpg"));
            m_ToolToImageSource[ToolType.Pawn] = src;
        }

        private void drawOnMap (object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                addToolToMap(args.NewItems[0]);
                return;
            }
            /*
                        if (args.Action == NotifyCollectionChangedAction.Replace)
                        {
                            replaceToolInMap(args.NewItems[args.NewStartingIndex], args.OldItems[args.OldStartingIndex]);
                            return;
                        }

                        if (args.Action == NotifyCollectionChangedAction.Remove)
                        {
                            removeToolFromMap(args.NewItems[args.NewStartingIndex]);
                            return;
                        }
            */
            return;      
        }

        private void addToolToMap(object v)
        {
            KeyValuePair<BoardPosition, ToolType> newPair = (KeyValuePair < BoardPosition, ToolType > )v;
            Image img = createImage(newPair.Value);
            int column = (int)newPair.Key.Position.X;
            int raw = (int)newPair.Key.Position.Y;

            m_mainGrid.Children.Add(img);
            Grid.SetColumn(img, column);
            Grid.SetRow(img, raw);
        }

        private Image createImage(ToolType value)
        {
            Image img = new Image();
            img.Source = m_ToolToImageSource[value];
            img.MouseMove += new MouseEventHandler(tool_MouseMove);
            return img;
        }

        public void init_Grid(object sender, RoutedEventArgs args)
        {
            m_mainGrid = sender as Grid;
            CreateGrid();
            ColorTheGrid();
        }

        private void CreateGrid()
        {
            for (int i = 0; i < 9; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(m_gridCellSize);
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(m_gridCellSize);

                m_mainGrid.ColumnDefinitions.Add(col);
                m_mainGrid.RowDefinitions.Add(row);
            }

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

        private void ColorTheGrid()
        {
            for (int i = 1; i < 9; ++i)
            {
                for (int j = 1; j < 9; ++j)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                    Color color = Colors.White;
                    if ((i + j) % 2 == 1)
                        color = Colors.Black;

                    m_mainGrid.Children.Add(rectangle);
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    rectangle.Fill = new SolidColorBrush(color);
                }
            }
        }

        public void StartGame(IMovementController controller)
        {
            Dictionary<BoardPosition, ToolType> state = controller.GetBoardState();
            m_boardState = new ObservableDictionary<BoardPosition, ToolType>(state);
            foreach(KeyValuePair<BoardPosition, ToolType> pair in m_boardState)
            {
                addToolToMap(pair);
            }
        }

        public void tool_MouseMove(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            if (img != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject dataObject = new DataObject(img);
            
                DragDrop.DoDragDrop(img, dataObject, DragDropEffects.Move);
            }
        }

        public void grid_OnDrop(object sender, DragEventArgs args)
        {
            Point position = args.GetPosition((Grid)sender);
            int column = (int)position.X / m_gridCellSize;
            int row = (int)position.Y / m_gridCellSize;

            Image element = (Image)args.Data.GetData(typeof(Image));

            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }
    }
}
