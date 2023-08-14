using System;
using Board;
using Tools;

namespace Common.Chess.ChessBoardEventArgs
{
    public class PromotionEventArgs : EventArgs
    {
        public ITool         ToolToPromote { get; }
        public BoardPosition ToolPosition        { get; }

        public PromotionEventArgs(ITool toolBeforePromotion, BoardPosition toolPosition)
        {
            ToolToPromote = toolBeforePromotion;
            ToolPosition        = toolPosition;
        }
    }
}
