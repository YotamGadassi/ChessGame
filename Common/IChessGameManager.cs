using System;

namespace Common
{
    public interface IChessGameManager
    {
        IGameStateController GameStateController { get; }
        IChessTeamManager    TeamsManager        { get; }
        IBoardEvents         BoardEvents         { get; }

        IChessBoardProxy ChessBoardProxy { get; }
    }
}