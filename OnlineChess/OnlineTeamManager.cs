using Common;
using OnlineChess.Common;
using Tools;

namespace OnlineChess
{
    public class OnlineChessTeamManager : IChessTeamManager, IDisposable
    {
        public event EventHandler<TeamId>? TeamSwitchedEvent;
        public TeamId                      CurrentTeamTurnId { get; private set; }
        public Team[]                      Teams             => m_teams.Values.Cast<Team>().ToArray();

        public TeamId LocalMachineTeamId { get; }

        private readonly Dictionary<TeamId, OnlineChessTeam> m_teams;
        private readonly IChessServerAgent                   m_serverAgent;

        public OnlineChessTeamManager(OnlineChessTeam   localTeam
                                    , OnlineChessTeam   remoteTeam
                                    , Team              currentTeamTurn
                                    , IChessServerAgent serverAgent)
        {
            m_serverAgent     = serverAgent;
            m_teams           = new Dictionary<TeamId, OnlineChessTeam>();
            CurrentTeamTurnId = currentTeamTurn.Id;
            initTeamsDict(localTeam, remoteTeam);
            LocalMachineTeamId = localTeam.Id;
            registerToEvents();
        }

        public Team GetTeam(TeamId teamId)
        {
            return m_teams[teamId];
        }

        public TeamId? GetTeamId(ToolId toolId)
        {
            throw new NotImplementedException();
        }

        public ITeamTimer GetTeamTimer(TeamId teamId)
        {
            return m_teams[teamId].TeamTimer;
        }

        public bool IsLocalMachineTeamTurn()
        {
            return LocalMachineTeamId.Equals(CurrentTeamTurnId);
        }

        public void Dispose()
        {
            unRegisterFromEvents();
        }

        private void initTeamsDict(OnlineChessTeam firstTeam
                                 , OnlineChessTeam secondTeam)
        {
            m_teams[firstTeam.Id]  = firstTeam;
            m_teams[secondTeam.Id] = secondTeam;
        }

        private void registerToEvents()
        {
            m_serverAgent.UpdatePlayingTeamEvent += onTeamSwitch;
        }

        private void unRegisterFromEvents()
        {
            m_serverAgent.UpdatePlayingTeamEvent -= onTeamSwitch;
        }

        private void onTeamSwitch(TeamId currentTeamId)
        {
            TeamSwitchedEvent?.Invoke(this, CurrentTeamTurnId);
        }
    }
}