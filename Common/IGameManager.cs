using System;

namespace Common
{
    public enum GameState
    {
        Play,
        Stop,
        Pause,
        End
    }

    public interface IGameManager
    {
        event EventHandler<GameState> StateChanged;

        GameState State { get; }

        void StartGame();

        void PauseGame();

        void EndGame();
    }
}