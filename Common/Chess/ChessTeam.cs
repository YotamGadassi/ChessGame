using System.Windows.Media;

namespace Common.Chess
{
    public class ChessTeam : Team
    {
        public TeamTimer TeamTimer { get; }

        public ChessTeam(string        name
                       , Color         color
                       , GameDirection moveDirection
                       , TeamTimer     teamTimer) : base(name, color, moveDirection)
        {
            TeamTimer = teamTimer;
        }
    }
}