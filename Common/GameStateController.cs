using System;

namespace Common;

public class GameStateController : IGameStateController
{
    public event EventHandler<GameState>? StateChanged;
    public GameState                      State { get; private set; }

    public GameStateController()
    {
        State = GameState.NotStarted;
    }

    public void StartResumeGame()
    {
        State = GameState.Running;
        StateChanged?.Invoke(this, State);
    }

    public void PauseGame()
    {
        State = GameState.Paused;
        StateChanged?.Invoke(this, State);
    }

    public void EndGame()
    {
        State = GameState.Ended;
        StateChanged?.Invoke(this, State);
    }
}