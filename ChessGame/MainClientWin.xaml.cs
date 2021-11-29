using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window m_hostWin;
        private OfflineFramwork Framework = null;


        public MainWindow()
        {
            InitializeComponent();
            m_hostWin = new HostGameWin();
        }

        private void ButtonClick_CreateUser(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClick_HostGame(object sender, RoutedEventArgs e)
        {
            m_hostWin.Show();
        }

        private void ButtonClick_ConnectToHost(object sender, RoutedEventArgs e)
        {

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
