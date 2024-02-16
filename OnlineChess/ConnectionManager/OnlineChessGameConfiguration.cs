using Board;
using Common;

namespace OnlineChess.ConnectionManager;

public class OnlineChessGameConfiguration
{
    public OnlineChessTeam     LocalTeam     { get; }
    public OnlineChessTeam     RemoteTeam    { get; }
    public Team              FirstTeamTurn { get; }
    public BoardState        BoardState    { get; }

    public OnlineChessGameConfiguration(OnlineChessTeam  localTeam
                                       , OnlineChessTeam remoteTeam
                                       , Team          firstTeamTurn
                                       , BoardState    boardState)
    {
        LocalTeam     = localTeam;
        RemoteTeam    = remoteTeam;
        FirstTeamTurn = firstTeamTurn;
        BoardState    = boardState;
    }
}