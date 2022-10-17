using System.Diagnostics;
using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ChessHub : Hub
    {
        private static readonly ServerState s_serverState = new ServerState();

        [HubMethodName("Move")]
        public async Task Move(BoardPosition start, BoardPosition end)
        {
            if (false == s_serverState.TryGetGroup(Context.ConnectionId, out string groupName))
            {
                //log
                return;
            }
            await Clients.OthersInGroup(groupName).SendAsync("Move", start, end);
        }

        public async Task Quit()
        {
            Debug.WriteLine($"Player quit: {Context.ConnectionId}");
            await s_serverState.RemovePlayer(Context.ConnectionId, this);
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Connected established: {Context.ConnectionId}");
            await s_serverState.HandleNewConnection(Context.ConnectionId, this);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine($"Disconnected established {Context.ConnectionId}");
            await s_serverState.RemovePlayer(Context.ConnectionId, this);
            await base.OnDisconnectedAsync(exception);  
        }
    }
}
    