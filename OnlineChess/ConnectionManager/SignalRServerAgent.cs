using System.Reflection;
using Board;
using Common.Chess;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
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
        public event TimeReceivedHandler?  TimeReceivedEvent;
        public event SwitchTeamHandler?    SwitchTeamEvent;

        private HubConnection m_connection;

        public SignalRServerAgent(HubConnection connection)
        {
            m_connection = connection;
            registerToEvents();
        }

        public async Task<bool> RequestGame(string userName)
        {
            s_log.Info("Game request sent to server");
            return await m_connection.InvokeAsync<bool>("RequestGame", userName);
        }

        public async Task WithdrawGame()
        {
            s_log.Info("Game withdraw sent to server");
            await m_connection.InvokeAsync("WithdrawGame");
        }

        public async Task<MoveResult> RequestMove(BoardPosition start
                                                , BoardPosition end)
        {
            return await m_connection.InvokeAsync<MoveResult>("RequestMove", start, end);
        }

        public async Task<PromotionResult> RequestPromote(BoardPosition         positionToPromote
                                             , IToolWrapperForServer tool)
        {
            return await m_connection.InvokeAsync<PromotionResult>("RequestPromote"
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
            m_connection.On<Guid, TimeSpan>("UpdateTime", handleTimeUpdate);
            m_connection.On<BoardPosition, ITool>("PromoteTool", handlePromotion);
            m_connection.On<BoardCommand[]>("BoardCommands", handleBoardCommands);
            m_connection.On<Guid>("SwitchTeam", handleTeamSwitch);
        }

        private void handleTeamSwitch(Guid currentTeamId)
        {
            SwitchTeamEvent?.Invoke(currentTeamId);
        }

        private void handleBoardCommands(BoardCommand[] commands)
        {
            BoardCommandsEvent?.Invoke(commands);
        }

        private Task<ITool> handlePromotion(BoardPosition positionToPromote)
        {
            return PromotionEvent?.Invoke(positionToPromote);
        }

        private void handleTimeUpdate(Guid     teamId
                                    , TimeSpan timeLeft)
        {
            TimeReceivedEvent?.Invoke(teamId, timeLeft);
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