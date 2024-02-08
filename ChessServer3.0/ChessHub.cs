using Board;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using OnlineChess;
using Tools;
using OnlineChess.ConnectionManager;
using ChessServer3._0.Game;

namespace ChessServer3._0;

public class ChessHub : Hub, IChessServerAPI
{
    private readonly IServerState      m_serverState;
    private readonly ILogger<ChessHub> m_log;

    public ChessHub(IServerState      serverState
                  , ILogger<ChessHub> logger)
    {
        m_serverState = serverState;
        m_log         = logger;
    }

    public async Task<UserData> SignIn(SignInRequest request)
    {

    }

    public async Task SignOut(UserData userData)
    {

    }

    public async Task<bool> SubmitGameRequest(string playerName)
    {
        GameRequestResult result = await m_serverState.OnGameRequest(Context.ConnectionId);
        m_log.LogInformation($"Request Game From ConnectionId [{Context.ConnectionId}] Result: [{result}]");
        switch (result)
        {
            case GameRequestResult.GameStarted:
            {
                await sendStartGameToGroup();
                return true;
            }
            case GameRequestResult.PlayerAddedToPendingList:
            {
                return true;
            }
            case GameRequestResult.CannotStartGame:
            case GameRequestResult.CannotAddPlayerToPendingList:
            default:
            {
                return false;
            }
        }
    }

    public Task<bool> IsMyTurn()
    {
        string connectionId = Context.ConnectionId;
        if (false == m_serverState.TryGetGame(connectionId, out GameUnit game))
        {
            m_log.LogError($"Connection Id [{connectionId}] is not subscribed to any game");
            throw new InvalidOperationException("Move request is not authorized");
        }

        if (false == m_serverState.TryGetPlayer(connectionId, out PlayerObject player))
        {
            m_log.LogError($"Connection Id [{connectionId}] is not attached to a player object");
            throw new InvalidOperationException("Move request is not authorized");
        }

        return new Task<bool>(() => game.IsPlayerTurn(player));
    }

    public Task SendMessage(string msg)
    {
        throw new NotImplementedException();
    }

    public async Task<MoveResult> MoveTool(BoardPosition start
                                            , BoardPosition end)
    {
        string connectionId = Context.ConnectionId;
        if (false == m_serverState.TryGetGame(connectionId, out GameUnit game))
        {
            m_log.LogError($"Connection Id [{connectionId}] is not subscribed to any game");
            throw new InvalidOperationException("Move request is not authorized");
        }

        MoveResult     moveResult = game.Move(start, end);
        MoveResultEnum resultEnum = moveResult.Result;

        if (resultEnum.HasFlag(MoveResultEnum.ToolMoved))
        {
            if (false == m_serverState.TryGetPlayer(connectionId, out PlayerObject player))
            {
                m_log.LogError($"Connection Id [{connectionId}] is not attached to a player object");
                throw new InvalidOperationException("Move request is not authorized");
            }

            PlayerObject otherPlayer = game.GetOtherPlayer(player);
            if (resultEnum.HasFlag(MoveResultEnum.CheckMate))
            {
                
                await Clients.Client(otherPlayer.ConnectionId)
                             .SendAsync("CheckMate",
                                        start,
                                        end);
                m_log.LogInformation($"CheckMate has occurred from {start} to {end}");
                m_serverState.EndGame(game);
                return moveResult;
            }

            if (resultEnum.HasFlag(MoveResultEnum.NeedPromotion))
            {
                m_log.LogInformation($"NeedPromotion has occurred from {start} to {end}");
                await Clients.Client(otherPlayer.ConnectionId).SendAsync("Move", start, end);
                return moveResult;
            }

            await Clients.Client(otherPlayer.ConnectionId).SendAsync("Move", start, end);
        }
        else if(resultEnum.HasFlag(MoveResultEnum.NoChangeOccurred))
        {
            m_log.LogInformation($"Move request [{moveResult.InitialPosition} -> {moveResult.EndPosition}] is illegal");
        }
        else
        {
            m_log.LogError($"Move result flag not identified");
        }

        return moveResult;
    }

    public async Task<PromotionResult> PromoteTool(BoardPosition         position
                                         , IToolWrapperForServer toolWrapper)
    {
        ITool  tool         = toolWrapper.Tool;
        string connectionId = Context.ConnectionId;
        if (false == m_serverState.TryGetGame(connectionId, out GameUnit game))
        {
            m_log.LogError($"Connection Id [{connectionId}] is not subscribed to any game");
            throw new InvalidOperationException("Move request is not authorized");
        }

        if (false == m_serverState.TryGetPlayer(connectionId, out PlayerObject player))
        {
            m_log.LogError($"Connection Id [{connectionId}] is not attached to a player object");
            throw new InvalidOperationException("Move request is not authorized");
        }

        PlayerObject otherPlayer = game.GetOtherPlayer(player);

        bool isPromoted = game.Promote(position, tool);
        if (false == isPromoted)
        {
            return false;
        }

        await Clients.Client(otherPlayer.ConnectionId).SendAsync("PromoteTool", position, new IToolWrapperForServer(tool));

        return true;
    }
    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;

        m_log.LogInformation($"Connected established: {connectionId}");
        string name = getNameForConnection();
        m_serverState.OnConnection(name, connectionId);
        await base.OnConnectedAsync();
    }

    public async Task WithdrawGame()
    {
        m_log.LogInformation($"Player quit: {Context.ConnectionId}");
        await m_serverState.OnPlayerQuit(Context.ConnectionId);
    }

    private async Task sendStartGameToGroup()
    {
        string connectionId = Context.ConnectionId;
        bool   isGameExists        = m_serverState.TryGetGame(connectionId, out GameUnit game);
        if (false == isGameExists)
        {
            m_log.LogError($"Connection Id [{connectionId}] is not subscribed to any game");
            return;
        }

        m_log.LogInformation($"Sending start game to: [{game.WhitePlayer1}] & [{game.BlackPlayer2}]");
        BoardState gameBoard = game.GetBoardState();
        await
            Task.WhenAll(Clients.Client(game.WhitePlayer1.ConnectionId)
                                .SendAsync("StartGame",
                                           game.WhitePlayer1.PlayersTeam,
                                           game.BlackPlayer2.PlayersTeam,
                                           game.GameToken)
                        ,
                         Clients.Client(game.BlackPlayer2.ConnectionId)
                                .SendAsync("StartGame",
                                           game.BlackPlayer2.PlayersTeam,
                                           game.WhitePlayer1.PlayersTeam,
                                           game.GameToken),
                         Clients.Groups(game.GroupName).SendAsync("ForceAddToBoard", gameBoard));
    }


    private string getNameForConnection()
    {
        return Context.GetHttpContext()?.Request.Query["Name"] ?? string.Empty;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;

        m_log.LogInformation( $"Disconnected established {connectionId}");
        await m_serverState.OnPlayerQuit(connectionId);
        await base.OnDisconnectedAsync(exception);
    }
}