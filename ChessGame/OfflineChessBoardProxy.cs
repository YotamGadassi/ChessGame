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

    private ChessBoard          m_chessBoard;
    private OfflineTeamsManager m_teamsManager;

    public OfflineChessBoardProxy(ChessBoard chessBoard, OfflineTeamsManager teamsManager)
    {
        m_chessBoard = chessBoard;
        m_teamsManager = teamsManager;
    }

    public MoveResult Move(BoardPosition start
                         , BoardPosition end)
    {
        s_log.Info($"Move - Start:{start} | End:{end}");

        MoveResult     result     = m_chessBoard.Move(start, end);
        MoveResultEnum resultEnum = result.Result;

        if (resultEnum.HasFlag(MoveResultEnum.ToolMoved))
        {
            m_teamsManager.SwitchCurrentTeam();
        }

        if ((resultEnum & (MoveResultEnum.CheckMate | MoveResultEnum.NeedPromotion)) != 0)
        {
            s_log.Info($"{resultEnum} occurred after move from {start} to {end}");
            return result;
        }

        return result;
    }

    public PromotionResult Promote(BoardPosition position
                                 , ITool         mewTool)
    {
        s_log.Info($"Promote: Position:{position} | New Tool:{mewTool}");
        PromotionResult promotionResult = m_chessBoard.Promote(position, mewTool);
        if (promotionResult.Result == PromotionResultEnum.PromotionSucceeded)
        {
            TeamId teamId = m_teamsManager.GetTeamId(promotionResult.PromotedTool.ToolId);
            m_teamsManager.AddToolId(teamId, promotionResult.NewTool.ToolId);
        }
        return promotionResult;
    }

    public bool TryGetTool(BoardPosition position
                         , out ITool     tool)
    {
        return m_chessBoard.TryGetTool(position, out tool);
    }

}