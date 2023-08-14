using System;

namespace Common
{
    [Flags]
    public enum MoveResultEnum
    {
        NoChangeOccurred = 0x1,
        ToolMoved = 0x2,
        ToolKilled = 0x4,
        NeedPromotion = 0x8,
        CheckMate = 0x10,
    }

    public class MoveResult
    {
        public static readonly MoveResult NoChangeOccurredResult =
            new(MoveResultEnum.NoChangeOccurred, BoardPosition.Empty, BoardPosition.Empty, null, null);

        public MoveResultEnum Result { get; }
        public BoardPosition InitialPosition { get; }
        public BoardPosition EndPosition { get; }
        public ITool ToolAtInitial { get; }
        public ITool ToolAtEnd { get; }

        public MoveResult(MoveResultEnum result, BoardPosition initialPosition, BoardPosition endPosition, ITool toolAtInitial, ITool toolAtEnd)
        {
            Result = result;
            InitialPosition = initialPosition;
            EndPosition = endPosition;
            ToolAtInitial = toolAtInitial;
            ToolAtEnd = toolAtEnd;
        }
    }

}
