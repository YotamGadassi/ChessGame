using System;
using System.Windows;

namespace Common
{
    public interface ITool
    {
        bool IsMovingLegal(BoardPosition start, BoardPosition End);

        Point[] PossibleMoves(BoardPosition Start);

        Guid Key
        {
            get;
        }
    }
}
