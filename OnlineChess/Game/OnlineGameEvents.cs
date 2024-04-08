using Common;
using OnlineChess.Common;

namespace OnlineChess.Game;

public class OnlineGameEvents : IGameEvents
{
    public event AskPromotionEventHandler? AskPromotionEvent
    {
        add => m_serverAgent.AskPromotionEvent += value;
        remove => m_serverAgent.AskPromotionEvent -= value;
    }

    public event CheckMateEventHandler? CheckMateEvent
    {
        add => m_serverAgent.CheckMateEvent += value;
        remove => m_serverAgent.CheckMateEvent -= value;
    }

    private readonly IChessServerAgent m_serverAgent;

    public OnlineGameEvents(IChessServerAgent server)
    {
        m_serverAgent = server;
    }
}