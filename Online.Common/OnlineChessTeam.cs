using System.Windows.Media;
using Common;

namespace OnlineChess.Common;

public class OnlineChessTeam : Team
{
    public ITeamTimer TeamTimer { get; }

    public OnlineChessTeam(string        name
                         , Color         color
                         , GameDirection gameDirection
                         , ITeamTimer    teamTimer) : base(name, color, gameDirection)
    {
        TeamTimer = teamTimer;
    }
}