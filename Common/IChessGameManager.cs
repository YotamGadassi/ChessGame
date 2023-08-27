using System;
using Board;
using Common.Chess;
using Tools;

namespace Common
{
    public interface IChessGameManager
    {
        IGameStateController GameStateController { get; }
        IBoardEvents         BoardEvents     { get; }
        public TeamWithTimer CurrentTeamTurn { get; }

        public TeamWithTimer[] Teams         { get; }

        bool TryGetTool(BoardPosition position
                      , out ITool     tool);

        MoveResult Move(BoardPosition start
                      , BoardPosition end);

        PromotionResult Promote(BoardPosition start
                              , ITool         newTool);

    }

    public interface IGameEvents
    {
        event Action<Team, Team> TeamChanged;
    }
}