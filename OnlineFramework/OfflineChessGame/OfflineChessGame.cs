using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessGame;
using Client.Game;
using Common;
using Common.Chess;
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
            OfflineTeamsManager teamsManager = createOfflineTeamsManager();

            GameManager              =  new OfflineChessGameManager(teamsManager);
            GameManager.GameStateController.StateChanged += onGameStateChanged;
            m_gameViewModel          =  new OfflineChessGameViewModel(GameManager);
            GameControl.DataContext  =  null;
            GameControl.DataContext  =  m_gameViewModel;
            GameManager.Init();
        }

        public override void Reset()
        {
            GameManager.GameStateController.StateChanged -= onGameStateChanged;
            GameManager              =  null;
            m_gameViewModel          =  null;
            m_gameControl            =  new GameControl();
        }

        public override void Dispose()
        {
            GameManager.GameStateController.StateChanged -= onGameStateChanged;
        }

        private void onGameStateChanged(object?   sender
                                      , GameState e)
        {
            if (e == GameState.Ended)
            {
                onGameEnd();
                Reset();
            }
        }

        private OfflineTeamsManager createOfflineTeamsManager()
        {
            ChessTeam team1 = new("White", Colors.White, GameDirection.North
                                , new TeamTimer(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1)));

            ChessTeam team2 = new("Black", Colors.Black, GameDirection.South
                                , new TeamTimer(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1)));

            OfflineTeamsManager teamsManager = new(new[] { team1, team2 });
            return teamsManager;
        }

    }
}