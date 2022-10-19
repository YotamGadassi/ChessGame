using System.Windows.Media;
using Common_6;

namespace Common
{
    public class Team
    {
        public GameDirection MoveDirection { get; }

        public string Name { get; }

        public Color Color { get; }

        public Team(string name, Color color, GameDirection moveDirection)
        {
            MoveDirection = moveDirection;
            Name          = name;
            Color         = color;
        }
    }
}
