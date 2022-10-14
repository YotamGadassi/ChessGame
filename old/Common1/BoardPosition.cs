namespace Common
{
    public readonly struct BoardPosition
    {
        public int Column { get; }
        public int Row { get; }

        public static BoardPosition Empty = new BoardPosition(int.MinValue, int.MinValue);

        public BoardPosition(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }
    }
}
