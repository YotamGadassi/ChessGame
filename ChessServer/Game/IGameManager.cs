using ChessServer.ChessPlayer;
using ChessServer.Users;
using OnlineChess.Common;

namespace ChessServer.Game;

public interface IGamesManager
{
    public Task<GameRequestId> SubmitGameRequestAsync(IServerChessPlayer player);

    public Task CancelGameRequestAsync(GameRequestId requestId);

    public Task<IGameUnit?> GetGameAsync(UserUniqueId playerId);

    public Task<IGameUnit?> RemoveGameAsync(GameId gameId);

}