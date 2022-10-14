using System.Reflection;
using System.Windows;
using System.Windows.Media;
using ChessGame;
using Client.Board;
using Common_6;
using Common_6.ChessBoardEventArgs;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using OnlineFramework.MainOnlineWindow;
using MainWindowControl = OnlineFramework.MainOnlineWindow.MainWindowControl;

namespace OnlineFramework
{
    internal class Program
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static          HubConnection   s_connection;
        private static          BaseGameManager s_gameManager;
        private static          Team            s_localMachineTeam;
        private static readonly string          s_hubAddress = @"https://localhost:7034/ChessHub";
        
        [STAThread]
        static void Main(string[] args)
        {
            MainWindowControl   mainWindow    = new MainWindowControl();
            MainWindowViewModel mainWindowsVm = new MainWindowViewModel();

            MessageBoxResult result = MessageBox.Show("Click OK for white, Cancel for Black","color",MessageBoxButton.OKCancel);

            s_localMachineTeam = resolveTeam(result);

            s_connection = new HubConnectionBuilder().WithUrl(s_hubAddress).Build();
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
                int[] start = new[] { e.InitialPosition.Row, e.InitialPosition.Column };
                int[] end = new[] { e.EndPosition.Row, e.EndPosition.Column };

                await s_connection.InvokeAsync("Move", start, end);
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
            s_connection.Closed += (error) => new Task(() =>  Console.WriteLine($"connection closed: {error}"));
            registerClientMethods();
            try
            {
                s_connection.StartAsync().ContinueWith((task) =>  Console.WriteLine($"connection to client completed. server state:{s_connection.State}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void registerClientMethods()
        {
            s_connection.On<int[], int[]>("Move", (start, end) =>
            {
                Console.WriteLine($"A move request received from server: [start:{start}], [end:{end}]");
                Task task = Task.Run(() => s_gameManager.Move(new BoardPosition(start[1], start[0]), new BoardPosition( end[1],end[0])));
                return task;
            });
        }
    }
}
