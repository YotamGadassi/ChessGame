using System.Windows;
using System.Windows.Input;
using Common;

namespace FrontCommon;

public abstract class BaseGameControllerViewModel : DependencyObject
{
    private static readonly DependencyProperty gameStateProperty =
        DependencyProperty.Register("GameState", typeof(GameStateEnum), typeof(BaseGameControllerViewModel));

    public GameStateEnum GameState
    {
        get => (GameStateEnum)GetValue(gameStateProperty);
        set => SetValue(gameStateProperty, value);
    }

    public abstract ICommand StartResume { get; }

    public abstract ICommand EndGame { get; }

    public abstract ICommand Pause { get; }
}