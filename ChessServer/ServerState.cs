using System.Collections.Concurrent;
using Utils.DataStructures;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer
{
    public enum GameRequestResult
    {
        GameStarted = 1,
        CannotStartGame = 2,
        PlayerAddedToPendingList = 3,
        CannotAddPlayerToPendingList = 4
    }
    
    public interface IServerState
    {
        System.Threading.Tasks.Task<GameRequestResult> OnGameRequest(string connectionId);
        bool                    OnConnection(string  name,         string       connectionId);
        bool                    TryGetGame(string    connectionId, out GameUnit game);
        void                    EndGame(GameUnit     game);
        bool TryGetPlayer(string           connectionId
                        , out PlayerObject player);

        Task OnPlayerQuit(string connectionId);
    }

    public class ServerState : IServerState
    {
        private readonly IHubContext<ChessHub>                       m_hubContext;
        private readonly UniqueQueue<PlayerObject>                   m_pendingPlayers       = new();
        private readonly ConcurrentDictionary<string, GameUnit>      m_groups               = new();
        private readonly ConcurrentDictionary<string, PlayerObject?> m_connectionIdToPlayer = new();
        private readonly ILogger<ServerState>                        m_log;
        public ServerState(IHubContext<ChessHub> hubContext, ILogger<ServerState> logger)
        {
            m_hubContext = hubContext;
            m_log        = logger;
            m_log.LogInformation($"Server state has been created");
        }

        public async Task<GameRequestResult> OnGameRequest(string connectionId)
        {
            // TODO: handle error
            m_connectionIdToPlayer.TryGetValue(connectionId, out PlayerObject? player);
            
            if (m_pendingPlayers.TryDequeue(out PlayerObject otherPlayer))
            {
                GameUnit newGame = new(player, otherPlayer);
                m_groups[newGame.GroupName]    =  newGame;
                newGame.PlayerTimeChangedEvent += onPlayerOneSecElapsed;
                bool isGameStarted = await newGame.StartGame();
                if (isGameStarted)
                {
                    await Task.WhenAll(m_hubContext.Groups.AddToGroupAsync(newGame.WhitePlayer1.ConnectionId, newGame.GroupName),
                                       m_hubContext.Groups.AddToGroupAsync(newGame.BlackPlayer2.ConnectionId, newGame.GroupName));
                }
                return isGameStarted ? GameRequestResult.GameStarted : GameRequestResult.CannotStartGame;
            }

            bool isEnqueued = m_pendingPlayers.TryEnqueue(player);
            return isEnqueued
                       ? GameRequestResult.PlayerAddedToPendingList
                       : GameRequestResult.CannotAddPlayerToPendingList;
        }

        public bool OnConnection(string name, string connectionId)
        {
            PlayerObject player = new(connectionId, name);

            return m_connectionIdToPlayer.TryAdd(connectionId, player);
        }

        private void sendEnteredToWaitingList(PlayerObject player)
        {
            m_hubContext.Clients.Client(player.ConnectionId).SendAsync("EnteredWaitingList");
        }

        public bool TryGetGame(string connectionId, out GameUnit game)
        {
            bool isConnected = TryGetPlayer(connectionId, out PlayerObject player);
            game = null;
            if (false == isConnected)
            {
                return false;
            }

            game = player.GameUnit;
            return game != null;
        }

        public bool TryGetPlayer(string           connectionId
                               , out PlayerObject player)
        {
            return m_connectionIdToPlayer.TryGetValue(connectionId, out player);
        }

        public async Task OnPlayerQuit(string connectionId)
        {
            bool   isConnected  = m_connectionIdToPlayer.Remove(connectionId, out PlayerObject? player);
            if (false == isConnected)
            {
                m_log.LogError($"Error - Player quit although is not listed to server");
                return;
            }

            if (true == m_pendingPlayers.TryRemove(player))
            {
                m_log.LogInformation($"Player quit and removed from pending list");
                return;
            }

            GameUnit game           = player.GameUnit;
            bool     isPlayerInGame = null != game;
            if (false == isPlayerInGame)
            {
                return;
            }

            //TODO: handle error
            m_groups.Remove(game.GroupName, out _);
            PlayerObject otherPlayer = game.GetOtherPlayer(player);
            await sendEndGame(otherPlayer);
        }

        public void EndGame(GameUnit game)
        {
            PlayerObject player1 = game.WhitePlayer1;
            PlayerObject player2 = game.BlackPlayer2;

            
            bool isConnected = m_connectionIdToPlayer.Remove(player1.ConnectionId, out _);
            if (false == isConnected)
            {
                m_log.LogError($"Error - Player [{player1}] quit although is not listed to server");
            }
            
            isConnected = m_connectionIdToPlayer.Remove(player2.ConnectionId, out _);
            if (false == isConnected)
            {
                m_log.LogError($"Error - Player [{player2}] quit although is not listed to server");
            }

            if(false == m_groups.Remove(game.GroupName, out _))
            {
                m_log.LogError($"Error - Trying to end game [{game}] that does not exist");
            }
        }

        private async Task sendEndGame(PlayerObject otherPlayer)
        {
            await m_hubContext.Clients.Client(otherPlayer.ConnectionId).SendAsync("EndGame");
        }

        private void onPlayerOneSecElapsed(PlayerObject player, TimeSpan timeLeft)
        {
            string groupName = player.GameUnit.GroupName;
            m_hubContext.Clients.Group(groupName).SendAsync("UpdateTime", player.PlayersTeam, timeLeft);
        }
    }
}
