using System;

namespace Common;

public enum GameState
{
    Running,
    NotStarted,
    Paused,
    Ended
}

public interface IGameStateController
{
    event EventHandler<GameState> StateChanged;

    GameState State { get; }

    void StartResumeGame();

    void PauseGame();

    void EndGame();
}