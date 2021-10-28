using System;
using System.Windows;
using Common;

namespace Server
{
    public class ToolPawn : ITool
    {
        public ToolPawn(int _GroupNumber)
        {
            GroupNumber = _GroupNumber;
        }

        public Guid Key { get; }

        public int GroupNumber { get; }

        public bool IsMovingLegal(BoardPosition start, BoardPosition End, bool isMoveForward, bool IsFirstMove, bool isKilling)
        {
            Move move = new Move(start, End);

            return PawnsMoveLogic.IsMoveLegal(move, isMoveForward, IsFirstMove, isKilling);
        }

        public Point[] PossibleMoves(BoardPosition Start)
        {
            throw new NotImplementedException();
        }
    }
}
