using System;
using Board;
using Tools;

namespace Common.Chess.ChessBoardEventArgs
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
