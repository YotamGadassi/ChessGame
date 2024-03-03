using ChessServer.ChessPlayer;
using ChessServer.Game;
using ChessServer.Users;

namespace ChessServer.ServerManager;

public class SignalRServerFacade : IServerFacade<string>
{
    private readonly ILogger<SignalRServerFacade> m_log;

    public SignalRServerFacade(ILogger<SignalRServerFacade> logger)
    {
        UsersManager   = new SignalRUsersManager();
        GamesManager   = new GameManager(logger);
        PlayersManager = new PlayersManager();
        m_log          = logger;
        m_log.LogInformation($"Server state has been created");
    }

    public IUsersManager<string> UsersManager { get; }
    public IGamesManager GamesManager { get; }
    public IPlayersManager PlayersManager { get; }
}