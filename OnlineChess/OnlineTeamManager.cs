using Common;
using log4net;
using OnlineChess.Common;
using Tools;

namespace OnlineChess
{
    public class OnlineChessTeamManager : IChessTeamManager, IDisposable
    {
        private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessTeamManager));

        public event EventHandler<TeamId>? TeamSwitchedEvent;
        public TeamId CurrentTeamTurnId { get; private set; }
        public Team[] Teams => m_teams.Values.Cast<Team>().ToArray();

        public TeamId LocalMachineTeamId { get; }

        private readonly Dictionary<TeamId, OnlineChessTeam> m_teams;
        private readonly Dictionary<ToolId, TeamId> m_toolIdToTeamId;

        private readonly IChessServerAgent m_serverAgent;

        public OnlineChessTeamManager(OnlineChessTeam localTeam
                                    , OnlineChessTeam remoteTeam
                                    , Team currentTeamTurn
                                    , IChessServerAgent serverAgent)
        {
            m_serverAgent = serverAgent;
            m_teams = new Dictionary<TeamId, OnlineChessTeam>();
            m_toolIdToTeamId = new Dictionary<ToolId, TeamId>();
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
            return m_toolIdToTeamId[toolId];
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

        private void addToolId(ToolAndTeamPair pair)
        {
            s_log.InfoFormat("Tool Id - Team Id pair added:[Tool Id:{0} | Team id:{1}]");

            m_toolIdToTeamId[pair.ToolId] = pair.TeamId;
        }

        private void initTeamsDict(OnlineChessTeam firstTeam
                                 , OnlineChessTeam secondTeam)
        {
            m_teams[firstTeam.Id] = firstTeam;
            m_teams[secondTeam.Id] = secondTeam;
        }

        private void registerToEvents()
        {
            m_serverAgent.UpdatePlayingTeamEvent += onTeamSwitch;
            m_serverAgent.UpdateToolsAndTeamsEvent += onUpdateToolsAndTeamsEvent;
        }

        private void unRegisterFromEvents()
        {
            m_serverAgent.UpdatePlayingTeamEvent -= onTeamSwitch;
            m_serverAgent.UpdateToolsAndTeamsEvent -= onUpdateToolsAndTeamsEvent;
        }

        private void onUpdateToolsAndTeamsEvent(ToolAndTeamPair[] pairs)
        {
            foreach (ToolAndTeamPair toolAndTeamPair in pairs)
            {
                addToolId(toolAndTeamPair);
            }
        }

        private void onTeamSwitch(TeamId currentTeamId)
        {
            TeamSwitchedEvent?.Invoke(this, CurrentTeamTurnId);
        }
    }
}