using System;
using System.Windows.Media;

namespace Common
{
    public class Team
    {
        public GameDirection MoveDirection { get; }

        public string Name { get; }

        public Color Color { get; }

        public Guid Id { get; }

        public Team(string        name
                  , Color         color
                  , GameDirection moveDirection)
        {
            MoveDirection = moveDirection;
            Name          = name;
            Color         = color;
            Id = Guid.NewGuid();
        }

        public bool Equals(Team other)
        {
            return other.Id.Equals(Id)
                && other.MoveDirection.Equals(MoveDirection)
                && other.Name.Equals(Name)
                && other.Color.Equals(Color);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Team otherTeam)
            {
                return Equals(otherTeam);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + MoveDirection.GetHashCode();
                hash = hash * 23 + Name?.GetHashCode() ?? 0;
                hash = hash * 23 + Color.GetHashCode();
                hash = hash * 23 + Id.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{nameof(MoveDirection)}: {MoveDirection}, {nameof(Name)}: {Name}, {nameof(Color)}: {Color}, {nameof(Id)}: {Id}";
        }
    }
}
