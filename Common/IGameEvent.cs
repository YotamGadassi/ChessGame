namespace Common;

public interface IGameEvents
{
    public event AskPromotionEventHandler? AskPromotionEvent;

    public event CheckMateEventHandler? CheckMateEvent;

}