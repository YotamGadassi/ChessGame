using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Client;
using Common;
using Common.MainWindow;
using Frameworks;
using log4net;

namespace Host
{
    public class MainWindowViewModel : BaseMainWindowViewModel
    {
        private static readonly ILog     s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public                  ICommand PlayOnlineCommand  { get; }
        public                  ICommand PlayOfflineCommand { get; }
        
        private OnlineFramework m_onlineFramework;

        private readonly Dispatcher m_dispatcher;

        public MainWindowViewModel()
        {
            m_dispatcher                    =  Dispatcher.CurrentDispatcher;
            PlayOnlineCommand               =  new WpfCommand(playOnlineCommandExecute,  playOnlineCommandCanExecute);
            PlayOfflineCommand              =  new WpfCommand(playOfflineCommandExecute, playOfflineCommandCanExecute);
            m_onlineFramework               =  new();
            m_onlineFramework.OnGameEnd     += onGameEnd;
        }

        private async void playOnlineCommandExecute(object parameter)
        {
            CurrentViewModel = m_onlineFramework.ViewModel;
            s_log.Info("Play online command invoked");
            bool isConnected = await m_onlineFramework.ConnectToServerAsync("Yotam");
            if (false == isConnected)
            {
                s_log.Warn($"Could not connect to server");
                resetViewModel();
                return;
            }
            bool isRequestApproved = await m_onlineFramework.AsyncRequestGameFromServer();
            if (false == isRequestApproved)
            {
                s_log.WarnFormat($"Request game is not approved by the server");
                resetViewModel();
                return;
            }

            s_log.Info($"Request game approved by the server, waiting for the server to start the game");
        }

        private void resetViewModel()
        {
            s_log.Info("Resetting View model");
            CurrentViewModel = null;
        }

        private bool playOnlineCommandCanExecute(object parameter)
        {
            return CurrentViewModel == null;
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
    }
}
