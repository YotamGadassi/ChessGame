using Common;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window m_hostWin;

        private IPlayer seconderyPlayer;
        private IPlayer mainPlayer;

        public MainWindow()
        {
            InitializeComponent();
            m_hostWin = new HostGameWin();
        }

        private void ButtonClick_CreateUser(object sender, RoutedEventArgs e)
        {
            mainPlayer = new Player("Main Player");
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
            seconderyPlayer = new Player("Second Player");
            m_board.Visibility = Visibility.Visible;
        }
    }
}
