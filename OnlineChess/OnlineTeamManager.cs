using Common;

namespace OnlineChess
{
    public class TeamWithTimer
    {
        public Team       Team      { get; }
        public ITeamTimer TeamTimer { get; }

        public TeamWithTimer(Team       team
                           , ITeamTimer teamTimer)
        {
            Team      = team;
            TeamTimer = teamTimer;
        }
    }

    public class OnlineChessTeamManager : IChessTeamManager, IDisposable
    {
        public event EventHandler<Team>? TeamSwitchedEvent;
        public Team                      CurrentTeamTurn  { get; private set; }
        public Team[]                    Teams            => m_teams.Values.ToArray();
        public Team                      LocalMachineTeam { get; }

        private Dictionary<Guid, Team>       m_teams;
        private Dictionary<Guid, ITeamTimer> m_teamsTimers;
        private IChessServerAgent            m_serverAgent;

        public OnlineChessTeamManager(TeamWithTimer     localTeam
                                    , TeamWithTimer     otherTeam
                                    , IChessServerAgent serverAgent)
        {
            m_serverAgent = serverAgent;
            initTeamsDict(localTeam.Team, otherTeam.Team);
            initTeamsTimerDict(localTeam, otherTeam);
            LocalMachineTeam = localTeam.Team;
            registerToEvents();
        }

        public ITeamTimer GetTeamTimer(Team team)
        {
            return m_teamsTimers[team.Id];
        }

        public bool IsLocalMachineTeamTurn()
        {
            return LocalMachineTeam.Equals(CurrentTeamTurn);
        }

        public void Dispose()
        {
            unRegisterFromEvents();
        }

        private void initTeamsTimerDict(TeamWithTimer firstTeam
                                      , TeamWithTimer secondTeam)
        {
            m_teamsTimers[firstTeam.Team.Id]  = firstTeam.TeamTimer;
            m_teamsTimers[secondTeam.Team.Id] = secondTeam.TeamTimer;
        }

        private void initTeamsDict(Team firstTeam
                                 , Team secondTeam)
        {
            m_teams[firstTeam.Id]  = firstTeam;
            m_teams[secondTeam.Id] = secondTeam;
        }

        private void registerToEvents()
        {
            m_serverAgent.SwitchTeamEvent += onTeamSwitch;
        }

        private void unRegisterFromEvents()
        {
            m_serverAgent.SwitchTeamEvent -= onTeamSwitch;
        }

        private void onTeamSwitch(Guid currentTeamid)
        {
            CurrentTeamTurn = m_teams[currentTeamid];
            TeamSwitchedEvent?.Invoke(this, CurrentTeamTurn);
        }
    }
}