using System;
using System.Windows;

namespace Common
{
    public class ToolSoldier : ITool
    {
        private bool moveForward;

        public ToolSoldier(bool MoveForward)
        {
            moveForward = MoveForward;
        }

        public Guid Key { get; }

        public bool IsMovingLegal(BoardPosition Start, BoardPosition End)
        {

        }

        public Point[] PossibleMoves(BoardPosition Start)
        {
            throw new NotImplementedException();
        }
    }
}
