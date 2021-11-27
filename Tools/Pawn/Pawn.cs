using System;
using System.Windows.Media;
using ChessBoard;
using Common;

namespace Tools
{
    public class Pawn : ITool
    {
        public string Type => "Pawn";

        public BoardPosition Position { get; private set; }

        public Color Color { get; }

        public bool IsFirstMove { get; private set; }

        public GameDirection MovingDirection {get; }

        public Pawn(BoardPosition boardPosition, Color Color_, GameDirection MovingDirection_)
        {
            Color = Color_;
            Position = boardPosition;
            MovingDirection = MovingDirection_; 
            
            IsFirstMove = false;
        }

        public BoardPosition[] PossibleMoves(BoardPosition Start)
        {
            throw new NotImplementedException();
        }

        public bool IsMovingLegal(BoardPosition End, ITool ToolAtEndPoint)
        {
            return true;
        }

        public ITool GetCopy()
        {
            Pawn newPawn = new Pawn(Position, Color, MovingDirection);
            newPawn.IsFirstMove = IsFirstMove;

            return newPawn;
        }

        public void Move(BoardPosition Postion_)
        {
            Position = Postion_;
            IsFirstMove = true;
        }

        public void Deactivate()
        {
            Position = BoardPosition.Empty;
            IsFirstMove = false;
        }
    }
}
