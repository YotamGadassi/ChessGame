using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoard
{
    public enum EventType
    {
        Add,
        Remove,
        Move
    }

    public class ChessBoardEventArgs : EventArgs
    {
        public EventType Type {get;}

        public BoardPosition StartPosition { get; }

        public BoardPosition EndPosition { get; }

        public ITool ToolAtStartPosition { get; }

        public ITool ToolAtEndPosition { get; }

        public ChessBoardEventArgs(EventType Type_, BoardPosition StartPosition_, BoardPosition EndPosition_, ITool ToolAtStartPosition_, ITool ToolAtEndPosition_)
        {
            Type = Type_;
            StartPosition = StartPosition_;
            EndPosition = EndPosition_;
            ToolAtStartPosition = ToolAtStartPosition_;
            ToolAtEndPosition = ToolAtEndPosition_;
        }

        public ChessBoardEventArgs(EventType Type_, BoardPosition Position_, ITool Tool_)
        {
            Type = Type;
            StartPosition = Position_;
            EndPosition = BoardPosition.Empty;
            ToolAtStartPosition = Tool_;
            ToolAtEndPosition = null;
        }
    }
}
