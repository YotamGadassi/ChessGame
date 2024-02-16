using System;
using Tools;

namespace Common.Chess;

public interface ITeamsManager
{
    public event EventHandler<TeamId> TeamSwitchedEvent;
    public TeamId                     CurrentTeamTurnId { get; }
    public Team[]                     Teams             { get; }

    public Team GetTeam(TeamId teamId);

    public TeamId? GetTeamId(ToolId toolId);
}