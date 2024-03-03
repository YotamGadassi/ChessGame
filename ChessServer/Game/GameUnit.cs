using System.Windows.Media;
using Board;
using ChessGame;
using ChessServer.ChessPlayer;
using Common;
using Common.Chess;
using OnlineChess.Common;
using Tools;

namespace ChessServer.Game
{
    public class GameUnit : IGameUnit, IDisposable
    {
        private readonly ILogger    m_log;

        public event EventHandler<GameEndedEventArgs>        GameEndedEvent;

        public IServerChessPlayer[] ChessPlayers  { get; }
        public GameId               Id            { get; }
        public TeamId               CurrentTeamId => m_gameManager.TeamsManager.CurrentTeamTurnId;

        private          OfflineChessGameManager                m_gameManager;
        private readonly Dictionary<TeamId, IServerChessPlayer> m_teamToPlayers;
        private readonly Dictionary<TeamId, Action<TimeSpan>>   m_teamToTimerEvent;
        private          int                                    m_initCounter = 0;

        public GameUnit(IServerChessPlayer[] chessPlayers
                      , GameId               id
                      , ILogger              log)
        {
            m_log = log;
            ChessPlayers            = chessPlayers;
            Id                      = id;
            m_teamToPlayers         = new Dictionary<TeamId, IServerChessPlayer>();
            m_teamToTimerEvent      = new Dictionary<TeamId, Action<TimeSpan>>();
        }

        public void StartGame()
        {
            OfflineTeamsManager teamsManager = createTeamManager();
            m_gameManager = new OfflineChessGameManager(teamsManager);
            registerToEvents();

            foreach (IServerChessPlayer player in ChessPlayers)
            {
                PlayerId playerId = player.PlayerId;
                Team[]   teams    = m_gameManager.TeamsManager.Teams;
                TeamConfig[] teamConfigs = teams.Select(team => new TeamConfig(team.MoveDirection == GameDirection.North
                                                                             , Equals(m_teamToPlayers[team.Id].PlayerId
                                                                                 , playerId)
                                                                             , team.MoveDirection
                                                                             , team.Name
                                                                             , team.Color
                                                                             , team.Id
                                                                             , m_gameManager.TeamsManager
                                                                                  .GetTeamTimer(team.Id)
                                                                                  .TimeLeft))
                                                .ToArray();

                GameConfig config = new(teamConfigs);
                player.StartGame(config);
            }
            m_gameManager.GameStateController.StartResumeGame();
        }

        public void Init()
        {
            ++m_initCounter;
            if (m_initCounter == 2)
            {
                m_gameManager.Init();
            }
        }

        public void EndGame()
        {
            m_gameManager.GameStateController.EndGame();
        }

        public PromotionResult Promote(BoardPosition position
                                     , ITool         tool)
        {
            PromotionResult result = m_gameManager.Promote(position, tool);
            m_log.LogInformation("Promotion occurred: [result: {0}]", result);
            return result;
        }

        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            return m_gameManager.Move(start, end);
        }

        public void Dispose()
        {
            unRegisterFromEvents();
            m_gameManager.Dispose();
        }

        private void registerToEvents()
        {
            m_gameManager.BoardEvents.ToolAddEvent       += onToolAdd;
            m_gameManager.BoardEvents.ToolRemoved        += onToolRemoved;
            m_gameManager.TeamsManager.TeamSwitchedEvent += onTeamSwitched;
            foreach (TeamId teamId in m_teamToPlayers.Keys)
            {
                ITeamTimer teamTimer = m_gameManager.TeamsManager.GetTeamTimer(teamId);
                m_teamToTimerEvent[teamId] =  (timeLeft) => onTimeLeftChange(teamId, timeLeft);
                teamTimer.TimeLeftChange   += m_teamToTimerEvent[teamId];
            }

            m_gameManager.TeamsManager.TeamAndToolPairEvent += onTeamAndToolPair;

            m_gameManager.GameStateController.StateChanged += onStateChanged;
            m_gameManager.AskPromotionEvent                += onAskPromotion;
            m_gameManager.CheckMateEvent                   += onCheckMate;
        }

        
        private void unRegisterFromEvents()
        {
            m_gameManager.BoardEvents.ToolAddEvent       -= onToolAdd;
            m_gameManager.BoardEvents.ToolRemoved        -= onToolRemoved;
            m_gameManager.TeamsManager.TeamSwitchedEvent -= onTeamSwitched;
            foreach (TeamId teamId in m_teamToPlayers.Keys)
            {
                ITeamTimer teamTimer = m_gameManager.TeamsManager.GetTeamTimer(teamId);
                teamTimer.TimeLeftChange -= m_teamToTimerEvent[teamId];
            }

            m_gameManager.TeamsManager.TeamAndToolPairEvent -= onTeamAndToolPair;
            m_gameManager.GameStateController.StateChanged  -= onStateChanged;
            m_gameManager.AskPromotionEvent                 -= onAskPromotion;
            m_gameManager.CheckMateEvent                    -= onCheckMate;
        }

        private void onCheckMate(CheckMateData checkMateData)
        {
            foreach (IServerChessPlayer serverChessPlayer in ChessPlayers)
            {
                serverChessPlayer.CheckMate(checkMateData);
            }
        }

        private void onAskPromotion(PromotionRequest promotionRequest)
        {
            ToolId             toolId = promotionRequest.ToolToPromote.ToolId;
            TeamId             teamId = m_gameManager.TeamsManager.GetTeamId(toolId);
            IServerChessPlayer player = m_teamToPlayers[teamId];
            askPromotion(player, promotionRequest.Position, promotionRequest.ToolToPromote);
        }

        private void onTimeLeftChange(TeamId   teamId
                                    , TimeSpan timeLeft)
        {
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.UpdateTime(teamId, timeLeft);
            }
        }

        private async void onStateChanged(object?       sender
                                  , GameStateEnum newState)
        {
            switch (newState)
            {
                case GameStateEnum.Running:
                    break;
                case GameStateEnum.NotStarted:
                    break;
                case GameStateEnum.Ended:
                {
                    foreach (IServerChessPlayer player in ChessPlayers)
                    {
                        await player.EndGame();
                    }
                    GameEndedEvent?.Invoke(this, new GameEndedEventArgs(this));
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void onTeamSwitched(object? sender
                                  , TeamId  teamId)
        {
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.UpdatePlayingTeam(teamId);
            }
        }

        private void onToolRemoved(BoardPosition position)
        {
            foreach (IServerChessPlayer player in ChessPlayers)
            {
                player.ApplyBoardCommands(new[] { new BoardCommand(BoardCommandType.Remove, position) });
            }
        }

        private void onToolAdd(ITool         tool
                             , BoardPosition position)
        {
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.ApplyBoardCommands(new[] { new BoardCommand(BoardCommandType.Add, position, tool) });
            }
        }

        private void onTeamAndToolPair(object?                  sender
                                     , TeamAndToolPairEventArgs pair)
        {
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.UpdateToolsAndTeams(new[] { new ToolAndTeamPair(pair.ToolId, pair.TeamId) });
            }
        }

        private OfflineTeamsManager createTeamManager()
        {
            IServerChessPlayer player1 = ChessPlayers[0];
            ChessTeam team1 = new(player1.Name, Colors.White, GameDirection.North
                                , new TeamTimer(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1)));

            player1.ChessTeam         = team1;
            m_teamToPlayers[team1.Id] = player1;

            IServerChessPlayer player2 = ChessPlayers[1];
            ChessTeam team2 = new(player2.Name, Colors.Black, GameDirection.South
                                , new TeamTimer(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1)));
            player2.ChessTeam         = team2;
            m_teamToPlayers[team2.Id] = player2;

            return new OfflineTeamsManager(new[] { team1, team2 });
        }

        private async void askPromotion(IServerChessPlayer player
                                      , BoardPosition      position
                                      , ITool              toolToPromote)
        {
            await player.AskPromote(new PromotionRequest(toolToPromote, position));
        }

    }
}