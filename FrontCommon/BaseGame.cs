using System.Windows;
using System.Windows.Controls;

namespace FrontCommon;

public abstract class BaseGamePanel
{
    public event Action GameEnded;
    public abstract DependencyObject GameViewModel { get; }
    public abstract Control          GameControl   { get; }

    protected void onGameEnd() => GameEnded?.Invoke();
}