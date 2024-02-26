using Board;
using Tools;

namespace Common;

public delegate void AskPromotionEventHandler(BoardPosition position, ITool toolToPromote);

public delegate void CheckMateEventHandler(BoardPosition position
                                         , ITool         toolToPromote);

public interface IChessGameEvents
{
    public event AskPromotionEventHandler AskPromotionEvent;
    public event CheckMateEventHandler    CheckMateEvent;
}