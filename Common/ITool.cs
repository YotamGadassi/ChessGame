using System;
using System.Windows;

namespace Common
{
    public enum ToolType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }
    
    public interface ITool
    {

        ToolType Type { get; }

        bool IsMovingLegal(BoardPosition start, BoardPosition End, MoveState moveState);

        Point[] PossibleMoves(BoardPosition Start);

        Guid Key{ get;}

        bool IsToolFirstMove { get; }
    }
}
