using Common;
using System.Windows;

namespace Server
{
    public class Move
    {
        public Vector Diff { get; }

        public Move(BoardPosition _Start, BoardPosition _End)
        {
            Start = _Start;
            End = _End;
            double xDiff = Start.Position.X - End.Position.X;
            double yDiff = Start.Position.Y - End.Position.Y;

            Diff = new Vector(xDiff, yDiff);
        }

        public BoardPosition Start { get; }
        public BoardPosition End { get; }
    }
}
