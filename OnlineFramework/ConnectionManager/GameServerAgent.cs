using System.Reflection;
using Board;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using Tools;
using PromotionEventHandler = FrontCommon.PromotionEventHandler;

namespace Frameworks
{
    internal class GameServerAgent : IChessServerAgent
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event StartGameRequestHandler? StartGameRequestEvent;
        public event EndGameRequestHandler?   EndtGameRequestEvent;
        public event MoveRequestHandler?      MoveRequestEvent;
        public event PromotionEventHandler?   PromotionEvent;
        public event BoardReceivedHandler?    BoardReceivedEvent;
        public event TimeReceivedHandler?     TimeReceivedEvent;
        public event MoveRequestHandler?      CheckmateEvent;

        private HubConnection m_connection;

        public GameServerAgent(HubConnection connection)
        {
            m_connection = connection;
            registerToEvents();
        }

        public void Close() { }

        public async Task<bool> RequestGame()
        {
            return await m_connection.InvokeAsync<bool>("RequestGame");
        }

        public async Task SendEndGame()
        {
            s_log.Info("Game ended");
            await m_connection.StopAsync();
        }

        public async Task<MoveResult> SendMoveRequest(BoardPosition start
                                                    , BoardPosition end
                                                    , Guid          gameVersion)
        {
            return await m_connection.InvokeAsync<MoveResult>("MoveRequest", start, end, gameVersion);
        }

        public async Task<bool> PromoteRequest(BoardPosition         positionToPromote
                                             , IToolWrapperForServer tool
                                             , Guid                  gameVersion)
        {
            return await m_connection.InvokeAsync<bool>("PromoteRequest"
                                                      , positionToPromote
                                                      , tool
                                                      , gameVersion);
        }

        public Task<bool> IsMyTurn()
        {
            return m_connection.InvokeAsync<bool>("IsMyTurn");
        }

        public Task SendMessage(string msg)
        {
            throw new NotImplementedException();
        }

        private void registerToEvents()
        {
            m_connection.On<Team, Team, Guid>("StartGame", handleStartGameRequest);
            m_connection.On("EndGame", handleEndGameRequest);
            m_connection.On<Team, TimeSpan>("UpdateTime", handleTimeUpdate);
            m_connection.On<BoardState>("ForceAddToBoard", handleBoardReceived);
            m_connection.On<BoardPosition, BoardPosition>("Move",      handleMoveRequest);
            m_connection.On<BoardPosition, BoardPosition>("CheckMate", handleCheckMateEvent);
            m_connection.On<BoardPosition, IToolWrapperForServer>("PromoteTool", handlePromotion);
        }

        private void handlePromotion(BoardPosition         positionToPromote
                                   , IToolWrapperForServer toolWrapper)
        {
            PromotionEvent?.Invoke(positionToPromote, toolWrapper);
        }

        private void handleCheckMateEvent(BoardPosition start
                                        , BoardPosition end)
        {
            CheckmateEvent?.Invoke(start, end);
        }

        private void handleMoveRequest(BoardPosition start
                                     , BoardPosition end)
        {
            MoveRequestEvent?.Invoke(start, end);
        }

        private void handleBoardReceived(BoardState boardState)
        {
            BoardReceivedEvent?.Invoke(boardState);
        }

        private void handleTimeUpdate(Team     team
                                    , TimeSpan timeLeft)
        {
            TimeReceivedEvent?.Invoke(team, timeLeft);
        }

        private void handleEndGameRequest()
        {
            EndtGameRequestEvent?.Invoke();
        }

        private void handleStartGameRequest(Team localTeam
                                          , Team remoteTeam
                                          , Guid gameToken)
        {
            StartGameRequestEvent?.Invoke(localTeam, remoteTeam, gameToken);
        }
    }
}