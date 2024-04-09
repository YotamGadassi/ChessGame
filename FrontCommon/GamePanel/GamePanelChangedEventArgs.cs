using FrontCommon.GamePanel;

namespace FrontCommon;

public class GamePanelChangedEventArgs : EventArgs
{
    public string OldPanel { get; }
    public IGamePanel NewPanel { get; }

    public GamePanelChangedEventArgs(string oldPanelName
                                   , IGamePanel newPanel)
    {
        OldPanel = oldPanelName;
        NewPanel = newPanel;
    }
}