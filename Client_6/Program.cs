using System;
using System.Windows.Media;
using ChessGame;
using Client.Board;
using Client.MainOfflineWindow;
using Common_6;

namespace Client
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MainOfflineWindowControl mainWindow    = new MainOfflineWindowControl();
            MainOfflineWindowViewModel mainWindowsVm = new MainOfflineWindowViewModel();

            BaseGameManager gameManager = new OfflineGameManager();
            gameManager.StartGame();
            
            BaseBoardPanel  panel       = new OfflineBoardPanel(gameManager);
            panel.Init();
            mainWindowsVm.CurrentView = panel.Control;
            mainWindow.DataContext    = mainWindowsVm;
            mainWindow.ShowDialog();
        }
    }
}
