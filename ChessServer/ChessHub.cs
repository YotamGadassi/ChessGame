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
    private readonly IServerManager<string> m_serverState;
    private readonly ILogger<ChessHub>      m_log;

    public ChessHub(IServerManager<string> serverState
                  , ILogger<ChessHub>      logger)
    {
        m_serverState = serverState;
        m_log         = logger;
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
        PlayerData playerData = createPlayerData(userData);

        GameRequestId gameRequestId = await m_serverState.GamesManager.SubmitGameRequestAsync(playerData);
        return new GameRequestResult(false, gameRequestId);
    }

    public Task CancelGameRequest(GameRequestId gameRequestId)
    {
        return m_serverState.GamesManager.CancelGameRequestAsync(gameRequestId);
    }

    public async Task SubmitGameWithdraw()
    {
        UserData   userData   = await getUserData();
        PlayerData playerData = await getPlayerData(userData);
        IGameUnit? gameUnit   = await getGameUnit(playerData);
        gameUnit.EndGame(playerData.PlayerId, EndGameReason.Withdraw);
        await m_serverState.GamesManager.RemoveGameAsync(gameUnit.Id);
    }

    public async Task<MoveResult> SubmitMove(BoardPosition start
                                     , BoardPosition end)
    {
        UserData   userData   = await getUserData();
        PlayerData playerData = await getPlayerData(userData);
        IGameUnit? gameUnit   = await getGameUnit(playerData);
        return gameUnit.Move(start, end);
    }

    public async Task<PromotionResult> SubmitPromote(BoardPosition positionToPromote
                                             , ITool         tool)
    {
        UserData   userData   = await getUserData();
        PlayerData playerData = await getPlayerData(userData);
        IGameUnit? gameUnit   = await getGameUnit(playerData);
        return gameUnit.Promote(positionToPromote, tool);
    }

    public async Task<TeamId> GetCurrentTeamTurn()
    {
        UserData   userData   = await getUserData();
        PlayerData playerData = await getPlayerData(userData);
        IGameUnit? gameUnit   = await getGameUnit(playerData);
        return gameUnit.CurrentTeamId;
    }

    public Task SendMessage(string msg)
    {
        throw new NotImplementedException();
    }

    private UserData createUserDataForConnection()
    {
        UserUniqueId userUniqueId = UserUniqueId.NewUniqueId();
        //TODO: implement name mechanisem
        return new UserData(userUniqueId, string.Empty);
    }

    private PlayerData createPlayerData(UserData userData)
    {
        PlayerId playerId = PlayerId.NewPlayerId();
        return new PlayerData(playerId, userData.UserName);
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
    private async Task<PlayerData> getPlayerData(UserData userData)
    {
        PlayerData playerData = await m_serverState.PlayersManager.GetPlayerDataAsync(userData.UserId);
        if (null == playerData)
        {
            throw new KeyNotFoundException(string.Format("Player Data does not exist for User Data: {0}", userData));
        }

        return playerData;
    }
    private async Task<IGameUnit?> getGameUnit(PlayerData playerData)
    {
        IGameUnit?    gameUnit   = await m_serverState.GamesManager.GetGameAsync(playerData.PlayerId);
        if (null == gameUnit)
        {
            throw new KeyNotFoundException(string.Format("Game Unit does not exist for Player Data: {0}", playerData));
        }

        return gameUnit;
    }
}