using Board;
using Tools;

namespace Common.Chess
{
    public enum PromotionResultEnum
    {
        PositionIsEmpty
      , ToolIsNotValidForPromotion
      , PromotionSucceeded
      , NoPromotionOccured
    }

    public class PromotionResult
    {
        public static PromotionResult NoPromotionOccured = new(null, null, BoardPosition.Empty, PromotionResultEnum.NoPromotionOccured);

        public ITool PromotedTool { get; }
        public ITool NewTool { get; }
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