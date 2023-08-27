using System.Reflection;
using Board;
using Common;
using Common.Chess;
using log4net;
using Tools;

namespace ChessGame
{
    public class OfflineChessGameManager : IChessGameManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IBoardEvents         BoardEvents         => m_gameBoard;
        public IChessBoardProxy     ChessBoardProxy     { get; }
        public ITeamsManager        TeamsManager        { get; }
        public TeamWithTimer        CurrentTeamTurn     => TeamsManager.CurrentTeamTurn;
        public TeamWithTimer[]      Teams               => TeamsManager.Teams;
        public IGameStateController GameStateController { get; }

        private ChessBoard m_gameBoard;

        public OfflineChessGameManager(OfflineTeamsManager teamsManager)
        {
            TeamsManager                     =  teamsManager;
            GameStateController              =  new GameStateController();
            GameStateController.StateChanged += onStateChanged;
            m_gameBoard                      =  new ChessBoard();
            ChessBoardProxy                  =  new ChessBoardProxy(m_gameBoard, teamsManager);
        }

        private void onStateChanged(object?   sender
                                  , GameState currState)
        {
            switch (currState)
            {
                case GameState.Running:
                {
                    s_log.Info($"Game Started");
                    CurrentTeamTurn.StartTimer();
                }
                    break;
                case GameState.Paused:
                {
                    s_log.Info($"Game Paused");
                    CurrentTeamTurn.StopTimer();
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

        public void Init()
        {
            s_log.Info("Init Game");

            TeamWithTimer team1 = Teams[0];
            TeamWithTimer team2 = Teams[1];

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
    }
}