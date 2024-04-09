using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessGame;
using Client.Game;
using Client.Game.GameMainControl;
using Common;
using Common.Chess;
using FrontCommon.GamePanel;
using log4net;

namespace OfflineChess
{
    public class OfflineChessGamePanel : IGamePanel
    {
        private static readonly ILog s_log = LogManager.GetLogger(typeof(OfflineChessGamePanel));

        public event Action<IGamePanel>? GameEnded;
        public string                    PanelName     { get; }
        public DependencyObject          GameViewModel => m_gameViewModel;
        public Control                   GameControl   => m_gameControl;
        public OfflineChessGameManager   GameManager;

        private OfflineChessGameViewModel m_gameViewModel;
        private GameControl               m_gameControl;
        private OfflineTeamsManager       m_teamsManager;

        public OfflineChessGamePanel(string panelName)
        {
            PanelName     = panelName;
            m_gameControl = new GameControl();
        }

        public void Init()
        {
            s_log.Info("Init");
            m_teamsManager = createOfflineTeamsManager();

            GameManager             = new OfflineChessGameManager(m_teamsManager, new OfflineGameEvents());
            m_gameViewModel         = new OfflineChessGameViewModel(GameManager);
            GameControl.DataContext = null;
            GameControl.DataContext = m_gameViewModel;
            GameManager.Init();
            m_gameViewModel.GameEnd += onGameEnd;
        }

        public void Reset()
        {
            disposeResources();
            GameManager     = null;
            m_gameViewModel = null;
            m_gameControl   = new GameControl();
        }

        public void Dispose()
        {
            disposeResources();
        }

        private void disposeResources()
        {
            m_gameViewModel.Dispose();
            GameManager.Dispose();
            m_teamsManager.Dispose();
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

        private void onGameEnd(object?   sender
                             , EventArgs e)
        {
            GameEnded?.Invoke(this);
        }
    }
}