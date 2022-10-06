using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client.Board
{
    /// <summary>
    /// Interaction logic for BoardSquare.xaml
    /// </summary>
    public partial class BoardSquare
    {
        private static readonly DependencyProperty BackgroundImageSourceProperty = DependencyProperty.Register("BackgroundImageSource", typeof(BitmapImage), typeof(BoardSquare));
        private static readonly DependencyProperty ToolImageSourceProperty = DependencyProperty.Register("ToolImageSource", typeof(BitmapImage), typeof(BoardSquare));

        public BitmapImage BackgroundImageSource
        {
            set => SetValue(BackgroundImageSourceProperty, value);
            get => (BitmapImage)GetValue(BackgroundImageSourceProperty);
        }

        public BitmapImage ToolImageSource
        {
            set => SetValue(ToolImageSourceProperty, value);
            get => (BitmapImage)GetValue(ToolImageSourceProperty);
        }


        public BoardSquare()
        {
            InitializeComponent();
        }
    }
}
