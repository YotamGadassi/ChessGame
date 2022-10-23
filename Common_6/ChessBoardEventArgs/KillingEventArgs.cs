namespace Common.ChessBoardEventArgs
{
    public class KillingEventArgs : ToolMovedEventArgs
    {
        public ITool KilledTool { get; }
        public KillingEventArgs(ITool movedTool, BoardPosition initialPosition, BoardPosition endPosition, ITool killedTool) : base(movedTool, initialPosition, endPosition)
        {
            KilledTool = killedTool;
        }
    }
}
