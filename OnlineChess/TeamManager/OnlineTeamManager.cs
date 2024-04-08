using Common;
using Common.Chess;
using log4net;
using OnlineChess.Common;
using Tools;

namespace OnlineChess.TeamManager
{
    public class OnlineChessTeamManager : IChessTeamManager, IDisposable
    {
        private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessTeamManager));

        public event EventHandler<TeamId>?                   TeamSwitchedEvent;
        public event EventHandler<TeamAndToolPairEventArgs>? TeamAndToolPairEvent;
        public TeamId                                        CurrentTeamTurnId { get; private set; }
        public Team[]                                        Teams             => m_teams.Values.Cast<Team>().ToArray();

        public TeamId LocalMachineTeamId { get; }

        private readonly Dictionary<TeamId, OnlineChessTeam> m_teams;
        private readonly Dictionary<ToolId, TeamId>          m_toolIdToTeamId;
        private readonly Dictionary<TeamId, HashSet<ToolId>> m_teamIdToToolId;

        private readonly IChessServerAgent m_serverAgent;

        public OnlineChessTeamManager(OnlineChessTeam   localTeam
                                    , OnlineChessTeam   remoteTeam
                                    , TeamId            firstTeamId
                                    , IChessServerAgent serverAgent)
        {
            m_serverAgent     = serverAgent;
            m_teams           = new Dictionary<TeamId, OnlineChessTeam>();
            m_toolIdToTeamId  = new Dictionary<ToolId, TeamId>();
            m_teamIdToToolId  = new Dictionary<TeamId, HashSet<ToolId>>();

            CurrentTeamTurnId = firstTeamId;
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

        public ToolId[] GetToolsId(TeamId teamId)
        {
            return m_teamIdToToolId[teamId].ToArray();
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
            s_log.InfoFormat("Tool Id - Team Id pair added:[{0}]", pair);

            m_toolIdToTeamId[pair.ToolId] = pair.TeamId;
            m_teamIdToToolId[pair.TeamId].Add(pair.ToolId);
            TeamAndToolPairEvent?.Invoke(this, new TeamAndToolPairEventArgs(pair.TeamId, pair.ToolId));
        }

        private void initTeamsDict(OnlineChessTeam firstTeam
                                 , OnlineChessTeam secondTeam)
        {
            m_teams[firstTeam.Id]          = firstTeam;
            m_teamIdToToolId[firstTeam.Id] = new HashSet<ToolId>();
            
            m_teams[secondTeam.Id]         = secondTeam;
            m_teamIdToToolId[secondTeam.Id] = new HashSet<ToolId>();
        }

        private void registerToEvents()
        {
            m_serverAgent.UpdatePlayingTeamEvent   += onTeamSwitch;
            m_serverAgent.UpdateToolsAndTeamsEvent += onUpdateToolsAndTeamsEvent;
        }

        private void unRegisterFromEvents()
        {
            m_serverAgent.UpdatePlayingTeamEvent   -= onTeamSwitch;
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
            CurrentTeamTurnId = currentTeamId;
            TeamSwitchedEvent?.Invoke(this, CurrentTeamTurnId);
        }
    }
}