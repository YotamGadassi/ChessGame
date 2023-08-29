using System;

namespace Common;

public interface ITeamsManager
{
    public event EventHandler<Team> TeamSwitchedEvent;
    public Team                          CurrentTeamTurn { get; }
    public Team[]                        Teams           { get; }
}