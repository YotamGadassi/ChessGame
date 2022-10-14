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
    public class OnlineFramework
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private                 HubConnection   s_connection;
        private                 BaseGameManager s_gameManager;
        private                 Team            s_localMachineTeam;
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

        public void Init()
        {

        }

        public void RequestGameFromServer()
        {

        }
        private async void GameManagerOnToolMovedEvent(object sender, ToolMovedEventArgs e)
        {
            bool isMovedFromServer = e.MovedTool.Color != s_localMachineTeam.Color;
            if (isMovedFromServer)
            {
                return;
            }

            try
            {
                await s_connection.InvokeAsync("Move", e.InitialPosition, e.EndPosition);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }    
        }

        private Team resolveTeam(MessageBoxResult result)
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

        private void connectToHub()
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

        private void registerClientMethods()
        {
            s_connection.On<BoardPosition, BoardPosition>("Move", (start, end) =>
                                                                  {
                                                                      Console.WriteLine($"A move request received from server: [start:{start}], [end:{end}]");
                                                                      Task task = Task.Run(() => s_gameManager.Move(start,end));
                                                                      return task;
                                                                  });
            // should add methods for StartGame and PlayerQuit
            s_connection.On("StartGame", ()=> )
        }


    }
}
