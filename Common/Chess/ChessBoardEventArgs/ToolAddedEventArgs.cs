using System;
using Board;
using Tools;

namespace Common.Chess.ChessBoardEventArgs
{
    public class ToolAddedEventArgs : EventArgs
    {
        public ITool         AddedTool { get; }
        public BoardPosition Position  { get; }

        public ToolAddedEventArgs(ITool addedTool, BoardPosition position)
        {
            AddedTool = addedTool;
            Position  = position;
        }
    }
}
