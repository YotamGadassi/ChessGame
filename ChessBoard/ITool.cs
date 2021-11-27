using System.Windows.Media;
using Common;

namespace ChessBoard
{
    public interface ITool
    {
        BoardPosition Position { get; }

        Color Color { get; }

         GameDirection MovingDirection {get;}

        string Type { get; }

        bool IsMovingLegal(BoardPosition End, ITool ToolAtEndPoint);

        BoardPosition[] PossibleMoves(BoardPosition Start);

        bool IsFirstMove { get; }

        ITool GetCopy();

        void Move(BoardPosition Postion);

        void Deactivate();
    }
}
