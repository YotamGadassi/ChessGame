using System.Windows.Media;
using Common;

namespace ChessGame
{
    public class Team
    {
        public string Name { get; }

        public Color Color { get; }

        public GameDirection MoveDirection { get; }

        public Team(string teamName, Color teamColor, GameDirection teamMovingDirection)
        {
            Name = teamName;
            Color = teamColor;
            MoveDirection = teamMovingDirection;
        }
    }
}
