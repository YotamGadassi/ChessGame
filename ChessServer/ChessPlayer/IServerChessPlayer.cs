using Common.Chess;
using OnlineChess.Common;

namespace ChessServer.ChessPlayer;

public interface IServerChessPlayer : IChessClientApi
{
    public PlayerId PlayerId { get; }

    public string Name { get; }

    public ChessTeam ChessTeam { get; set; }
}