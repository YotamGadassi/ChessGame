using System;

namespace Common_6.ChessBoardEventArgs
{
    public class CheckEventArgs : EventArgs
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
