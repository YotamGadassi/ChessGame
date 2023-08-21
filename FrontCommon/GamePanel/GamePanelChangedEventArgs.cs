namespace FrontCommon;

public class GamePanelChangedEventArgs : EventArgs
{
    public BaseGamePanel OldPanel { get; }
    public BaseGamePanel NewPanel { get; }

    public GamePanelChangedEventArgs(BaseGamePanel oldPanel
                                   , BaseGamePanel newPanel)
    {
        OldPanel = oldPanel;
        NewPanel = newPanel;
    }
}