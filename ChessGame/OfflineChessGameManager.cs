using System.Reflection;
using Board;
using Common;
using Common.Chess;
using log4net;
using Tools;

namespace ChessGame
{
    public class OfflineChessGameManager : IChessGameEvents, IChessBoardProxy, IDisposable
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event AskPromotionEventHandler? AskPromotionEvent;
        public event CheckMateEventHandler?    CheckMateEvent;

        public IBoardEvents                    BoardEvents         => m_gameBoard;
        public IChessTeamManager               TeamsManager        => m_teamsManager;
        public IGameStateController            GameStateController { get; }

        public  IBoardQuery            BoardQuery => m_gameBoard;
        private ChessBoard             m_gameBoard;
        private OfflineChessBoardProxy m_chessBoardProxy;

        private OfflineTeamsManager m_teamsManager;

        public OfflineChessGameManager(OfflineTeamsManager teamsManager)
        {
            m_teamsManager      = teamsManager;
            GameStateController = new GameStateController();
            m_gameBoard         = new ChessBoard();
            m_chessBoardProxy   = new OfflineChessBoardProxy(m_gameBoard, teamsManager);
            registerToEvents();
        }

        public void Init()
        {
            s_log.Info("Init Game");

            Team team1 = TeamsManager.Teams[0];
            Team team2 = TeamsManager.Teams[1];

            KeyValuePair<BoardPosition, ITool>[] team1BoardArrangement =
                GameInitHelper.GenerateInitialArrangement(team1.MoveDirection, team1.Color);
            KeyValuePair<BoardPosition, ITool>[] team2BoardArrangement =
                GameInitHelper.GenerateInitialArrangement(team2.MoveDirection, team2.Color);

            foreach (KeyValuePair<BoardPosition, ITool> pair in team1BoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
                m_teamsManager.AddToolId(team1.Id, pair.Value.ToolId);
            }

            foreach (KeyValuePair<BoardPosition, ITool> pair in team2BoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
                m_teamsManager.AddToolId(team2.Id, pair.Value.ToolId);
            }
        }

        public BoardState GetBoardState()
        {
            return m_gameBoard.GetBoardState();
        }

        public void Dispose()
        {
            unRegisterFromEvents();
        }

        private void registerToEvents()
        {
            GameStateController.StateChanged += onStateChanged;
        }

        private void unRegisterFromEvents()
        {
            GameStateController.StateChanged -= onStateChanged;
        }

        private void onStateChanged(object?   sender
                                  , GameStateEnum currState)
        {
            switch (currState)
            {
                case GameStateEnum.Running:
                {
                    s_log.Info($"Game Started");
                    m_teamsManager.StartTimer(m_teamsManager.CurrentTeamTurnId);
                }
                    break;
                case GameStateEnum.Paused:
                {
                    s_log.Info($"Game Paused");
                    m_teamsManager.StopTimer(m_teamsManager.CurrentTeamTurnId);
                }
                    break;
                case GameStateEnum.Ended:
                {
                    s_log.Info("End Game");
                    m_gameBoard.Clear();
                }
                    break;
                case GameStateEnum.NotStarted:
                {
                    s_log.Info($"Game State changed to Game Not Started");
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currState), currState, null);
            }
        }

        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            MoveResult result     = m_chessBoardProxy.Move(start, end);
            MoveResultEnum        resultEnum = result.Result;
            if (resultEnum.HasFlag(MoveResultEnum.CheckMate))
            {
                ITool   toolOfWinningTeam = result.ToolAtInitial;
                TeamId? winningTeamId     = m_teamsManager.GetTeamId(toolOfWinningTeam.ToolId);
                CheckMateEvent?.Invoke(new CheckMateData(result.EndPosition, winningTeamId));
                GameStateController.EndGame();
            }
            else if (resultEnum.HasFlag(MoveResultEnum.NeedPromotion))
            {
                AskPromotionEvent?.Invoke(new PromotionRequest(result.ToolAtInitial, result.EndPosition));
            }

            return result;
        }

        public PromotionResult Promote(BoardPosition start
                                     , ITool         newTool) => m_chessBoardProxy.Promote(start, newTool);

        public bool TryGetTool(BoardPosition position
                             , out ITool     tool) => m_chessBoardProxy.TryGetTool(position, out tool);
    }
}