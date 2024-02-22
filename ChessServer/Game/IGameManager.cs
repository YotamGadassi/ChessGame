using ChessServer.ChessPlayer;
using OnlineChess.Common;

namespace ChessServer.Game;

public interface IGamesManager
{
    public Task<GameRequestId> SubmitGameRequestAsync(PlayerData playerData);

    public Task CancelGameRequestAsync(GameRequestId requestId);

    public Task<IGameUnit?> GetGameAsync(PlayerId playerId);

    public Task<IGameUnit> RemoveGameAsync(GameId gameId);

}