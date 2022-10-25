using System;
using System.Reflection;
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



        private readonly Dispatcher m_dispatcher;

        public MainWindowViewModel()
        {
            m_dispatcher       = Dispatcher.CurrentDispatcher;
            PlayOnlineCommand  = new WpfCommand(playOnlineCommandExecute, playOnlineCommandCanExecute);
            PlayOfflineCommand = new WpfCommand(playOfflineCommandExecute);
        }

        private void playOnlineCommandExecute(object parameter)
        {
            s_log.Info("Play online command invoked");
            OnlineFramework onlineFramework = new();
            onlineFramework.OnGameStarted += onGameStart;
            onlineFramework.OnGameEnd     += onGameEnd;
            onlineFramework.ConnectToHubAsync("Yotam");
            onlineFramework.AsyncRequestGameFromServer();
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
            m_dispatcher.Invoke(()=>CurrentViewModel = null);
        }

        private void onGameStart(object sender, BaseGameViewModel gameViewModel)
        {
            m_dispatcher.Invoke(()=>CurrentViewModel = gameViewModel);
        }

        private void onMessageShow(Window win)
        {
            win.ShowDialog();
        }
    }
}
