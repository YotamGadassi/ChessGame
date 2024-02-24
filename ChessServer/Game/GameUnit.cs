﻿using System.Drawing;
using System.Windows.Media;
using Board;
using ChessGame;
using ChessServer.ChessPlayer;
using Common;
using Common.Chess;
using OnlineChess.Common;
using Tools;
using Color = System.Windows.Media.Color;

namespace ChessServer.Game
{
    public class GameUnit : IGameUnit
    {
        public ServerChessPlayer[] ChessPlayers  { get; }
        public GameId              Id            { get; }
        public TeamId              CurrentTeamId => m_gameManager.TeamsManager.CurrentTeamTurnId;

        private          OfflineChessGameManager               m_gameManager;
        private readonly Dictionary<TeamId, ServerChessPlayer> m_teamToPlayers;
        private readonly Dictionary<TeamId, Action<TimeSpan>>  m_teamToTimerEvent;

        public GameUnit(ServerChessPlayer[] chessPlayers
                      , GameId              id)
        {
            ChessPlayers       = chessPlayers;
            Id                 = id;
            m_teamToPlayers    = new Dictionary<TeamId, ServerChessPlayer>();
            m_teamToTimerEvent = new Dictionary<TeamId, Action<TimeSpan>>();
        }

        public PromotionResult Promote(BoardPosition position
                                     , ITool         tool)
        {
            return m_gameManager.ChessBoardProxy.Promote(position, tool);
        }

        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            MoveResult     result     = m_gameManager.ChessBoardProxy.Move(start, end);
            MoveResultEnum resultEnum = result.Result;
            if (resultEnum == MoveResultEnum.NeedPromotion)
            {
                ToolId            toolId = result.ToolAtInitial.ToolId;
                TeamId            teamId = m_gameManager.TeamsManager.GetTeamId(toolId);
                ServerChessPlayer player = m_teamToPlayers[teamId];
                Task.Run(() => AskPromotion(player, result.EndPosition));
            }

            return result;
        }

        public async void AskPromotion(ServerChessPlayer player
                                     , BoardPosition     position)
        {
            PromotionResult promotionResult = PromotionResult.NoPromotionOccured;
            while (promotionResult.Result != PromotionResultEnum.PromotionSucceeded)
            {
                ITool tool = await player.AskPromote(position);
                promotionResult = Promote(position, tool);
                switch (promotionResult.Result)
                {
                    case PromotionResultEnum.PositionIsEmpty:
                    case PromotionResultEnum.NoPromotionOccured:
                    {
                        throw new Exception(string.Format("Error with promotion. Promotion Result: {0}"
                                                        , promotionResult));
                    }
                    case PromotionResultEnum.ToolIsNotValidForPromotion:
                    case PromotionResultEnum.PromotionSucceeded:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void StartGame()
        {
            OfflineTeamsManager teamsManager = createTeamManager();
            m_gameManager = new OfflineChessGameManager(teamsManager);
            registerToEvents();


            m_gameManager.Init();
        }

        public void EndGame(PlayerId      playerId
                          , EndGameReason reason)
        {
            unRegisterFromEvents();
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.EndGame(reason);
            }
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
        }

        private void onTimeLeftChange(TeamId   teamId
                                    , TimeSpan timeLeft)
        {
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.UpdateTime(teamId, timeLeft);
            }
        }

        private void onStateChanged(object?       sender
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
                    foreach (ServerChessPlayer player in ChessPlayers)
                    {
                        player.EndGame(EndGameReason.Withdraw);
                    }
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
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.BoardCommands(new[] { new BoardCommand(BoardCommandType.Remove, position) });
            }
        }

        private void onToolAdd(ITool         tool
                             , BoardPosition position)
        {
            foreach (ServerChessPlayer player in ChessPlayers)
            {
                player.BoardCommands(new[] { new BoardCommand(BoardCommandType.Add, position, tool) });
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
            ServerChessPlayer player1 = ChessPlayers[0];
            ChessTeam team1 = new(player1.Name, Colors.White, GameDirection.North
                                , new TeamTimer(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1)));

            player1.ChessTeam         = team1;
            m_teamToPlayers[team1.Id] = player1;

            ServerChessPlayer player2 = ChessPlayers[1];
            ChessTeam team2 = new(player2.Name, Colors.Black, GameDirection.South
                                , new TeamTimer(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1)));
            player2.ChessTeam         = team2;
            m_teamToPlayers[team2.Id] = player2;

            return new OfflineTeamsManager(new[] { team1, team2 });
        }
    }
}