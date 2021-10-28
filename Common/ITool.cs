using System;
using System.Windows;

namespace Common
{
    public interface ITool
    {
        bool IsMovingLegal(BoardPosition start, BoardPosition End,bool IsMoveForward, bool IsFirstMove, bool isKilling);

        Point[] PossibleMoves(BoardPosition Start);

        Guid Key{ get;}

        int GroupNumber { get; }   
    }
}
