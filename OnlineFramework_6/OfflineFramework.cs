using System.Windows;
using ChessGame;
using Client.Game;
using Common;

namespace Frameworks
{
    public class OfflineFramework
    {
        public  OfflineGameViewModel      ViewModel { get; }
        private BaseGameManager           m_gameManager;
        public event EventHandler<Window> OnUserMessage;
        public OfflineFramework(Team northTeam, Team southTeam)
        {
            m_gameManager  = new OfflineGameManager();
            ViewModel      = new OfflineGameViewModel(m_gameManager, northTeam, southTeam);
            m_gameManager.StartGame();
        }

        private void onUserMessage(Window win)
        {
            OnUserMessage?.Invoke(this, win);
        }
    }
}
