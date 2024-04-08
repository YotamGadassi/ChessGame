using ChessServer.Users;
using Common.Chess;
using OnlineChess.Common;

namespace ChessServer.ChessPlayer;

public interface IServerChessPlayer : IChessClientApi
{
    public UserUniqueId UserUniqueId { get; }
    public PlayerId     PlayerId     { get; }

    public string Name { get; }

    public ChessTeam ChessTeam { get; set; }
}