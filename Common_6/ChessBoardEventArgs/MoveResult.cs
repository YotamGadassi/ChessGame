using Common_6;

namespace Common.ChessBoardEventArgs
{
    public enum MoveResultEnum
    {
        ToolMoved        = 1,
        ToolKilled       = 2,
        NoChangeOccurred = 3
    }

    public class MoveResult
    {
        public static readonly MoveResult NoChangeOccurredResult =
            new MoveResult(MoveResultEnum.NoChangeOccurred, BoardPosition.Empty, BoardPosition.Empty, null, null); 
        
        public MoveResultEnum Result          { get; }
        public BoardPosition  InitialPosition { get; }
        public BoardPosition  EndPosition     { get; }
        public ITool          ToolAtInitial   { get; }
        public ITool          ToolAtEnd       { get; }

        public MoveResult(MoveResultEnum result, BoardPosition initialPosition, BoardPosition endPosition, ITool toolAtInitial, ITool toolAtEnd)
        {
            Result          = result;
            InitialPosition = initialPosition;
            EndPosition     = endPosition;
            ToolAtInitial   = toolAtInitial;
            ToolAtEnd       = toolAtEnd;
        }
    }

}
