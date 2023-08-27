using System;

namespace Common
{
    public interface IChessGameManager
    {
        IGameStateController GameStateController { get; }
        ITeamsManager        TeamsManager        { get; }
        IBoardEvents         BoardEvents         { get; }

        IChessBoardProxy ChessBoardProxy { get; }

        public TeamWithTimer CurrentTeamTurn { get; }

        public TeamWithTimer[] Teams { get; }
    }

    public interface IGameEvents
    {
        event Action<Team, Team> TeamChanged;
    }
}