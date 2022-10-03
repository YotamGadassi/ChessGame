using Common;

namespace ChessBoard.EventArgs
{
    public class CheckEventArgs : System.EventArgs
    {
        public TeamColor TeamMadeCheck { get; }
        public BoardPosition KingPosition { get; }
        public BoardPosition CheckToolPosition { get; }

        public CheckEventArgs(TeamColor teamMadeCheck, BoardPosition kingPosition, BoardPosition checkToolPosition)
        {
            TeamMadeCheck = teamMadeCheck;
            KingPosition = kingPosition;
            CheckToolPosition = checkToolPosition;
        }
    }
}
