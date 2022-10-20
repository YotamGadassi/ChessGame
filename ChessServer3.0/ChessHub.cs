using System.Diagnostics;
using Common.ChessBoardEventArgs;
using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ChessHub : Hub
    {
        private readonly ServerState m_serverState = new();

        [HubMethodName("Move")]
        public async Task<bool> Move(BoardPosition start, BoardPosition end, Guid gameVersion)
        {
            if (false == m_serverState.TryGetGame(Context.ConnectionId, out GameUnit game))
            {
                // TODO: handle error
                return false;
            }
            MoveResult moveResult = game.Move(this, gameVersion,start,end);
            switch (moveResult.Result)
            {
                case MoveResultEnum.ToolMoved:
                case MoveResultEnum.ToolKilled:
                {
                    if (false == m_serverState.TryGetPlayer(Context.ConnectionId, out PlayerObject player))
                    {
                        // TODO: Handle error
                        return false;
                    }

                    PlayerObject otherPlayer = game.GetOtherPlayer(player);
                    await Clients.Client(otherPlayer.ConnectionId).SendAsync("Move", start, end, game.CurrentGameVersion);
                    return true;
                }
            }

            return false;
        }

        public async Task QuitGame()
        {
            Debug.WriteLine($"Player quit: {Context.ConnectionId}");
            await m_serverState.onPlayerQuit(this);
        }

        public async Task RequestGame()
        {
            await m_serverState.onGameRequest(this);
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Connected established: {Context.ConnectionId}");
            m_serverState.onConnection(this);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine($"Disconnected established {Context.ConnectionId}");
            await m_serverState.onPlayerQuit(this);
            await base.OnDisconnectedAsync(exception);  
        }

    }
}
    