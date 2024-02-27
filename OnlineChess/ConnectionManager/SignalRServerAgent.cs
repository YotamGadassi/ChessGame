using System.Reflection;
using Board;
using Common;
using Common.Chess;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using OnlineChess.Common;

namespace OnlineChess.ConnectionManager
{
    public class SignalRServerAgent : IChessServerAgent
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event StartGameHandler?           StartGameEvent;
        public event EndGameHandler?             EndGameEvent;
        public event BoardCommandsHandler?       BoardCommandsEvent;
        public event AskPromotionEventHandler?   AskPromotionEvent;
        public event CheckMateEventHandler?      CheckMateEvent;
        public event UpdateTimerHandler?         UpdateTimeEvent;
        public event UpdatePlayingTeamHandler?   UpdatePlayingTeamEvent;
        public event UpdateToolsAndTeamsHandler? UpdateToolsAndTeamsEvent;

        private readonly HubConnection m_connection;

        public SignalRServerAgent(HubConnection connection)
        {
            m_connection = connection;
            registerToEvents();
        }

        public async Task<GameRequestResult> SubmitGameRequest(GameRequest gameRequest)
        {
            s_log.InfoFormat("Game request sent to server: {0}", gameRequest);
            return await m_connection.InvokeAsync<GameRequestResult>("SubmitGameRequest", gameRequest);
        }

        public Task CancelGameRequest(GameRequestId gameRequestId)
        {
            s_log.InfoFormat("Cancel game request sent to server: {0}", gameRequestId);
            return m_connection.InvokeAsync("CancelGameRequest", gameRequestId);
        }

        public async Task SubmitGameWithdraw()
        {
            s_log.Info("Game withdraw sent to server");
            await m_connection.InvokeAsync("SubmitGameWithdraw");
        }

        public async Task Init()
        {
            s_log.Info("Init sent to server");
            await m_connection.InvokeAsync("Init");
        }

        public async Task<MoveResult> SubmitMove(BoardPosition start
                                               , BoardPosition end)
        {
            s_log.Info($"Submit Move sent to server: [start:{start} | end:{end}]");
            return await m_connection.InvokeAsync<MoveResult>("SubmitMove", start, end);
        }

        public async Task<PromotionResult> SubmitPromote(PromotionRequest promotionRequest)
        {
            s_log.Info($"Submit Promote sent to server: [{promotionRequest}]");
            return await m_connection.InvokeAsync<PromotionResult>("SubmitPromote"
                                                                 , promotionRequest);
        }

        public Task<TeamId> GetCurrentTeamTurn()
        {
            //TODO: consider remove
            return m_connection.InvokeAsync<TeamId>("GetCurrentTeamTurn");
        }

        public Task SendMessage(string msg)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        private void registerToEvents()
        {
            m_connection.On<GameConfig>("StartGame", handleStartGame);
            m_connection.On<EndGameReason>("EndGame", handleEndGame);
            m_connection.On<TeamId, TimeSpan>("UpdateTime", handleUpdateTime);
            m_connection.On<PromotionRequest>("AskPromote", handleAskPromotion);
            m_connection.On<CheckMateData>("CheckMate", handleCheckMate);
            m_connection.On<BoardCommand[]>("ApplyBoardCommands", handleApplyBoardCommands);
            m_connection.On<TeamId>("UpdatePlayingTeam", handleUpdatePlayingTeam);
            m_connection.On<ToolAndTeamPair[]>("UpdateToolsAndTeams", handleUpdateToolsAndTeams);
        }

        private void handleCheckMate(CheckMateData checkMateData)
        {
            s_log.DebugFormat("CheckMate Arrived: [{0}]", checkMateData);
            CheckMateEvent?.Invoke(checkMateData);
        }

        private void handleUpdateToolsAndTeams(ToolAndTeamPair[] pairs)
        {
            s_log.DebugFormat("UpdateToolsAndTeams Arrived: [Pairs: {0}]", string.Join<ToolAndTeamPair>(",", pairs));
            UpdateToolsAndTeamsEvent?.Invoke(pairs);
        }

        private void handleUpdatePlayingTeam(TeamId teamId)
        {
            s_log.DebugFormat("UpdatePlayingTeam Arrived: [Team Id: {0}]", teamId);
            UpdatePlayingTeamEvent?.Invoke(teamId);
        }

        private void handleApplyBoardCommands(BoardCommand[] commands)
        {
            s_log.DebugFormat("ApplyBoardCommandsArrived: [command: {0}]", string.Join<BoardCommand>(",", commands));
            BoardCommandsEvent?.Invoke(commands);
        }

        private void handleAskPromotion(PromotionRequest promotionRequest)
        {
            s_log.DebugFormat("AskPromotion Arrived: [request: {0}]", promotionRequest);
            AskPromotionEvent?.Invoke(promotionRequest);
        }

        private void handleUpdateTime(TeamId   teamId
                                    , TimeSpan timeLeft)
        {
            s_log.DebugFormat("UpdateTime Arrived: [team id: {0}| time left: {1}]", teamId, timeLeft);
            UpdateTimeEvent?.Invoke(teamId, timeLeft);
        }

        private void handleEndGame(EndGameReason reason)
        {
            s_log.DebugFormat("End Game Arrived: [End Game Reason: {0}]", reason);
            EndGameEvent?.Invoke(reason);
        }

        private void handleStartGame(GameConfig gameConfiguration)
        {
            s_log.DebugFormat("Start Game Arrived: [Game Configuration: {0}]", gameConfiguration);
            StartGameEvent?.Invoke(gameConfiguration);
        }
    }
}