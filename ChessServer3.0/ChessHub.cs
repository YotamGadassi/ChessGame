using System.Diagnostics;
using System.Net.NetworkInformation;
using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ChessHub : Hub
    {
        private static ServerState s_serverState = new ServerState();

        [HubMethodName("Move")]
        public async Task Move(BoardPosition start, BoardPosition end)
        {
            await Clients.Others.SendAsync("Move", start, end);
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine("Connected established");
            await s_serverState.HandleNewConnection(Context.ConnectionId, this);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine("Disconnected established");
            await s_serverState.HandleDisconnection(Context.ConnectionId, this);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
