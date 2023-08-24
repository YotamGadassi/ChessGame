using System;
using System.Collections.Generic;
using Board;
using ChessGame;
using ChessGame.Helpers;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Tools;

namespace Client.GameManagers
{
    public class OnlineChessGameManager : BaseChessGameManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGameManager));

        public event Action<TeamWithTimer[]> TeamsInitialized;

        public Team                  LocalMachineTeam     { get; private set; }
        public IAvailableMovesHelper AvailableMovesHelper { get; }


        private IConnectionManager<IGameServerAgent> m_connectionManager;
        private Dictionary<int, TeamWithTimer>       m_teams;

        public OnlineChessGameManager(ChessBoard                           gameBoard
                                    , IConnectionManager<IGameServerAgent> connectionManager)
            : base(gameBoard, s_log)
        {
            m_connectionManager  = connectionManager;
            AvailableMovesHelper = new AvailableMovesHelper(this);
            registerToEvents();
        }

        private void registerToEvents()
        {
            IGameServerAgent serverAgent = m_connectionManager.ServerAgent;

            serverAgent.BoardReceivedEvent    += onBoardReceivedEvent;
            serverAgent.StartGameRequestEvent += onStartGameRequestEvent;
            serverAgent.TimeReceivedEvent     += onTimeReceivedEvent;
        }

        private void unRegisterFromEvents()
        {
            IGameServerAgent serverAgent = m_connectionManager.ServerAgent;

            serverAgent.BoardReceivedEvent    -= onBoardReceivedEvent;
            serverAgent.StartGameRequestEvent -= onStartGameRequestEvent;
            serverAgent.TimeReceivedEvent     -= onTimeReceivedEvent;
        }
        private void onTimeReceivedEvent(Team     team
                                       , TimeSpan timeleft)
        {
            TeamWithTimer currTeam = m_teams[team.GetHashCode()];
            currTeam.SetTimeLeft(timeleft);
        }

        private void onStartGameRequestEvent(Team localTeam
                                           , Team remoteTeam
                                           , Guid gametoken)
        {
            s_log.Info($"Start Game Request Received: [local team: {localTeam}, remote team: {remoteTeam}, token: {gametoken}");

            LocalMachineTeam = localTeam;

            TeamWithTimer localTeamWithTimer  = createTeamWithTimer(localTeam);
            TeamWithTimer remoteTeamWithTimer = createTeamWithTimer(remoteTeam); 
            
            //TODO: Add Guid to each team
            m_teams.Add(localTeam.GetHashCode(),  localTeamWithTimer);
            m_teams.Add(remoteTeam.GetHashCode(), remoteTeamWithTimer);

            TeamWithTimer[] teamsWithTimer = { localTeamWithTimer, remoteTeamWithTimer };

            setTeams(teamsWithTimer);
            TeamsInitialized?.Invoke(teamsWithTimer);
        }

        private TeamWithTimer createTeamWithTimer(Team team)
        {
            return new TeamWithTimer(team.Name, team.Color, team.MoveDirection, TimeSpan.FromMinutes(10));
        }

        private void onBoardReceivedEvent(BoardState boardState)
        {
            foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }
        }
    }
}