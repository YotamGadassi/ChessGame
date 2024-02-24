using Board;
using Common;
using Common.Chess;
using OnlineChess.Common;
using Tools;

namespace ChessServer.ChessPlayer;

public class ServerChessPlayer : IChessClientApi
{
    public PlayerId PlayerId { get; }

    public string Name { get; }

    public ChessTeam ChessTeam { get; set; }

    public void        StartGame(GameConfig                gameConfig)
    {
        throw new NotImplementedException();
    }

    public void        EndGame(EndGameReason               endGameReason)
    {
        throw new NotImplementedException();
    }

    public void        BoardCommands(BoardCommand[] boardCommands)
    {
        throw new NotImplementedException();
    }

    public Task<ITool> AskPromote(BoardPosition positionToPromote)
    {
        throw new NotImplementedException();
    }

    public void UpdateTime(TeamId   teamId
                         , TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }

    public void UpdatePlayingTeam(TeamId teamId)
    {
        throw new NotImplementedException();
    }

    public void UpdateToolsAndTeams(ToolAndTeamPair[] pairs)
    {
        throw new NotImplementedException();
    }
}