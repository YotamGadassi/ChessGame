using System;
using System.Windows.Media;

namespace Common.ChessBoardEventArgs
{
    public class CheckmateEventArgs : EventArgs
    {
        public Color WinningTeamColor { get; }
        public BoardPosition KingLastPosition { get; }
        public BoardPosition KillerPosition { get; }

        public CheckmateEventArgs(Color winningTeamColor, BoardPosition kingPosition, BoardPosition killerPosition)
        {
            WinningTeamColor = winningTeamColor;
            KingLastPosition = kingPosition;
            KillerPosition   = killerPosition;
        }
    }
}
