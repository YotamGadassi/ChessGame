using Common;

namespace ChessGame;

public class OfflineGameEvents : IGameEvents
{
    public event AskPromotionEventHandler? AskPromotionEvent;
    public event CheckMateEventHandler?    CheckMateEvent;

    public void RaiseAskPromotionEvent(PromotionRequest request)
    {
        AskPromotionEvent?.Invoke(request);
    }

    public void RaiseCheckMateEvent(CheckMateData data)
    {
        CheckMateEvent?.Invoke(data);
    }
}