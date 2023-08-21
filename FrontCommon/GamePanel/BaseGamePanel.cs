using System.Windows;
using System.Windows.Controls;

namespace FrontCommon;

public abstract class BaseGamePanel
{
    public event Action              GameEnded;
    public string           PanelName     { get; }
    public abstract DependencyObject GameViewModel { get; }
    public abstract Control          GameControl   { get; }

    protected BaseGamePanel(string panelName)
    {
        PanelName = panelName;
    }

    protected void onGameEnd() => GameEnded?.Invoke();
}