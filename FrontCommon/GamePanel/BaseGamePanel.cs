using System.Windows;
using System.Windows.Controls;

namespace FrontCommon;

public abstract class BaseGamePanel
{
    public event Action<BaseGamePanel>              GameEnded;
    public          string           PanelName     { get; }
    public abstract DependencyObject GameViewModel { get; }
    public abstract Control          GameControl   { get; }

    public abstract void Init();

    public abstract void Reset();

    protected BaseGamePanel(string panelName)
    {
        PanelName = panelName;
    }

    protected void onGameEnd() => GameEnded?.Invoke(this);
}