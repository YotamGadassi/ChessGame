using System;
using Common;

namespace Client.Board
{
    public class BoardClickEventArgs : EventArgs
    {
        public BoardPosition Position { get; }
        public ITool?        Tool     { get; }

        public BoardClickEventArgs(BoardPosition position, ITool? tool)
        {
            Position = position;
            Tool     = tool;
        }
    }
}
