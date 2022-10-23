using System.Windows.Media;
using Common_6;

namespace Common
{
    public class Team
    {
        public GameDirection MoveDirection { get; }

        public string Name { get; }

        public Color Color { get; }

        public Team(string        name
                  , Color         color
                  , GameDirection moveDirection)
        {
            MoveDirection = moveDirection;
            Name          = name;
            Color         = color;
        }

        public bool Equals(Team? other)
        {
            if (null == other)
            {
                return false;
            }

            return other.MoveDirection.Equals(MoveDirection)
                && other.Name.Equals(Name)
                && other.Color.Equals(Color);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Team);
        }

        public override int    GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + MoveDirection.GetHashCode();
                hash = hash * 23 + Name?.GetHashCode() ?? 0;
                hash = hash * 23 + Color.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"Name:[{Name}], Move Direction:[{MoveDirection}], Color:[{Color}]";
        }
    }
}
