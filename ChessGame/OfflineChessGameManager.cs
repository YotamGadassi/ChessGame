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
    public class OfflineChessGameManager : BaseChessGameManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IAvailableMovesHelper          AvailableMovesHelper { get; }

        public OfflineChessGameManager(TeamWithTimer team1
                                     , TeamWithTimer team2) 
            : base(new ChessBoard(), s_log)
        {
            AvailableMovesHelper = new AvailableMovesHelper(this);
            setTeams(new TeamWithTimer[]{team1, team2});
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

        public override bool StartResumeGame()
        {
            base.StartResumeGame();
            CurrentTeamTurn.StartTimer();
            return false;
        }

        public override bool PauseGame()
        {
            base.PauseGame();
            CurrentTeamTurn.StopTimer();
            return true;
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

        public BoardState GetBoardState()
        {
            return m_gameBoard.GetBoard;
        }
    }
}