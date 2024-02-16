using System.Windows.Media;
using Common;

namespace OnlineChess
{
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

    public class OnlineChessTeamManager : IChessTeamManager, IDisposable
    {
        public event EventHandler<Team>? TeamSwitchedEvent;
        public Team                      CurrentTeamTurn  { get; private set; }
        public Team[]                    Teams            => m_teams.Values.ToArray();
        public Team                      LocalMachineTeam { get; }

        private readonly Dictionary<Guid, Team>       m_teams;
        private readonly Dictionary<Guid, ITeamTimer> m_teamsTimers;
        private readonly IChessServerAgent            m_serverAgent;

        public OnlineChessTeamManager(OnlineChessTeam     localTeam
                                    , OnlineChessTeam     otherTeam
                                    , Team              currentTeamTurn
                                    , IChessServerAgent serverAgent)
        {
            m_serverAgent   = serverAgent;
            m_teams         = new Dictionary<Guid, Team>();
            m_teamsTimers   = new Dictionary<Guid, ITeamTimer>();
            CurrentTeamTurn = currentTeamTurn;
            initTeamsDict(localTeam, otherTeam);
            initTeamsTimerDict(localTeam, otherTeam);
            LocalMachineTeam = localTeam;
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

        private void initTeamsTimerDict(OnlineChessTeam firstTeam
                                      , OnlineChessTeam secondTeam)
        {
            m_teamsTimers[firstTeam.Id]  = firstTeam.TeamTimer;
            m_teamsTimers[secondTeam.Id] = secondTeam.TeamTimer;
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

        private void onTeamSwitch(Guid currentTeamId)
        {
            CurrentTeamTurn = m_teams[currentTeamId];
            TeamSwitchedEvent?.Invoke(this, CurrentTeamTurn);
        }
    }
}