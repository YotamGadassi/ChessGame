using ChessServer.ChessPlayer;

namespace ChessServer.Game;

public interface IGamesManager
{
    public GameRequestId SubmitGame(PlayerData playerData);

    public void CancelGameRequest(GameRequestId requestId);

    public GameId GetGameId(PlayerId playerId);

    public PlayerId[] GetPlayersId(GameId gameId);

    public GameUnit GetGameUnit(GameId gameId);

    public GameUnit RemoveGame(GameId gameId);

}