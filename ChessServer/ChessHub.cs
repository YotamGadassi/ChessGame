using Board;
using ChessServer.ChessPlayer;
using ChessServer.Game;
using ChessServer.ServerManager;
using ChessServer.Users;
using Common;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using OnlineChess.Common;
using UserData = ChessServer.Users.UserData<string>;

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
            UserData           userData = await getUserData();
            m_serverState.UsersManager.RemoveUserAsync(connectionId);
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
        IGameUnit? gameUnit   = await getGameUnit(userData.UserId);
        gameUnit.EndGame();
        await m_serverState.GamesManager.RemoveGameAsync(gameUnit.Id);
    }

    public async Task AckGameReceive()
    {
        UserData           userData = await getUserData();
        IGameUnit?         game     = await m_serverState.GamesManager.GetGameAsync(userData.UserId);
        game.Init();
    }

    public async Task<MoveResult> SubmitMove(BoardPosition start
                                           , BoardPosition end)
    {
        UserData   userData   = await getUserData();
        IGameUnit? gameUnit   = await getGameUnit(userData.UserId);
        return gameUnit.Move(start, end);
    }

    public async Task<PromotionResult> SubmitPromote(PromotionRequest promotionRequest)
    {
        UserData   userData   = await getUserData();
        IGameUnit? gameUnit   = await getGameUnit(userData.UserId);
        return gameUnit.Promote(promotionRequest.Position, promotionRequest.ToolToPromote);
    }

    public async Task<TeamId> GetCurrentTeamTurn()
    {
        UserData   userData   = await getUserData();
        IGameUnit? gameUnit   = await getGameUnit(userData.UserId);
        return gameUnit.CurrentTeamId;
    }

    public Task SendMessage(string msg)
    {
        throw new NotImplementedException();
    }

    private UserData createUserDataForConnection()
    {
        string connectionId = Context.ConnectionId;
        
        UserUniqueId userUniqueId = UserUniqueId.NewUniqueId();
        //TODO: implement name mechanism
        return new UserData(connectionId, userUniqueId, string.Empty);
    }

    private IServerChessPlayer createPlayer(UserData userData)
    {
        PlayerId          playerId = PlayerId.NewPlayerId();
        ServerChessPlayer player   = new(userData.UserId, playerId, userData.UserName, m_hubContext, Context.ConnectionId);
        return player;
    }

    private async Task<UserData> getUserData()
    {
        string conncetionId = Context.ConnectionId;

        UserData userData = await m_serverState.UsersManager.GetUserDataAsync(conncetionId);
        if (null == userData)
        {
            throw new KeyNotFoundException($"User Data does not exist for connection id: {conncetionId}");
        }

        return userData;
    }

    private async Task<IGameUnit?> getGameUnit(UserUniqueId userId)
    {
        IGameUnit?    gameUnit   = await m_serverState.GamesManager.GetGameAsync(userId);
        if (null == gameUnit)
        {
            throw new KeyNotFoundException($"Game Unit does not exist for User Id: {userId}");
        }

        return gameUnit;
    }
}