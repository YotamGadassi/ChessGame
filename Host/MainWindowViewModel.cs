using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Client;
using Client.Game;
using Common;
using Common.MainWindow;
using Common_6;
using Frameworks;
using log4net;

namespace Host
{
    public class MainWindowViewModel : BaseMainWindowViewModel
    {
        private static readonly ILog     s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public                  ICommand PlayOnlineCommand  { get; }
        public                  ICommand PlayOfflineCommand { get; }
        private                 OnlineFramework m_onlineFramework;
        private                 bool isOnlineCommandExecuting;

        private readonly Dispatcher m_dispatcher;

        public MainWindowViewModel()
        {
            isOnlineCommandExecuting        =  false;
            m_dispatcher                    =  Dispatcher.CurrentDispatcher;
            PlayOnlineCommand               =  new WpfCommand(playOnlineCommandExecute, playOnlineCommandCanExecute);
            PlayOfflineCommand              =  new WpfCommand(playOfflineCommandExecute);
            m_onlineFramework               =  new();
            m_onlineFramework.OnGameStarted += onGameStart;
            m_onlineFramework.OnGameEnd     += onGameEnd;
        }

        private async void playOnlineCommandExecute(object parameter)
        {
            isOnlineCommandExecuting = true;
            s_log.Info("Play online command invoked");
            await m_onlineFramework.ConnectToHubAsync("Yotam");
            bool isRequestApproved = await m_onlineFramework.AsyncRequestGameFromServer();
            if (isRequestApproved)
            {
                s_log.Info($"Request game approved by the server, waiting for the server to start the game");
                return;
            }

            s_log.WarnFormat($"Request game is not approved by the server");
            resetViewModel();
            isOnlineCommandExecuting = false;
        }

        private void resetViewModel()
        {
            s_log.Info("Resetting View model");
            CurrentViewModel = null;
        }

        private bool playOnlineCommandCanExecute(object parameter)
        {
            return false == isOnlineCommandExecuting && CurrentViewModel == null;
        }

        private void playOfflineCommandExecute(object parameter)
        {
            Team northTeam = new("Black Team", Colors.Black, GameDirection.South);
            Team southTeam = new("White Team", Colors.White, GameDirection.North);

            OfflineFramework framework = new(northTeam, southTeam);
            CurrentViewModel = framework.ViewModel;
        }

        private bool playOfflineCommandCanExecute(object parameter)
        {
            return CurrentViewModel == null;
        }
        
        private void onGameEnd(object? sender, EventArgs e)
        {
            m_dispatcher.Invoke(resetViewModel);
        }

        private void onGameStart(object sender, BaseGameViewModel gameViewModel)
        {
            m_dispatcher.Invoke(()=>CurrentViewModel = gameViewModel);
        }

    }
}
