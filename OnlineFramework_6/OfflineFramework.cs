using ChessGame;
using Client.Game;
using Common_6;

namespace Frameworks
{
    public class OfflineFramework
    {
        public OfflineGameViewModel ViewModel { get; }
        private BaseGameManager m_gameManager;
        public OfflineFramework(Team northTeam, Team southTeam)
        {
            m_gameManager  = new OfflineGameManager();
            ViewModel      = new OfflineGameViewModel(m_gameManager, northTeam, southTeam);
            m_gameManager.StartGame();
        }
    }
}
