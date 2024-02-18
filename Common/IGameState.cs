using System;

namespace Common;

public interface IGameState : IDisposable
{
    event EventHandler<GameStateEnum>? StateChanged;
    GameStateEnum                      State { get; }
}