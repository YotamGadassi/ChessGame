using System;

namespace Common;

public interface IGameStateController
{
    event EventHandler<GameState> StateChanged;

    GameState State { get; }

    void StartResumeGame();

    void PauseGame();

    void EndGame();
}