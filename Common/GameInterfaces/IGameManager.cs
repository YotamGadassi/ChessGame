using System;

namespace Common.GameInterfaces
{
    public enum GameState
    {
        Running,
        NotStarted,
        Paused,
        Ended
    }

    public interface IGameManager
    {
        event EventHandler<GameState> StateChanged;

        GameState State { get; }

        bool StartResumeGame();

        bool PauseGame();

        void EndGame();
    }
}