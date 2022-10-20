using System.Collections.Concurrent;
using System.Windows.Media;
using Global.DataStructures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace ChessServer3._0
{
    public class ServerState
    {
        private readonly UniqueQueue<PlayerObject>                   m_pendingPlayers      = new();
        private readonly ConcurrentDictionary<string, GameUnit>      m_groups              = new();
        private readonly ConcurrentDictionary<string, PlayerObject?> m_connectionIdToPlayer = new();
        public async Task onGameRequest(Hub          hub)
        {
            // TODO: handle error
            m_connectionIdToPlayer.TryGetValue(hub.Context.ConnectionId, out PlayerObject? player);
            
            if (m_pendingPlayers.TryDequeue(out PlayerObject otherPlayer))
            {
                GameUnit newGame = new(player, otherPlayer);
                m_groups[newGame.GroupName] = newGame;
                await Task.WhenAll(newGame.StartGame(hub),
                                   sendStartGame(hub, newGame));
                return;
            }

            bool isEnqueued = m_pendingPlayers.TryEnqueue(player);
            // sendEnteredToWaitingList(player, hub);
        }

        private async Task sendStartGame(Hub      hub
                                       , GameUnit game)
        {
            await
                Task.WhenAll(hub.Clients.Client(game.WhitePlayer1.ConnectionId).SendAsync("StartGame", game.WhitePlayer1.PlayersTeam, game.BlackPlayer2.PlayersTeam, game.CurrentGameVersion)
                            ,
                             hub.Clients.Client(game.BlackPlayer2.ConnectionId)
                                .SendAsync("StartGame", game.BlackPlayer2.PlayersTeam, game.WhitePlayer1.PlayersTeam, Guid.Empty));
        }

        public bool onConnection(Hub hub)
        {
            string name;
            if (hub.Context.GetHttpContext().Request.Query.TryGetValue("Name", out StringValues nameStrings))
            {
                name = nameStrings[0];
            }
            else
            {
                name = getRandomName();
            }

            PlayerObject player = new(hub.Context.ConnectionId, name);

            return m_connectionIdToPlayer.TryAdd(hub.Context.ConnectionId, player);
        }

        private string getRandomName()
        {
            return "Guest";
        }

        private void sendEnteredToWaitingList(PlayerObject player, Hub hub)
        {
            hub.Clients.Caller.SendAsync("EnteredWaitingList");
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

        public async Task onPlayerQuit(Hub hub)
        {
            string connectionId = hub.Context.ConnectionId;
            bool   isConnected  = m_connectionIdToPlayer.Remove(connectionId, out PlayerObject? player);
            if (false == isConnected)
            {
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
            await sendEndGame(hub, otherPlayer);
        }

        private async Task sendEndGame(Hub          hub
                                     , PlayerObject otherPlayer)
        {
            await hub.Clients.Client(otherPlayer.ConnectionId).SendAsync("EndGame");
        }
    }
}
