using System;
using System.Windows.Input;
using System.Windows.Threading;
using ChessGame;
using Client.Board;
using Client.Command;
using Common.MainWindow;
using Frameworks;

namespace Host
{
    public class MainWindowViewModel : BaseMainWindowViewModel
    {
        public ICommand PlayOnlineCommand { get; }
        public ICommand PlayOfflineCommand { get; }

        private readonly Dispatcher m_dispatcher;

        public MainWindowViewModel()
        {
            m_dispatcher       = Dispatcher.CurrentDispatcher;
            PlayOnlineCommand  = new WpfCommand(playOnlineCommandExecute, playOnlineCommandCanExecute);
            PlayOfflineCommand = new WpfCommand(playOfflineCommandExecute);
        }

        private void playOnlineCommandExecute(object parameter)
        {
            OnlineFramework onlineFramework = new OnlineFramework();
            onlineFramework.OnGameStarted += onGameStart;
            onlineFramework.OnGameEnd     += onGameEnd;
            onlineFramework.RequestGameFromServer();
        }

        private bool playOnlineCommandCanExecute(object parameter)
        {
            return CurrentViewModel == null;
        }

        private void playOfflineCommandExecute(object parameter)
        {
            OfflineFramework framework = new OfflineFramework();
            CurrentViewModel = framework.BoardPanel.BoardVm;
        }

        private bool playOfflineCommandCanExecute(object parameter)
        {
            return CurrentViewModel == null;
        }
        private void onGameEnd(object? sender, EventArgs e)
        {
            m_dispatcher.Invoke(()=>CurrentViewModel = null);
        }

        private void onGameStart(object sender, BaseBoardPanel boardPanel)
        {
            m_dispatcher.Invoke(()=>CurrentViewModel = boardPanel.BoardVm);
        }
    }
}
