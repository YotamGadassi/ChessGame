using Board;
using Common;

namespace OnlineChess.ConnectionManager;

public class OnlineChessGameConfiguration
{
    public TeamWithTimer     LocalTeam     { get; }
    public TeamWithTimer     RemoteTeam    { get; }
    public Team              FirstTeamTurn { get; }
    public BoardState        BoardState    { get; }

    public OnlineChessGameConfiguration(TeamWithTimer  localTeam
                                       , TeamWithTimer remoteTeam
                                       , Team          firstTeamTurn
                                       , BoardState    boardState)
    {
        LocalTeam     = localTeam;
        RemoteTeam    = remoteTeam;
        FirstTeamTurn = firstTeamTurn;
        BoardState    = boardState;
    }
}