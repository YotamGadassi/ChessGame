using System;

namespace Common;

public interface IGameStateController : IGameState
{
    void StartResumeGame();

    void PauseGame();

    void EndGame();
}