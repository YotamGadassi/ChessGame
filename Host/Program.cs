using System;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Common.MainWindow;
using Frameworks.ChessGame;
using FrontCommon;
using FrontCommon.Facade;
using log4net;
using log4net.Config;

namespace Host
{
    public class Program
    {
        private static ILog s_log;

        [STAThread]
        private static void Main(string[] args)
        {
            setUpLog();
            setUpUnhandledExceptions();
            s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            s_log.Info("App started");
            initFacade();
            MainWindowControl       mainWinControl = new MainWindowControl();
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
        }

        private static IGamePanelManager createGamePanelManager()
        {
            GamePanelManager      panelManager          = new();
            string                panelName             = "OfflineChessGame";
            OfflineChessGamePanel offlineChessGamePanel = new OfflineChessGamePanel(panelName);
            panelManager.Add(panelName, offlineChessGamePanel);

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