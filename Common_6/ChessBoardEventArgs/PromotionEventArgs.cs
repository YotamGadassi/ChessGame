using System;

namespace Common_6.ChessBoardEventArgs
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
