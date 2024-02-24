using Common;
using Common.Chess;
using Tools;

namespace ChessGame;

public class OfflineTeamsManager : IChessTeamManager, IDisposable
{
    public event EventHandler<TeamId>                    TeamSwitchedEvent;
    public event EventHandler<TeamAndToolPairEventArgs>? TeamAndToolPairEvent;

    private static readonly int    s_teamsAmount = 2;
    public                  TeamId CurrentTeamTurnId      => getCurrentTeam().Id;
    public                  Team[] Teams                  => m_teams.Cast<Team>().ToArray();

    private          int                                 m_currentTeamIndex = 0;
    private readonly ChessTeam[]                         m_teams;
    private readonly Dictionary<TeamId, ChessTeam>       m_teamIdToTeams;
    private readonly Dictionary<TeamId, HashSet<ToolId>> m_teamIdToTools;
    private readonly Dictionary<ToolId, TeamId>          m_toolIdToTeam;

    public OfflineTeamsManager(ChessTeam[] teams)
    {
        m_teams         = teams;
        m_teamIdToTeams = teams.ToDictionary((team) => team.Id);
        m_teamIdToTools = teams.ToDictionary((team) => team.Id, (_) => new HashSet<ToolId>());
        m_toolIdToTeam  = new Dictionary<ToolId, TeamId>();
    }

    public Team GetTeam(TeamId teamId)
    {
        return m_teamIdToTeams[teamId];
    }

    public TeamId? GetTeamId(ToolId toolId)
    {
        return m_toolIdToTeam[toolId];
    }

    public ToolId[] GetToolsId(TeamId teamId)
    {
        return m_teamIdToTools[teamId].ToArray();
    }

    public ITeamTimer GetTeamTimer(TeamId teamId)
    {
        return m_teamIdToTeams[teamId].TeamTimer;
    }

    public void AddToolId(TeamId teamId
                        , ToolId toolId)
    {
        m_teamIdToTools[teamId].Add(toolId);
        m_toolIdToTeam[toolId] = teamId;
        TeamAndToolPairEvent?.Invoke(this, new TeamAndToolPairEventArgs(teamId, toolId));
    }

    public void SwitchCurrentTeam()
    {
        TeamTimer currTeamTimer = getCurrentTeamTimer();
        currTeamTimer.StopTimer();
        
        switchTeamIndex();
        TeamSwitchedEvent?.Invoke(this, CurrentTeamTurnId);
        
        currTeamTimer = getCurrentTeamTimer();
        currTeamTimer.StartTimer();
    }

    public void StartTimer(TeamId teamId)
    {
        m_teamIdToTeams[teamId].TeamTimer.StartTimer();
    }

    public void StopTimer(TeamId teamId)
    {
        m_teamIdToTeams[teamId].TeamTimer.StopTimer();
    }

    public void Dispose()
    {
        foreach (ChessTeam chessTeam in m_teams)
        {
            chessTeam.TeamTimer.Dispose();
        }
    }

    private void switchTeamIndex()
    {
        m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
    }

    private ChessTeam getCurrentTeam()      => m_teams[m_currentTeamIndex];
    private TeamTimer getCurrentTeamTimer() => getCurrentTeam().TeamTimer;

}