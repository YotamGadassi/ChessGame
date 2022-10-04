using System;
using Common;

namespace ChessBoard.ChessBoardEventArgs
{
    public class PromotionEventArgs : EventArgs
    {
        public ITool PromotedTool { get; }
        public BoardPosition ToolPosition { get; }

        public PromotionEventArgs(ITool promotedTool, BoardPosition toolPosition)
        {
            PromotedTool = promotedTool;
            ToolPosition = toolPosition;
        }
    }
}
