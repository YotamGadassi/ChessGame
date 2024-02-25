using Board;
using ChessServer.ChessPlayer;
using ChessServer.Game;
using ChessServer.ServerManager;
using ChessServer.Users;
using Common;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using OnlineChess.Common;
using Tools;

namespace ChessServer;

public class ChessHub : Hub<IChessClientApi>, IChessServerApi
{
    private readonly IServerManager<string>                 m_serverState;
    private readonly ILogger<ChessHub>                      m_log;
    private readonly IHubContext<ChessHub, IChessClientApi> m_hubContext;

    public ChessHub(IServerManager<string>                 serverState
                  , ILogger<ChessHub>                      logger
                  , IHubContext<ChessHub, IChessClientApi> hubContext)
    {
        m_serverState = serverState;
        m_log         = logger;
        m_hubContext = hubContext;
    }

    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;

        m_log.LogInformation($"Connected established: {connectionId}");

        UserData userData = createUserDataForConnection();
        await m_serverState.UsersManager.AddNewUserAsync(connectionId, userData);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;

        m_log.LogInformation($"Disconnected established {connectionId}");

        try
        {
            await SubmitGameWithdraw();
        }
        catch (KeyNotFoundException e)
        {
            m_log.LogError("");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<GameRequestResult> SubmitGameRequest(GameRequest gameRequest)
    {
        UserData   userData   = await getUserData();
        IServerChessPlayer player = createPlayer(userData);

        GameRequestId gameRequestId = await m_serverState.GamesManager.SubmitGameRequestAsync(player);
        return new GameRequestResult(false, gameRequestId);
    }

    public Task CancelGameRequest(GameRequestId gameRequestId)
    {
        return m_serverState.GamesManager.CancelGameRequestAsync(gameRequestId);
    }

    public async Task SubmitGameWithdraw()
    {
        UserData   userData   = await getUserData();
        IServerChessPlayer player = await getPlayer(userData.UserId);
        IGameUnit? gameUnit   = await getGameUnit(player.PlayerId);
        gameUnit.EndGame(player.PlayerId, EndGameReason.Withdraw);
        await m_serverState.GamesManager.RemoveGameAsync(gameUnit.Id);
    }

    public async Task Init()
    {
        UserData           userData = await getUserData();
        IServerChessPlayer player   = await getPlayer(userData.UserId);
        IGameUnit?         game     = await m_serverState.GamesManager.GetGameAsync(player.PlayerId);
        game.Init();
    }

    private async Task<IServerChessPlayer> getPlayer(UserUniqueId id)
    {
        return await m_serverState.PlayersManager.GetPlayerDataAsync(id);
    }

    public async Task<MoveResult> SubmitMove(BoardPosition start
                                           , BoardPosition end)
    {
        UserData   userData   = await getUserData();
        IServerChessPlayer player = await getPlayer(userData.UserId);
        IGameUnit? gameUnit   = await getGameUnit(player.PlayerId);
        return gameUnit.Move(start, end);
    }

    public async Task<PromotionResult> SubmitPromote(BoardPosition positionToPromote
                                             , ITool         tool)
    {
        UserData   userData   = await getUserData();
        IServerChessPlayer player = await getPlayer(userData.UserId);
        IGameUnit? gameUnit   = await getGameUnit(player.PlayerId);
        return gameUnit.Promote(positionToPromote, tool);
    }

    public async Task<TeamId> GetCurrentTeamTurn()
    {
        UserData   userData   = await getUserData();
        IServerChessPlayer player = await getPlayer(userData.UserId);
        IGameUnit? gameUnit   = await getGameUnit(player.PlayerId);
        return gameUnit.CurrentTeamId;
    }

    public Task SendMessage(string msg)
    {
        throw new NotImplementedException();
    }

    private UserData createUserDataForConnection()
    {
        UserUniqueId userUniqueId = UserUniqueId.NewUniqueId();
        //TODO: implement name mechanism
        return new UserData(userUniqueId, string.Empty);
    }

    private IServerChessPlayer createPlayer(UserData userData)
    {
        PlayerId          playerId = PlayerId.NewPlayerId();
        ServerChessPlayer player   = new(playerId, userData.UserName, m_hubContext, Context.ConnectionId);
        m_serverState.PlayersManager.AddNewPlayerAsync(userData.UserId, player);
        return player;
    }

    private async Task<UserData> getUserData()
    {
        string conncetionId = Context.ConnectionId;

        UserData userData = await m_serverState.UsersManager.GetUserDataAsync(conncetionId);
        if (null == userData)
        {
            throw new KeyNotFoundException(string.Format("User Data does not exist for connection id: {0}", conncetionId));
        }

        return userData;
    }

    private async Task<IGameUnit?> getGameUnit(PlayerId playerId)
    {
        IGameUnit?    gameUnit   = await m_serverState.GamesManager.GetGameAsync(playerId);
        if (null == gameUnit)
        {
            throw new KeyNotFoundException(string.Format("Game Unit does not exist for Player Id: {0}", playerId));
        }

        return gameUnit;
    }
}