using System.Collections.Concurrent;
using System.Windows.Media;
using Global.DataStructures;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ServerState
    {
        private readonly UniqueQueue<string>                        m_pendingPlayers      = new();
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
            m_pendingPlayers.TryEnqueue(connectionId);
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
            await groupManager.AddToGroupAsync(connectionId, currentGroup);
            await groupManager.AddToGroupAsync(connectionIdFromQueue, currentGroup);
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
            foreach (string connection in conncetions)
            {
                await hub.Groups.RemoveFromGroupAsync(connection, groupName);
                m_connectionsToGroup.TryRemove(connection, out _);
            }
        }

        private bool tryRemoveFromQueue(string connectionId)
        {
            return m_pendingPlayers.TryRemove(connectionId);
        }

        public bool TryGetGroup(string connectionId, out string groupName)
        {
            return m_connectionsToGroup.TryGetValue(connectionId, out groupName);
        }
    }
}
