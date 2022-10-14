using System.Collections.Concurrent;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ServerState
    {
        private readonly ConcurrentQueue<string>                    m_pendingPlayers      = new();
        private          int                                        m_currentGameNumber   = 0;
        private readonly ConcurrentDictionary<string, List<string>> m_groupsToConnections = new();
        private readonly ConcurrentDictionary<string, string>       m_connectionsToGroup  = new();

        public async Task HandleNewConnection(string connectionId, Hub hub)
        {
            if (m_pendingPlayers.TryDequeue(out string connectionIdFromQueue))
            {
                string group = await createNewGroup(connectionId, connectionIdFromQueue, hub.Groups);
                await startGame(connectionId, connectionIdFromQueue, hub.Clients);
                return;
            }
            m_pendingPlayers.Enqueue(connectionId);
        }

        private async Task startGame(string connectionId, string connectionIdFromQueue, IHubCallerClients clients)
        {
            await clients.Client(connectionId).SendCoreAsync("StartGame", new object?[] { Colors.White });
            await clients.Client(connectionIdFromQueue).SendCoreAsync("StartGame", new object?[] { Colors.Black });
        }

        private async Task<string> createNewGroup(string connectionId, string connectionIdFromQueue, IGroupManager groupManager)
        {
            int    currentNumber = Interlocked.Increment(ref m_currentGameNumber);
            string currentGroup  = currentNumber.ToString();
            await groupManager.AddToGroupAsync(currentGroup, connectionId);
            await groupManager.AddToGroupAsync(currentGroup, connectionIdFromQueue);
            List<string> groupsConnections = new List<string>() { connectionId, connectionIdFromQueue };
            foreach (string groupsConnection in groupsConnections)
            {
                m_connectionsToGroup.TryAdd(groupsConnection, currentGroup);
            }

            m_groupsToConnections.TryAdd(currentGroup, groupsConnections);

            return currentGroup;
        }

        public async Task HandleDisconnection(string connectionId, Hub hub)
        {
            if (tryRemoveFromQueue(connectionId))
            {
                return;
            }

            m_connectionsToGroup.TryRemove(connectionId, out string groupName);
            m_groupsToConnections.TryRemove(groupName, out List<string>? conncetions);
            await hub.Clients.OthersInGroup(groupName).SendCoreAsync("PlayerQuit",null);
            foreach (var connection in conncetions)
            {
                await hub.Groups.RemoveFromGroupAsync(connection, groupName);
                m_connectionsToGroup.TryRemove(connection , out _);
            }
        }

        private bool tryRemoveFromQueue(string connectionId)
        {
            return false;
        }
    }
}
