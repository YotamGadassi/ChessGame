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
        public override Control          GameControl   => m_gameControl;

        public OfflineChessGameManager GameManager;

        private OfflineChessGameViewModel m_gameViewModel;
        private GameControl m_gameControl;

        public OfflineChessGamePanel(string panelName)
            : base(panelName)
        {
            m_gameControl = new GameControl();
        }

        public override void Init()
        {
            TeamWithTimer team1 = new("White", Colors.White, GameDirection.North, TimeSpan.FromMinutes(10));

            TeamWithTimer team2 = new("Black", Colors.Black, GameDirection.South, TimeSpan.FromMinutes(10));

            GameManager              =  new OfflineChessGameManager(team1, team2);
            GameManager.StateChanged += onGameStateChanged;
            m_gameViewModel          =  new OfflineChessGameViewModel(GameManager);
            GameControl.DataContext  =  null;
            GameControl.DataContext  =  m_gameViewModel;
            GameManager.Init();
        }

        public override void Reset()
        {
            GameManager.StateChanged -= onGameStateChanged;
            GameManager              =  null;
            m_gameViewModel          =  null;
            m_gameControl            =  new GameControl();
        }

        private void onGameStateChanged(object?   sender
                                      , GameState e)
        {
            if (e == GameState.End)
            {
                onGameEnd();
                Reset();
            }
        }
    }
}