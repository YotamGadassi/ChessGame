using System;

namespace Common.ChessBoardEventArgs
{
    public class ToolPromotedEventArgs : EventArgs
    {
        public ITool ToolBeforePromotion { get; } 
        public ITool ToolAfterPromotion  { get; }
        public BoardPosition ToolPosition { get; }

        public ToolPromotedEventArgs(ITool          toolBeforePromotion
                                    , ITool         toolAfterPromotion
                                    , BoardPosition boardPosition)
        {
            ToolBeforePromotion = toolBeforePromotion;
            ToolAfterPromotion  = toolAfterPromotion;
            ToolPosition       = boardPosition;
        }
    }
}
