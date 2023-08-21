using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Client;
using Common;
using Common.MainWindow;
using Frameworks;
using Frameworks.ChessGame;
using FrontCommon;
using log4net;

namespace Host
{
    public class MainWindowViewModel : BaseMainWindowViewModel
    {
        private static readonly ILog     s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public                  ICommand PlayOfflineCommand { get; }

        private readonly Dispatcher m_dispatcher;

        public MainWindowViewModel()
        {
            m_dispatcher       = Dispatcher.CurrentDispatcher;
            PlayOfflineCommand = new WpfCommand(playOfflineCommandExecute, playOfflineCommandCanExecute);
        }

        private void resetViewModel()
        {
            s_log.Info("Resetting View model");
            CurrentViewModel = null;
        }

        private void playOfflineCommandExecute(object parameter)
        {
            BaseGamePanel offlineGame = getOfflineGame();
            CurrentViewModel      =  offlineGame.GameViewModel;
            offlineGame.GameEnded += onGameEnd;
        }

        private bool playOfflineCommandCanExecute(object parameter)
        {
            return CurrentViewModel == null;
        }

        private void onGameEnd()
        {
            m_dispatcher.Invoke(resetViewModel);
        }

        private BaseGamePanel getOfflineGame()
        {
            return new OfflineChessGamePanel("OfflineGame");
        }
    }
}