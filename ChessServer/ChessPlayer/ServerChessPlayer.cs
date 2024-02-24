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

    public ServerChessPlayer(PlayerId   playerId
                            , string    name)
    {
        PlayerId  = playerId;
        Name      = name;
    }

    public Task        StartGame(GameConfig                gameConfig)
    {
        throw new NotImplementedException();
    }

    public Task EndGame(EndGameReason endGameReason)
    {
        throw new NotImplementedException();
    }

    public Task BoardCommands(BoardCommand[] boardCommands)
    {
        throw new NotImplementedException();
    }

    public Task AskPromote(BoardPosition positionToPromote)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTime(TeamId   teamId
                         , TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePlayingTeam(TeamId teamId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateToolsAndTeams(ToolAndTeamPair[] pairs)
    {
        throw new NotImplementedException();
    }
}