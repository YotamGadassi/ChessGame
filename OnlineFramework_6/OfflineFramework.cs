using System.Windows.Media;
using ChessGame;
using Client.Board;
using Common_6;

namespace Frameworks
{
    public class OfflineFramework
    {
        public  BaseBoardPanel  BoardPanel { get; }
        private BaseGameManager m_gameManager;
        public OfflineFramework()
        {
            m_gameManager = new OfflineGameManager();
            BoardPanel    = new OfflineBoardPanel(m_gameManager);
            m_gameManager.StartGame();
            BoardPanel.Init();
        }
    }
}
