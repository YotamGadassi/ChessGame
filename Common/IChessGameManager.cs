using System;
using Board;
using Common.Chess;
using Tools;

namespace Common
{
    public interface IChessGameManager
    {
        IBoardEvents         BoardEvents     { get; }
        public TeamWithTimer CurrentTeamTurn { get; }

        public TeamWithTimer[] Teams         { get; }

        bool                   IsGameRunning { get; }
        
        IAvailableMovesHelper AvailableMovesHelper { get; }
        
        void  StartGame();

        void  EndGame();

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