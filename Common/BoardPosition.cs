namespace Common
{
    public struct BoardPosition
    {
        public int Column { get; }
        public int Row { get; }

        public static BoardPosition Empty = new BoardPosition(int.MinValue, int.MinValue);

        public BoardPosition(int Column_, int Row_)
        {
            Column = Column_;
            Row = Row_;
        }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }
    }
}
