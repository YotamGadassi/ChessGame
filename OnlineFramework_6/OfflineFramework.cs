using System.Windows;
using System.Windows.Interop;
using ChessGame;
using Client.Game;
using Client.Messages;
using Common;
using Common.ChessBoardEventArgs;

namespace Frameworks
{
    public class OfflineFramework
    {
        public  OfflineGameViewModel      ViewModel { get; }
        private BaseGameManager           m_gameManager;
        public OfflineFramework(Team northTeam, Team southTeam)
        {
            m_gameManager  = new OfflineGameManager();
            ViewModel      = new OfflineGameViewModel(m_gameManager, northTeam, southTeam);
            m_gameManager.StartGame();
        }
    }
}
