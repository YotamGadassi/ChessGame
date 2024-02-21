using System;
using Board;
using Tools;

namespace Common.Chess
{
    [Flags]
    public enum MoveResultEnum
    {
        NoChangeOccurred = 0x1
      , ToolMoved        = 0x2
      , ToolKilled       = 0x4
      , NeedPromotion    = 0x8
      , CheckMate        = 0x10
       ,
    }

    public class MoveResult
    {
        public static readonly MoveResult NoChangeOccurredResult =
            new(MoveResultEnum.NoChangeOccurred, BoardPosition.Empty, BoardPosition.Empty, null, null);

        public MoveResultEnum Result          { get; }
        public BoardPosition  InitialPosition { get; }
        public BoardPosition  EndPosition     { get; }
        public ITool          ToolAtInitial   { get; }
        public ITool          ToolAtEnd       { get; }

        public MoveResult(MoveResultEnum result
                        , BoardPosition  initialPosition
                        , BoardPosition  endPosition
                        , ITool          toolAtInitial
                        , ITool          toolAtEnd)
        {
            Result          = result;
            InitialPosition = initialPosition;
            EndPosition     = endPosition;
            ToolAtInitial   = toolAtInitial;
            ToolAtEnd       = toolAtEnd;
        }

        protected bool Equals(MoveResult other)
        {
            return Result == other.Result                && InitialPosition.Equals(other.InitialPosition) &&
                   EndPosition.Equals(other.EndPosition) && ToolAtInitial.Equals(other.ToolAtInitial)     &&
                   ToolAtEnd.Equals(other.ToolAtEnd);
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

            return Equals((MoveResult)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Result, InitialPosition, EndPosition, ToolAtInitial, ToolAtEnd);
        }

        public override string ToString()
        {
            return
                $"{nameof(Result)}: {Result}, {nameof(InitialPosition)}: {InitialPosition}, {nameof(EndPosition)}: {EndPosition}, {nameof(ToolAtInitial)}: {ToolAtInitial}, {nameof(ToolAtEnd)}: {ToolAtEnd}";
        }
    }
}