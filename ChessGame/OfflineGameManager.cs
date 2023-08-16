using System.Reflection;
using System.Windows.Media;
using Board;
using Common;
using Common.Chess;
using log4net;
using Tools;

namespace ChessGame
{
    public class OfflineGameManager : IChessGameManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ChessBoard m_gameBoard;
        
        public Color                      CurrentColorTurn => m_teams[m_currentTeamIndex];
        public bool                       IsGameRunning    { get; private set; }

        protected                 Color[]? m_teams = { Colors.White, Colors.Black };
        protected                 int      m_currentTeamIndex;
        protected static readonly int      s_teamsAmount = 2;

        public OfflineGameManager()
        {
            IsGameRunning = false;
            m_gameBoard   = new ChessBoard();
        }

        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            s_log.Info($"Move - Start:{start} | End:{end}");

            MoveResult     result     = m_gameBoard.Move(start, end);
            MoveResultEnum resultEnum = result.Result;

            if ((resultEnum & (MoveResultEnum.CheckMate | MoveResultEnum.NeedPromotion)) != 0)
            {
                s_log.Info($"{resultEnum} occurred after move from {start} to {end}");
                return result;
            }

            if (resultEnum.HasFlag(MoveResultEnum.ToolMoved))
            {
                switchCurrentTeam();
            }

            return result;
        }

        public PromotionResult Promote(BoardPosition position
                                     , ITool         promotedTool)
        {
            s_log.Info($"Promote: Position:{position} | Promoted Tool:{promotedTool}");
            PromotionResult promotionResult = m_gameBoard.Promote(position, promotedTool);
            if (promotionResult.Result == PromotionResultEnum.PromotionSucceeded)
            {
                switchCurrentTeam();
            }

            return promotionResult;
        }

        public void StartGame()
        {
            s_log.Info("Start Game");

            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement =
                GameInitHelper.GenerateInitialArrangement(GameDirection.North, Colors.White);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement =
                GameInitHelper.GenerateInitialArrangement(GameDirection.South, Colors.Black);

            foreach (KeyValuePair<BoardPosition, ITool> pair in whiteGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<BoardPosition, ITool> pair in blackGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }

            m_currentTeamIndex = 0;
            IsGameRunning      = true;
        }

        public void EndGame()
        {
            s_log.Info("End Game");

            IsGameRunning = false;
            m_gameBoard.Clear();
            m_teams            = null;
            m_currentTeamIndex = 0;
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
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
        }
    }
}