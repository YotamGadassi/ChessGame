using ChessGame;
using Client.Board;
using Client.MainWindow;
using Common;

namespace OnlineFramework
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MainWindowControl   mainWindow    = new MainWindowControl();
            MainWindowViewModel mainWindowsVm = new MainWindowViewModel();

            BaseGameManager gameManager = new OfflineGameManager();
            gameManager.StartGame(new Team("A", Colors.White, GameDirection.North), new Team("B", Colors.Black, GameDirection.South));

            BaseBoardPanel panel = new OfflineBoardPanel(gameManager);
            panel.Init();
            mainWindowsVm.CurrentView = panel.Control;
            mainWindow.DataContext    = mainWindowsVm;
            mainWindow.ShowDialog();
        }
    }
}