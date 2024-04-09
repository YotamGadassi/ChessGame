namespace Common;

public delegate void AskPromotionEventHandler(PromotionRequest promotionRequest);
public delegate void CheckMateEventHandler(CheckMateData checkMateData);

public interface IGameEvents
{
    public event AskPromotionEventHandler? AskPromotionEvent;
    public event CheckMateEventHandler?    CheckMateEvent;
}