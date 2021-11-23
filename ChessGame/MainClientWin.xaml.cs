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

        private IPlayer m_seconderyPlayer;
        private IPlayer m_mainPlayer;
        private IMovementController m_mainPlayerController;
        private IMovementController m_secondaryPlayerController;


        public MainWindow()
        {
            InitializeComponent();
            m_hostWin = new HostGameWin();
        }

        private void ButtonClick_CreateUser(object sender, RoutedEventArgs e)
        {
            m_mainPlayer = new Player("Main Player");
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
            m_seconderyPlayer = new Player("Second Player");
            ChessGameServer.instance.Init();
            ChessGameServer.instance.RegisterPlayer(m_mainPlayer, Team.White);
            m_mainPlayerController = ChessGameServer.instance.GetController(m_mainPlayer);

            ChessGameServer.instance.RegisterPlayer(m_seconderyPlayer, Team.Black);
            m_secondaryPlayerController = ChessGameServer.instance.GetController(m_seconderyPlayer);
            
            m_offlineGameGrid.Visibility = Visibility.Visible;
        }

        private void ButtonClick_StartGame(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
