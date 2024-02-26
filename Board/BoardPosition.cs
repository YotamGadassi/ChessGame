using System.Text.Json.Serialization;

namespace Board
{
    public readonly struct BoardPosition
    {
        public int Column { get; }
        public int Row { get; }

        public static BoardPosition Empty = new(int.MinValue, int.MinValue);

        [JsonConstructor]
        public BoardPosition(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }

        public bool Equals(BoardPosition other)
        {
            return other.Column == Column && other.Row == Row;
        }

        public override bool Equals(object? obj)
        {
            return obj is BoardPosition && Equals((BoardPosition)obj);
        }

        public override string ToString()
        {
            return $"[Column:{Column}], [Row:{Row}]";
        }
    }

}
