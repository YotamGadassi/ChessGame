using System.Windows;
using System.Windows.Controls;

namespace FrontCommon;

public abstract class BaseGamePanel : IDisposable
{
    public event Action<BaseGamePanel>              GameEnded;
    public          string           PanelName     { get; }
    public abstract DependencyObject GameViewModel { get; }
    public abstract Control          GameControl   { get; }

    public abstract void Init();

    public abstract void Reset();
    public abstract void Dispose();

    protected BaseGamePanel(string panelName)
    {
        PanelName = panelName;
    }

    protected void onGameEnd() => GameEnded?.Invoke(this);
}