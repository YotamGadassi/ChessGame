using System;

namespace Common_6.ChessBoardEventArgs
{
    [Serializable]
    public class ToolMovedEventArgs : EventArgs
    {
        public ITool MovedTool { get; }
        public BoardPosition InitialPosition { get; }
        public BoardPosition EndPosition { get; }

        public ToolMovedEventArgs(ITool movedTool, BoardPosition initialPosition, BoardPosition endPosition)
        {
            MovedTool = movedTool;
            InitialPosition = initialPosition;
            EndPosition = endPosition;
        }
    }
}
