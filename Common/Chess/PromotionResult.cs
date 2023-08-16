using Board;
using Tools;

namespace Common.Chess
{
    public enum PromotionResultEnum
    {
        PositionIsEmpty
      , ToolIsNotValidForPromotion
      , PromotionSucceeded
    }

    public class PromotionResult
    {
        public ITool         PromotedTool      { get; }
        public ITool         NewTool           { get; }
        public BoardPosition PromotionPosition { get; }

        public PromotionResultEnum Result { get; }

        public PromotionResult(ITool               promotedTool
                             , ITool               newTool
                             , BoardPosition       promotionPosition
                             , PromotionResultEnum result)
        {
            PromotedTool      = promotedTool;
            NewTool           = newTool;
            PromotionPosition = promotionPosition;
            Result            = result;
        }
    }
}