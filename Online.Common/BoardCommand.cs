using Board;
using Tools;

namespace OnlineChess.Common
{
    public enum BoardCommandType
    {
        Add
      , Remove
    }

    public class BoardCommand
    {
        public BoardCommandType Type { get; }
        public BoardPosition Position { get; }
        public ITool? Tool { get; }

        public BoardCommand(BoardCommandType type
                          , BoardPosition position
                          , ITool? tool = null)
        {
            Type = type;
            Position = position;
            Tool = tool;
        }

        protected bool Equals(BoardCommand other)
        {
            return Type == other.Type && Position.Equals(other.Position) && Equals(Tool, other.Tool);
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

            return Equals((BoardCommand)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Position, Tool);
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Position)}: {Position}, {nameof(Tool)}: {Tool}";
        }
    }
}