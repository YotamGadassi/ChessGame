using Board;

namespace ChessServer3._0.ClientInterface;

public class ServerChessGameConfiguration
{
    public ServerTeamWithTimer LocalTeam  { get; }
    public ServerTeamWithTimer RemoteTeam { get; }

    public Guid FirstTeamId { get; }

    public BoardState BoardState { get; }

    public ServerChessGameConfiguration(ServerTeamWithTimer  localTeam
                                       , ServerTeamWithTimer remoteTeam
                                       , Guid                firstTeamId
                                       , BoardState          boardState)
    {
        LocalTeam   = localTeam;
        RemoteTeam  = remoteTeam;
        FirstTeamId = firstTeamId;
        BoardState  = boardState;
    }
}