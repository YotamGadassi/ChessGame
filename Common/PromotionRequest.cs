using System;
using Board;
using Tools;

namespace Common
{
    public class PromotionRequest
    {
        public ITool         ToolToPromote { get; }
        public BoardPosition Position      { get; }

        public PromotionRequest(ITool          toolToPromote
                               , BoardPosition position)
        {
            ToolToPromote = toolToPromote;
            Position      = position;
        }

        protected bool Equals(PromotionRequest other)
        {
            return ToolToPromote.Equals(other.ToolToPromote) && Position.Equals(other.Position);
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

            return Equals((PromotionRequest)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToolToPromote, Position);
        }

        public override string ToString()
        {
            return $"{nameof(ToolToPromote)}: {ToolToPromote}, {nameof(Position)}: {Position}";
        }
    }
}
