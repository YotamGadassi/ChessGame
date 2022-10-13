using ChessBoard.ChessBoardEventArgs;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ChessHub : Hub
    {
        public async Task Move(ToolMovedEventArgs args)
        {
            await Clients.Others.SendAsync("Move", args.InitialPosition, args.EndPosition);
        }
    }
}
