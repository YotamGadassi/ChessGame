using System;
using System.Collections.Generic;
using Common.Chess;

namespace Common;

public interface ITeamsManager
{
    public event EventHandler<Team> TeamSwitchedEvent;
    public Team                          CurrentTeamTurn { get; }
    public Team[]                        Teams           { get; }
}

public interface IChessTeamManager : ITeamsManager
{
    public ITeamTimer GetTeamTimer(Team team);
}

public class OfflineTeamsManager : IChessTeamManager
{
    public event EventHandler<Team> TeamSwitchedEvent;

    private static readonly int        s_teamsAmount = 2;
    public                  Team       CurrentTeamTurn         => getCurrentTeam();
    public                  Team[]     Teams                   => m_teams;

    private int                          m_currentTeamIndex = 0;
    private ChessTeam[]                  m_teams;
    private Dictionary<Team, TeamTimer> m_teamTimers;

    public OfflineTeamsManager(ChessTeam[] teams)
    {
        m_teams                  = teams;
        m_teamTimers = new Dictionary<Team, TeamTimer>();
        m_teamTimers[m_teams[0]] = m_teams[0].TeamTimer;
        m_teamTimers[m_teams[1]] = m_teams[1].TeamTimer;
    }

    public ITeamTimer GetTeamTimer(Team team)
    {
        return m_teamTimers[team];
    }

    public void SwitchCurrentTeam()
    {
        TeamTimer currTeamTimer = getCurrentTeamTimer();
        currTeamTimer.StopTimer();
        
        switchTeamIndex();
        TeamSwitchedEvent?.Invoke(this, CurrentTeamTurn);
        
        currTeamTimer = getCurrentTeamTimer();
        currTeamTimer.StartTimer();
    }

    public void StartTimer(Team team)
    {
        m_teamTimers[team].StartTimer();
    }

    public void StopTimer(Team team)
    {
        m_teamTimers[team].StopTimer();
    }

    private void switchTeamIndex()
    {
        m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
    }

    private ChessTeam getCurrentTeam() => m_teams[m_currentTeamIndex];
    private TeamTimer getCurrentTeamTimer() => getCurrentTeam().TeamTimer;
}