using FrontCommon.GamePanel;

namespace FrontCommon;

public class GamePanelChangedEventArgs : EventArgs
{
    public string OldPanel { get; }
    public BaseGamePanel NewPanel { get; }

    public GamePanelChangedEventArgs(string oldPanelName
                                   , BaseGamePanel newPanel)
    {
        OldPanel = oldPanelName;
        NewPanel = newPanel;
    }
}