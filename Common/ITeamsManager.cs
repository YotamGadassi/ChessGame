using System;

namespace Common;

public interface ITeamsManager
{
    public event EventHandler<TeamWithTimer> TeamSwitchedEvent;
    public TeamWithTimer   CurrentTeamTurn { get; }
    public TeamWithTimer[] Teams           { get; }
}

public class OfflineTeamsManager : ITeamsManager
{
    public event EventHandler<TeamWithTimer> TeamSwitchedEvent;

    private static readonly int s_teamsAmount = 2;
    public TeamWithTimer   CurrentTeamTurn => Teams[m_currentTeamIndex];
    public TeamWithTimer[] Teams           { get; }

    private                 int m_currentTeamIndex = 0;
    
    public OfflineTeamsManager(TeamWithTimer[] teams)
    {
        Teams           = teams;
    }

    public void SwitchCurrentTeam()
    {
        CurrentTeamTurn.StopTimer();
        switchTeamIndex();
        TeamSwitchedEvent?.Invoke(this, CurrentTeamTurn);
        CurrentTeamTurn.StartTimer();
    }

    private void switchTeamIndex()
    {
        m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
    }
}