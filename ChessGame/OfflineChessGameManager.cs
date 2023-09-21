using System.Reflection;
using Board;
using Common;
using Common.Chess;
using log4net;
using Tools;

namespace ChessGame
{
    public class OfflineChessGameManager : IDisposable
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IBoardEvents         BoardEvents         => m_gameBoard;
        public OfflineChessBoardProxy     ChessBoardProxy     { get; }
        public IChessTeamManager    TeamsManager        => m_teamsManager;
        public IGameStateController GameStateController { get; }

        public  IBoardQuery BoardQuery => m_gameBoard;
        private ChessBoard  m_gameBoard;

        private OfflineTeamsManager m_teamsManager;

        public OfflineChessGameManager(OfflineTeamsManager teamsManager)
        {
            m_teamsManager                   =  teamsManager;
            GameStateController              =  new GameStateController();
            m_gameBoard                      =  new ChessBoard();
            ChessBoardProxy                  =  new OfflineChessBoardProxy(m_gameBoard, teamsManager);
            registerToEvents();
        }

        public void Init()
        {
            s_log.Info("Init Game");

            Team team1 = TeamsManager.Teams[0];
            Team team2 = TeamsManager.Teams[1];

            KeyValuePair<BoardPosition, ITool>[] firstGroupBoardArrangement =
                GameInitHelper.GenerateInitialArrangement(team1.MoveDirection, team1.Color);
            KeyValuePair<BoardPosition, ITool>[] secondkGroupBoardArrangement =
                GameInitHelper.GenerateInitialArrangement(team2.MoveDirection, team2.Color);

            foreach (KeyValuePair<BoardPosition, ITool> pair in firstGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<BoardPosition, ITool> pair in secondkGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }
        }

        public BoardState GetBoardState()
        {
            return m_gameBoard.GetBoard;
        }

        public void Dispose()
        {
            unregisterToEvents();
        }

        private void registerToEvents()
        {
            GameStateController.StateChanged += onStateChanged;
        }

        private void unregisterToEvents()
        {
            GameStateController.StateChanged -= onStateChanged;
        }

        private void onStateChanged(object?   sender
                                  , GameState currState)
        {
            switch (currState)
            {
                case GameState.Running:
                {
                    s_log.Info($"Game Started");
                    m_teamsManager.StartTimer(m_teamsManager.CurrentTeamTurn);
                }
                    break;
                case GameState.Paused:
                {
                    s_log.Info($"Game Paused");
                    m_teamsManager.StopTimer(m_teamsManager.CurrentTeamTurn);
                }
                    break;
                case GameState.Ended:
                {
                    s_log.Info("End Game");
                    m_gameBoard.Clear();
                }
                    break;
                case GameState.NotStarted:
                {
                    s_log.Info($"Game State changed to Game Not Started");
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currState), currState, null);
            }
        }

    }
}