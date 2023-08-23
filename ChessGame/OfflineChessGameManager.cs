using System.Reflection;
using Board;
using ChessGame.Helpers;
using Common;
using Common.Chess;
using Common.GameInterfaces;
using log4net;
using Tools;

namespace ChessGame
{
    public class OfflineChessGameManager : IChessGameManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler<GameState>? StateChanged;
        public IBoardEvents                   BoardEvents     => m_gameBoard;
        public TeamWithTimer                  CurrentTeamTurn => Teams[m_currentTeamIndex];
        public TeamWithTimer[]                Teams           { get; private set; }
        public GameState                      State                { get; private set; }
        public IAvailableMovesHelper          AvailableMovesHelper { get; }

        private                 ChessBoard m_gameBoard;
        private                 int        m_currentTeamIndex;
        private static readonly int        s_teamsAmount = 2;

        public OfflineChessGameManager(TeamWithTimer team1
                                     , TeamWithTimer team2)
        {
            Teams                = new[] { team1, team2 };
            State                = GameState.NotStarted;
            m_gameBoard          = new ChessBoard();
            AvailableMovesHelper = new AvailableMovesHelper(this);
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

            m_currentTeamIndex = 0;
        }

        public void StartResumeGame()
        {
            s_log.Info($"Game Started");
            CurrentTeamTurn.StartTimer();
            setState(GameState.Running);
        }

        public void PauseGame()
        {
            s_log.Info($"Game Paused");
            CurrentTeamTurn.StopTimer();
            setState(GameState.Paused);
        }

        public void EndGame()
        {
            s_log.Info("End Game");

            m_gameBoard.Clear();
            m_currentTeamIndex = 0;
            setState(GameState.Ended);
        }

        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            s_log.Info($"Move - Start:{start} | End:{end}");

            MoveResult     result     = m_gameBoard.Move(start, end);
            MoveResultEnum resultEnum = result.Result;

            if (resultEnum.HasFlag(MoveResultEnum.ToolMoved))
            {
                switchCurrentTeam();
            }

            if ((resultEnum & (MoveResultEnum.CheckMate | MoveResultEnum.NeedPromotion)) != 0)
            {
                s_log.Info($"{resultEnum} occurred after move from {start} to {end}");
                return result;
            }

            return result;
        }

        public PromotionResult Promote(BoardPosition position
                                     , ITool         promotedTool)
        {
            s_log.Info($"Promote: Position:{position} | Promoted Tool:{promotedTool}");
            PromotionResult promotionResult = m_gameBoard.Promote(position, promotedTool);
            // if (promotionResult.Result == PromotionResultEnum.PromotionSucceeded)
            // {
            //     switchCurrentTeam();
            // }

            return promotionResult;
        }

        public bool TryGetTool(BoardPosition position
                             , out ITool     tool)
        {
            return m_gameBoard.TryGetTool(position, out tool);
        }

        public BoardState GetBoardState()
        {
            return m_gameBoard.GetBoard;
        }

        protected void switchCurrentTeam()
        {
            CurrentTeamTurn.StopTimer();
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
            CurrentTeamTurn.StartTimer();
        }

        private void setState(GameState state)
        {
            if (state == State)
                return;

            State = state;
            StateChanged?.Invoke(this, state);
            
        }
    }
}