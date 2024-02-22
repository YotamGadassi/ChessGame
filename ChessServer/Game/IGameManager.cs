using ChessServer.ChessPlayer;
using OnlineChess.Common;

namespace ChessServer.Game;

public interface IGamesManager
{
    public Task<GameRequestId> SubmitGameAsync(PlayerData playerData);

    public Task CancelGameRequestAsync(GameRequestId requestId);

    public Task<GameId> GetGameIdAsync(PlayerId playerId);

    public Task<PlayerId[]> GetPlayersIdAsync(GameId gameId);

    public Task<IGameUnit> GetGameUnitAsync(GameId gameId);

    public Task<IGameUnit> RemoveGameAsync(GameId gameId);

}