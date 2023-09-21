using Board;
using Common.Chess;
using Tools;

namespace OnlineChess.GamePanel;

public class OnlineChessBoardProxy
{
    private IChessServerAgent m_serverAgent;

    public OnlineChessBoardProxy(IChessServerAgent serverAgent)
    {
        m_serverAgent = serverAgent;
    }

    public Task<MoveResult> Move(BoardPosition start
                               , BoardPosition end)
    {
        return m_serverAgent.RequestMove(start, end);
    }

    public Task<PromotionResult> RequestPromotion(BoardPosition position
                                                , ITool         tool)
    {
        IToolWrapperForServer toolWrapper = new IToolWrapperForServer(tool);
        return m_serverAgent.RequestPromote(position, toolWrapper);
    }

}