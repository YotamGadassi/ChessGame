using System;

namespace Common;

public class GameStateController : IGameStateController
{
    public event EventHandler<GameStateEnum>? StateChanged;
    public GameStateEnum                      State { get; private set; }

    public GameStateController()
    {
        State = GameStateEnum.NotStarted;
    }

    public void StartResumeGame()
    {
        State = GameStateEnum.Running;
        StateChanged?.Invoke(this, State);
    }

    public void PauseGame()
    {
        State = GameStateEnum.Paused;
        StateChanged?.Invoke(this, State);
    }

    public void EndGame()
    {
        State = GameStateEnum.Ended;
        StateChanged?.Invoke(this, State);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}