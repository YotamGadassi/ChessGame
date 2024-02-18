using System.Reflection;
using Board;
using Common;
using Common.Chess;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using OnlineChess.Common;
using Tools;

namespace OnlineChess.ConnectionManager
{
    public class SignalRServerAgent : IChessServerAgent
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event StartGameHandler?     StartGameEvent;
        public event EndGameHandler?       EndGameEvent;
        public event BoardCommandsHandler? BoardCommandsEvent;
        public event PromotionHandler?     PromotionEvent;
        public event UpdateTimerHandler?  UpdateTimeEvent;
        public event UpdatePlayingTeamHandler?    UpdatePlayingTeamEvent;

        private readonly HubConnection m_connection;

        public SignalRServerAgent(HubConnection connection)
        {
            m_connection = connection;
            registerToEvents();
        }

        public async Task<GameRequestResult> SubmitGameRequest(GameRequest gameRequest)
        {
            s_log.InfoFormat("Game request sent to server: {0}", gameRequest);
            return await m_connection.InvokeAsync<GameRequestResult>("SubmitGameRequest", gameRequest.UserName);
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

        public async Task<MoveResult> SubmitMove(BoardPosition start
                                                , BoardPosition end)
        {
            s_log.Info($"Submit Move sent to server: [start:{start} | end:{end}]");
            return await m_connection.InvokeAsync<MoveResult>("SubmitMove", start, end);
        }

        public async Task<PromotionResult> SubmitPromote(BoardPosition         positionToPromote
                                                       , IToolWrapperForServer tool)
        {
            s_log.Info($"Submit Promote sent to server: [Position: {positionToPromote} | Tool: {tool}]");
            return await m_connection.InvokeAsync<PromotionResult>("SubmitPromote"
                                                                 , positionToPromote
                                                                 , tool);
        }

        public Task<bool> IsMyTurn()
        {
            //TODO: consider remove
            return m_connection.InvokeAsync<bool>("IsMyTurn");
        }

        public Task SendMessage(string msg)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        private void registerToEvents()
        {
            m_connection.On<OnlineChessGameConfiguration>("StartGame", handleStartGameRequest);
            m_connection.On<EndGameReason>("EndGame", handleEndGameRequest);
            m_connection.On<TeamId, TimeSpan>("UpdateTime", handleTimeUpdate);
            m_connection.On<BoardPosition, ITool>("AskPromotion", handlePromotion);
            m_connection.On<BoardCommand[]>("ApplyBoardCommands", handleBoardCommands);
            m_connection.On<TeamId>("UpdatePlayingTeam", handleTeamSwitch);
        }

        private void handleTeamSwitch(TeamId currentTeamId)
        {
            UpdatePlayingTeamEvent?.Invoke(currentTeamId);
        }

        private void handleBoardCommands(BoardCommand[] commands)
        {
            BoardCommandsEvent?.Invoke(commands);
        }

        private Task<ITool> handlePromotion(BoardPosition positionToPromote)
        {
            return PromotionEvent?.Invoke(positionToPromote);
        }

        private void handleTimeUpdate(TeamId     teamId
                                    , TimeSpan timeLeft)
        {
            UpdateTimeEvent?.Invoke(teamId, timeLeft);
        }

        private void handleEndGameRequest(EndGameReason reason)
        {
            EndGameEvent?.Invoke(reason);
        }

        private void handleStartGameRequest(OnlineChessGameConfiguration gameConfiguration)
        {
            StartGameEvent?.Invoke(gameConfiguration);
        }
    }
}