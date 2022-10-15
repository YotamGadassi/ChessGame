using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;
using ChessGame;
using Client.Board;
using Common_6;
using Common_6.ChessBoardEventArgs;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.SignalR.Client;
using OnlineFramework.MainOnlineWindow;
using MainWindowControl = OnlineFramework.MainOnlineWindow.MainWindowControl;

namespace OnlineFramework
{
    public class OnlineFramework
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static          MainWindowViewModel m_mainWindowVm;
        private                 HubConnection       m_connection;
        private                 BaseGameManager     m_gameManager;
        private                 Team                m_localMachineTeam;
        private                 Dispatcher          m_dispatcher;
        private static readonly string              s_hubAddress = @"https://localhost:7034/ChessHub";
        
        public OnlineFramework (MainWindowControl winControl, MainWindowViewModel winVM)
        {
            m_dispatcher   = Dispatcher.CurrentDispatcher;;
            m_mainWindowVm = winVM;
            
            m_connection = new HubConnectionBuilder().WithUrl(s_hubAddress).Build();
        }

        public void Init()
        {
            setUpLog();
        }

        private void setUpLog()
        {
            XmlConfigurator.Configure(new FileInfo("LogConfiguration.xml"));
        }

        public void RequestGameFromServer()
        {
            connectToHubAsync();
        }

        private async void GameManagerOnToolMovedEvent(object sender, ToolMovedEventArgs e)
        {
            bool isMovedFromServer = e.MovedTool.Color != m_localMachineTeam.Color;
            if (isMovedFromServer)
            {
                return;
            }

            try
            {
                s_log.Info($"Local move event has been invoked");
                await m_connection.InvokeAsync("Move", e.InitialPosition, e.EndPosition);
                s_log.Info($"Move invocation has been sent to server: [start:{e.InitialPosition}] [end:{e.EndPosition}]");
            }
            catch (Exception exception)
            {
                s_log.Error(exception);
            }    
        }

        private Team resolveTeam(Color color)
        {
            if (color == Colors.White)
            {
                return new Team("White_A", Colors.White, GameDirection.North);
            }
            else
            {
                return new Team("Black_B", Colors.Black, GameDirection.South);
            }
        }

        private async void connectToHubAsync()
        {
            m_connection.Closed += (error) => new Task(() =>  Console.WriteLine($"connection closed: {error}"));
            registerClientMethods();
            try
            {
                s_log.Info($"Starting connection to client. server state:{m_connection.State}");
                await m_connection.StartAsync();
                s_log.Info($"connection to client completed. server state:{m_connection.State}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void registerClientMethods()
        {
            m_connection.On<BoardPosition, BoardPosition>("Move", (start, end) =>
                                                                  {
                                                                      Console.WriteLine($"A move request received from server: [start:{start}], [end:{end}]");
                                                                      // TODO: fix thread safe issue!
                                                                      Task task = Task.Run(() => m_gameManager.Move(start,end));
                                                                      return task;
                                                                  });
            m_connection.On<Color>("StartGame", (color) => m_dispatcher.InvokeAsync(() => startGame(color)));
            m_connection.On("PlayerQuit", endGame);
        }

        private void startGame(Color otherTeamColor)
        {
            s_log.Info("Stating Game");

            Color currentTeamColor = otherTeamColor == Colors.Black ? Colors.White : Colors.Black;

            m_localMachineTeam = resolveTeam(currentTeamColor);
            Team remoteTeam = resolveTeam(otherTeamColor);

            OnlineGameManager gameManager = new OnlineGameManager(m_localMachineTeam);
            m_gameManager = gameManager;
            if (m_localMachineTeam.Color == Colors.White)
            {
                gameManager.StartGame(m_localMachineTeam, remoteTeam);
            }
            else
            {
                gameManager.StartGame(remoteTeam, m_localMachineTeam);
            }

            m_gameManager.ToolMovedEvent  += GameManagerOnToolMovedEvent;
            m_gameManager.ToolKilledEvent += GameManagerOnToolMovedEvent;
            BaseBoardPanel panel = new OnlineBoardPanel(gameManager);
            panel.Init();
            m_mainWindowVm.CurrentViewModel = panel.BoardVm;
            s_log.Info("Game Started");

        }

        private void endGame()
        {
            s_log.Info("Game ended");
            m_gameManager.EndGame();
            m_gameManager              = null;
            m_dispatcher.InvokeAsync(()=>m_mainWindowVm.CurrentViewModel = null);
            unregisterClientMethods();
        }

        private void unregisterClientMethods()
        {
            m_connection.Remove("Move");
            m_connection.Remove("PlayerQuit");
            m_connection.Remove("StartGame");
        }

        public void DummyStartGame(Color color)
        {
            Thread.Sleep(TimeSpan.FromSeconds(4));
            startGame(color);
        }

    }
}
