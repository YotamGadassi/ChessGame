using Common;

namespace ChessBoard.ChessBoardEventArgs
{
    public enum EndGameReason
    {
        CheckMate = 0,
        Draw = 1,
        BlackPlayerResign = 2,
        WhitePlayerResign = 3,
        FlagFall = 4
    }

    public class EndGameEventArgs : System.EventArgs
    {
        public EndGameReason Reason {get;}

        public BoardPosition WhiteKingLastPosition { get; }

        public BoardPosition BlackKingLastPosition { get; }

        public EndGameEventArgs(EndGameReason reason, BoardPosition whiteKingLastPosition, BoardPosition blackKingLastPosition)
        {
            Reason = reason;
            WhiteKingLastPosition = whiteKingLastPosition;
            BlackKingLastPosition = blackKingLastPosition;
        }
    }
}
