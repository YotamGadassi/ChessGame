using ChessGame;
using Client.Game;
using Common;

namespace Frameworks
{
    public class OfflineFramework
    {
        public  OfflineGameViewModel ViewModel { get; }
        private IGameManager         m_gameManager;

        public OfflineFramework(Team northTeam
                              , Team southTeam)
        {
            m_gameManager = new OfflineGameManager();
            ViewModel     = new OfflineGameViewModel(m_gameManager, northTeam, southTeam);
            m_gameManager.StartGame();
        }
    }
}
