using ChessServer.ChessPlayer;
using OnlineChess.Common;

namespace ChessServer.Game;

public class GameRequestData
{
    public GameRequestId GameRequestId { get; }

    public PlayerData PlayerData { get; }

    public GameRequestData(GameRequestId gameRequestId
                         , PlayerData    playerData)
    {
        GameRequestId = gameRequestId;
        PlayerData    = playerData;
    }
}