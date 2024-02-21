using ChessServer.ChessPlayer;
using ChessServer.Game;
using ChessServer.Users;
using Microsoft.AspNetCore.SignalR;
using OnlineChess.Common;

namespace ChessServer.ServerManager;

public class SignalRServerManager : IServerManager<string>
{
    private readonly IHubContext<ChessHub, IChessClientApi> m_hubContext;
    private readonly ILogger<SignalRServerManager> m_log;

    public SignalRServerManager(IHubContext<ChessHub, IChessClientApi> hubContext
                              , ILogger<SignalRServerManager> logger
                              , IUsersManager<string> usersManager
                              , IGamesManager gamesManager)
    {
        UsersManager = usersManager;
        GamesManager = gamesManager;
        m_hubContext = hubContext;
        m_log = logger;
        m_log.LogInformation($"Server state has been created");
    }

    public IUsersManager<string> UsersManager { get; }
    public IGamesManager GamesManager { get; }
    public IPlayersManager PlayersManager { get; }
}