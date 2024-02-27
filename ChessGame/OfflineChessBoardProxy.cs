using Board;
using Common;
using Common.Chess;
using log4net;
using Tools;

namespace ChessGame;

public interface IChessBoardProxy
{
    public MoveResult Move(BoardPosition start
                         , BoardPosition end);

    PromotionResult Promote(BoardPosition start
                          , ITool         newTool);

    bool TryGetTool(BoardPosition position
                  , out ITool     tool);
}

public class OfflineChessBoardProxy : IChessBoardProxy
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OfflineChessBoardProxy));

    private readonly ChessBoard          m_chessBoard;
    private readonly OfflineTeamsManager m_teamsManager;
    private          bool                m_isWaitingForPromotion;

    public OfflineChessBoardProxy(ChessBoard chessBoard, OfflineTeamsManager teamsManager)
    {
        m_isWaitingForPromotion = false;
        m_chessBoard            = chessBoard;
        m_teamsManager          = teamsManager;
    }

    public MoveResult Move(BoardPosition start
                         , BoardPosition end)
    {
        s_log.Info($"Move - Start:{start} | End:{end}");
        if (m_isWaitingForPromotion)
        {
            s_log.Warn($"Game is waiting for promotion, Can't move tool");
            return MoveResult.NoChangeOccurredResult;
        }

        MoveResult     result     = m_chessBoard.Move(start, end);
        MoveResultEnum resultEnum = result.Result;

        bool isPromotionOrCheckMate = resultEnum.HasFlag(MoveResultEnum.NeedPromotion) &
                                      resultEnum.HasFlag(MoveResultEnum.CheckMate);

        if (resultEnum.HasFlag(MoveResultEnum.ToolMoved)
         && false == isPromotionOrCheckMate)
        {
            m_teamsManager.SwitchCurrentTeam();
        }

        if (resultEnum.HasFlag(MoveResultEnum.NeedPromotion))
        {
            m_isWaitingForPromotion = true;
        }

        if (isPromotionOrCheckMate)
        {
            s_log.Info($"{resultEnum} occurred after move from {start} to {end}");
        }

        return result;
    }

    public PromotionResult Promote(BoardPosition position
                                 , ITool         mewTool)
    {
        s_log.Info($"Promote: Position:{position} | New Tool:{mewTool}");
        if (false == m_isWaitingForPromotion)
        {
            s_log.Warn($"Game is not waiting for promotion, Can't promote");
            return PromotionResult.NoPromotionOccured;
        }

        PromotionResult promotionResult = m_chessBoard.Promote(position, mewTool);
        if (promotionResult.Result == PromotionResultEnum.PromotionSucceeded)
        {
            TeamId teamId = m_teamsManager.GetTeamId(promotionResult.PromotedTool.ToolId);
            m_teamsManager.AddToolId(teamId, promotionResult.NewTool.ToolId);
            m_teamsManager.SwitchCurrentTeam();
            m_isWaitingForPromotion = false;
        }
        return promotionResult;
    }

    public bool TryGetTool(BoardPosition position
                         , out ITool     tool)
    {
        return m_chessBoard.TryGetTool(position, out tool);
    }

}