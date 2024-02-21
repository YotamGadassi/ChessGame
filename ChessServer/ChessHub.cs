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
using GameRequestId = OnlineChess.Common.GameRequestId;
using GameRequestResult = OnlineChess.Common.GameRequestResult;

namespace ChessServer;

public class ChessHub : Hub<IChessClientApi>, IChessServerApi
{
    private readonly IServerManager<string>      m_serverState;
    private readonly ILogger<ChessHub> m_log;

    public ChessHub(IServerManager<string>      serverState
                  , ILogger<ChessHub> logger)
    {
        m_serverState = serverState;
        m_log         = logger;
    }

    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;

        m_log.LogInformation($"Connected established: {connectionId}");
        UserData userData = getUserDataForConnection();
        m_serverState.UsersManager.AddNewUser(connectionId, userData);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;

        m_log.LogInformation( $"Disconnected established {connectionId}");
        UserData userData = m_serverState.UsersManager.RemoveUser(connectionId);
        if (null != userData)
        {
            PlayerData playerData = m_serverState.PlayersManager.RemovePlayer(userData.UserId);
            if (null != playerData)
            {
                GameId gameId = m_serverState.GamesManager.GetGameId(playerData.PlayerId);
                if (null != gameId)
                {
                    GameUnit gameUnit = m_serverState.GamesManager.RemoveGame(gameId);
                    //TODO: End Game!
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    private UserData getUserDataForConnection()
    {
        //TODO: implement
        throw new NotImplementedException();
    }

    private bool tryGetGameUnit(string       connectionId
                              , out GameUnit gameUnit)
    {
        gameUnit = null;
        UserData userData = m_serverState.UsersManager.GetUserData(connectionId);
        if (null != userData)
        {
            PlayerData playerData = m_serverState.PlayersManager.GetPlayerData(userData.UserId);
            if (null != playerData)
            {
                GameId gameId = m_serverState.GamesManager.GetGameId(playerData.PlayerId);
                if (null != gameId)
                {
                    gameUnit = m_serverState.GamesManager.GetGameUnit(gameId);
                }
            }
        }

        return gameUnit != null;
    }

    public Task<GameRequestResult> SubmitGameRequest(GameRequest   gameRequest)
    {
        throw new NotImplementedException();
    }

    public Task CancelGameRequest(GameRequestId gameRequestId)
    {
        throw new NotImplementedException();
    }

    public Task SubmitGameWithdraw()
    {
        throw new NotImplementedException();
    }

    public Task<MoveResult> SubmitMove(BoardPosition start
                                     , BoardPosition end)
    {
        throw new NotImplementedException();
    }

    public Task<PromotionResult> SubmitPromote(BoardPosition positionToPromote
                                             , ITool         tool)
    {
        throw new NotImplementedException();
    }

    public Task<TeamId> GetCurrentTeamTurn()
    {
        throw new NotImplementedException();
    }

    public Task SendMessage(string msg)
    {
        throw new NotImplementedException();
    }
}