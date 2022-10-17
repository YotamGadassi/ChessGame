using System;
using Common_6;

namespace Client.Board
{
    public class BoardClickEventArgs : EventArgs
    {
        public BoardPosition Position { get; }
        public ITool         Tool     { get; }

        public BoardClickEventArgs(BoardPosition position, ITool tool)
        {
            Position = position;
            Tool     = tool;
        }
    }
}
