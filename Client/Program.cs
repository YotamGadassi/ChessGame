using System;
using System.Windows.Media;
using ChessGame;
using Client.Board;
using Client.MainWindow;
using Common;

namespace Client
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MainWindowControl   mainWindow    = new MainWindowControl();
            MainWindowViewModel mainWindowsVm = new MainWindowViewModel();

            GameManager gameManager = new GameManager();
            gameManager.StartGame(new Team("A", Colors.White, GameDirection.North), new Team("B", Colors.Black, GameDirection.South));
            
            BoardPanel  panel       = new BoardPanel(gameManager);
            panel.Init();
            mainWindowsVm.CurrentView = panel.Control;
            mainWindow.DataContext    = mainWindowsVm;
            mainWindow.ShowDialog();
        }
    }
}
