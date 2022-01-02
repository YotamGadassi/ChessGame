using Client.Command;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainClientWindows : Window
    {
        private Window m_hostWin;
        private OfflineFramwork Framework = null;
        public PlayOnlineCommand playOnlineCommand { get; }

        public MainClientWindows()
        {
            InitializeComponent();
            playOnlineCommand = new PlayOnlineCommand();
            DataContext = this;
        }

        private void ButtonClick_StartOfflineGame(object sender, RoutedEventArgs e)
        {
            if(Framework != null)
            {
                return;
            }

            Framework = new OfflineFramwork();
            m_offlineGameGrid.Children.Add(Framework.boardControl);
            Grid.SetColumn(Framework.boardControl, 1);
            m_offlineGameGrid.Visibility = Visibility.Visible;
        }

        bool isGameRunning = false;
        private void ButtonClick_StartGame(object sender, RoutedEventArgs e)
        {
            if (isGameRunning)
            {
                return;
            }
            isGameRunning = true;
            Framework.StartGame();
        }

        private void ButtonClick_PauseGame(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonClick_EndGame(object sender, RoutedEventArgs e)
        {
            isGameRunning = false;
            Framework.EndGame();
        }

    }
}
