using System.Windows.Media;
using Common;
using OnlineChess.Common;

namespace OnlineChess.TeamManager;

public class OnlineChessTeam : Team
{
    public ITeamTimer TeamTimer { get; }

    public OnlineChessTeam(string        name
                         , Color         color
                         , GameDirection gameDirection
                         , ITeamTimer    teamTimer,
                           TeamId teamId) : base(name, color, gameDirection, teamId)
    {
        TeamTimer = teamTimer;
    }

}

public static class TeamConfigExtention
{
    public static OnlineChessTeam ToOnlineChessTeam(this TeamConfig teamConfig, IChessServerAgent serverAgent)
    {
        OnlineTeamTimer teamTimer = new OnlineTeamTimer(serverAgent, teamConfig.Id, teamConfig.TotalGameTime);
        return new OnlineChessTeam(teamConfig.Name, teamConfig.Color, teamConfig.MoveDirection, teamTimer, teamConfig.Id);
    }
}