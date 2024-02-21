using System;
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
        public static PromotionResult NoPromotionOccured =
            new(null, null, BoardPosition.Empty, PromotionResultEnum.NoPromotionOccured);

        public ITool               PromotedTool      { get; }
        public ITool               NewTool           { get; }
        public BoardPosition       PromotionPosition { get; }
        public PromotionResultEnum Result            { get; }

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

        protected bool Equals(PromotionResult other)
        {
            return PromotedTool.Equals(other.PromotedTool)           && NewTool.Equals(other.NewTool) &&
                   PromotionPosition.Equals(other.PromotionPosition) && Result == other.Result;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((PromotionResult)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PromotedTool, NewTool, PromotionPosition, (int)Result);
        }

        public override string ToString()
        {
            return
                $"{nameof(PromotedTool)}: {PromotedTool}, {nameof(NewTool)}: {NewTool}, {nameof(PromotionPosition)}: {PromotionPosition}, {nameof(Result)}: {Result}";
        }
    }
}