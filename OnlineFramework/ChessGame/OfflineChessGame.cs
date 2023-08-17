using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessGame;
using Client.Game;
using Common;
using FrontCommon;

namespace Frameworks.ChessGame
{
    public class OfflineChessGamePanel : BaseGamePanel
    {
        public override DependencyObject GameViewModel => m_gameViewModel;
        public override Control          GameControl   { get; }

        public OfflineChessGameManager GameManager;

        private OfflineChessGameViewModel m_gameViewModel;

        public OfflineChessGamePanel()
        {
            TeamWithTimer team1 =
                new TeamWithTimer("White", Colors.White, GameDirection.North, TimeSpan.FromMinutes(10));

            TeamWithTimer team2 =
                new TeamWithTimer("Black", Colors.Black, GameDirection.South, TimeSpan.FromMinutes(10));
            
            GameManager   = new OfflineChessGameManager(team1, team2);
            m_gameViewModel = new OfflineChessGameViewModel(GameManager);
            GameControl   = new GameControl();

            m_gameViewModel.GameEnded += onGameEnd;

            GameControl.DataContext = GameViewModel;
            GameManager.StartGame();
        }
    }
}