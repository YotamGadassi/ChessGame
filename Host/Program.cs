using System;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Common.MainWindow;
using Frameworks;
using Frameworks.ChessGame;
using FrontCommon;
using FrontCommon.Facade;
using log4net;
using log4net.Config;
using OnlineChess.GamePanel;

namespace Host
{
    public class Program
    {
        private static ILog s_log;

        [STAThread]
        private static void Main(string[] args)
        {
            setUpLog();
            s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            setUpUnhandledExceptions();
            s_log.Info("App started");
            initFacade();
            MainWindowControl       mainWinControl = new();
            BaseMainWindowViewModel mainWinVm      = BaseGameFacade.Instance.MainWindowViewModel;
            mainWinControl.DataContext = mainWinVm;
            mainWinControl.ShowDialog();
        }

        private static void initFacade()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(GameFacade).TypeHandle);
            s_log.Info("Facade Initialized");

            GameFacade gameFacade = (GameFacade)GameFacade.Instance;

            IGamePanelManager panelManager = createGamePanelManager();

            MainWindowViewModel mainWindowViewModel = new();
            initGameButtons(mainWindowViewModel, panelManager);
            gameFacade.SetGamePanelManager(panelManager);
            gameFacade.SetMainWindoesViewModel(mainWindowViewModel);
        }

        private static void initGameButtons(MainWindowViewModel mainWindowViewModel
                                          , IGamePanelManager   panelManager)
        {
            OfflineGameButton offlineGameButton = new OfflineGameButton(Dispatcher.CurrentDispatcher, panelManager);
            mainWindowViewModel.AddGameButton(offlineGameButton);

            OnlineGameButton onlineGameButton = createOnlineGameButton(panelManager);
            mainWindowViewModel.AddGameButton(onlineGameButton);
        }

        private static OnlineGameButton createOnlineGameButton(IGamePanelManager panelManager)
        {
            IChessConnectionManager   connectionManager  = new SignalRConnectionManager(); //TODO: use Factory
            OnlineGameRequestManager gameRequestManager = new(connectionManager);
            OnlineGameButton         gameButton         = new(Dispatcher.CurrentDispatcher, panelManager, gameRequestManager);
            return gameButton;
        }

        private static IGamePanelManager createGamePanelManager()
        {
            GamePanelManager      panelManager          = new();
            string                panelName             = "OfflineChessGame";
            OfflineChessGamePanel offlineChessGamePanel = new(panelName);
            panelManager.Add(panelName, offlineChessGamePanel);

            panelName = "OnlineChessGame";
            OnlineChessGamePanel onlineChessGamePanel = new(panelName);
            panelManager.Add(panelName, onlineChessGamePanel);

            return panelManager;
        }

        private static void setUpUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
        }

        private static void onUnhandledException(object                      sender
                                               , UnhandledExceptionEventArgs e)
        {
            Exception ex  = (e.ExceptionObject as Exception);
            string    log = $"{ex.Message}\n {ex.StackTrace}";
            s_log.Error(log);
            throw ex;
        }

        private static void setUpLog()
        {
            Stream? stream = Assembly.GetCallingAssembly().GetManifestResourceStream("Host.LogConfiguration.xml");
            XmlConfigurator.Configure(stream);
        }
    }
}