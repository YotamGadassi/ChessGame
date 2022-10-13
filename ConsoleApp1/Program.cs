using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ChessBoard.ChessBoardEventArgs;
using ChessGame;
using Client.Board;
using Common;
using Microsoft.AspNetCore.SignalR.Client;
using OnlineFramework.MainOnlineWindow;
using MainWindowControl = OnlineFramework.MainOnlineWindow.MainWindowControl;

namespace OnlineFramework
{
    internal class Program
    {
        private static HubConnection   s_connection;
        private static BaseGameManager s_gameManager;
        private static Team            s_localMachineTeam;
        [STAThread]
        static void Main(string[] args)
        {
            MainWindowControl   mainWindow    = new MainWindowControl();
            MainWindowViewModel mainWindowsVm = new MainWindowViewModel();

            MessageBoxResult result = MessageBox.Show("Click OK for white, Cancel for Black","color",MessageBoxButton.OKCancel);

            s_localMachineTeam = resolveTeam(result);

            s_connection = new HubConnectionBuilder()
                           .WithUrl("https://localhost:7034/ChessHub")
                           .Build();
            
            connectToHub();
            OnlineGameManager gameManager = new OnlineGameManager(s_localMachineTeam);
            s_gameManager = gameManager;
            gameManager.StartGame(new Team("A", Colors.White, GameDirection.North), new Team("B", Colors.Black, GameDirection.South));
            s_gameManager.ToolMovedEvent  += GameManagerOnToolMovedEvent;
            s_gameManager.ToolKilledEvent += GameManagerOnToolMovedEvent;
            BaseBoardPanel panel = new OnlineBoardPanel(gameManager);
            panel.Init();
            mainWindowsVm.CurrentView = panel.Control;
            mainWindow.DataContext    = mainWindowsVm;
            mainWindow.ShowDialog();
        }

        private static async void GameManagerOnToolMovedEvent(object sender, ToolMovedEventArgs e)
        {
            bool isMovedFromServer = e.MovedTool.Color != s_localMachineTeam.Color;
            if (isMovedFromServer)
            {
                return;
            }

            try
            {
                await s_connection.InvokeAsync("Move", e);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }    
        }

        private static Team resolveTeam(MessageBoxResult result)
        {
            if (result == MessageBoxResult.OK)
            {
                return new Team("White_A", Colors.White, GameDirection.North);
            }
            else
            {
                return new Team("Black_B", Colors.Black, GameDirection.South);
            }
        }

        private static void connectToHub()
        {
            registerClientMethods();
            try
            {
                s_connection.StartAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void registerClientMethods()
        {
            s_connection.On<BoardPosition, BoardPosition>("Move", (start, end) =>
            {
                Task task = Task.Run(() => s_gameManager.Move(start, end));
                return task;
            });
        }
    }
}
