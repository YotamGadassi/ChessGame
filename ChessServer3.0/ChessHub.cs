using System.Diagnostics;
using Common.ChessBoardEventArgs;
using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class ChessHub : Hub
    {
        private readonly IServerState m_serverState;

        private readonly IHubContext<ChessHub> m_hubContext;
        public ChessHub(IServerState serverState)
        {
            m_serverState = serverState;
        }

        [HubMethodName("Move")]
        public async Task<bool> Move(BoardPosition start, BoardPosition end, Guid gameVersion)
        {
            if (false == m_serverState.TryGetGame(Context.ConnectionId, out GameUnit game))
            {
                // TODO: handle error
                return false;
            }
            MoveResult moveResult = game.Move( gameVersion,start,end);
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
            await m_serverState.OnPlayerQuit(Context.ConnectionId);
        }

        public async Task RequestGame()
        {
            await m_serverState.OnGameRequest(this);
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Connected established: {Context.ConnectionId}");
            string name = getName();
            m_serverState.OnConnection(name, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        private string getName()
        {
            return Context.GetHttpContext().Request.Query["Name"];
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine($"Disconnected established {Context.ConnectionId}");
            await m_serverState.OnPlayerQuit(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);  
        }

    }
}
    