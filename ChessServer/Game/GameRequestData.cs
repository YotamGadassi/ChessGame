using ChessServer.ChessPlayer;
using OnlineChess.Common;

namespace ChessServer.Game;

public class GameRequestData
{
    public GameRequestId GameRequestId { get; }

    public IServerChessPlayer Player { get; }

    public GameRequestData(GameRequestId      gameRequestId
                         , IServerChessPlayer player)
    {
        GameRequestId = gameRequestId;
        Player        = player;
    }
}