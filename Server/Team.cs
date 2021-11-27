using Common;
using System.Collections.Generic;
using ChessBoard;
using System.Windows.Media;

namespace Game
{
    public class Team
    {
        public string Name { get; }

        public Color Color { get; }

        public GameDirection MoveDirection { get; }

        internal IList<ITool> Tools { get; }

        public Team(string TeamName, Color TeamColor, GameDirection TeamMovingDirection)
        {
            Name = TeamName;
            Color = TeamColor;
            MoveDirection = TeamMovingDirection;
            Tools = new List<ITool>();
        }
    }
}
