using System;
using System.Windows;
using Common;

namespace Server
{
    public class ToolPawn : ITool
    {
        public ToolType Type => ToolType.Pawn;
        public Guid Key { get; }
        public bool IsToolFirstMove 
        {
            get; internal set;
        }

        public ToolPawn()
        {
            IsToolFirstMove = false;
        }

        public bool IsMovingLegal(BoardPosition start, BoardPosition End, MoveState moveState)
        {
            Move move = new Move(start, End);

            return PawnsMoveLogic.IsMoveLegal(move, moveState);
        }

        public Point[] PossibleMoves(BoardPosition Start)
        {
            throw new NotImplementedException();
        }
    }
}
