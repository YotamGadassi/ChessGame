using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ChessHub : Hub
    {
        [HubMethodName("Move")]
        public async Task Move(int[] start, int[] end)
        {
            await Clients.Others.SendAsync("Move", start, end);
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine("Connected established");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine("Disconnected established");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
