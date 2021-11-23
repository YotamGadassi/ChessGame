using Common;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;

using Rectangle = System.Windows.Shapes.Rectangle;

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private ObservableDictionary<BoardPosition, ToolType> boardState;

        public BoardControl()
        {
            InitializeComponent();

            CreateGrid();
            ColorTheGrid();
        }

        private void CreateGrid()
        {
            for (int i = 0; i < 9; ++i)
            {
                m_mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                m_mainGrid.RowDefinitions.Add(new RowDefinition());
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
            boardState = new ObservableDictionary<BoardPosition, ToolType>(state);
        }
    }
}
