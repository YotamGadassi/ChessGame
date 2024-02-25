using Board;
using Common;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using OnlineChess.Common;
using Tools;

namespace ChessServer.ChessPlayer;

public class ServerChessPlayer : IServerChessPlayer
{
    public PlayerId PlayerId { get; }

    public string Name { get; }

    public ChessTeam ChessTeam { get; set; }

    private IHubContext<ChessHub, IChessClientApi> m_hubContext;
    private string                                 m_connectionId;

    public ServerChessPlayer(PlayerId                                playerId
                           , string                                  name
                            , IHubContext<ChessHub, IChessClientApi> hubContext
                            , string                                 connectionId)
    {
        PlayerId       = playerId;
        Name           = name;
        m_hubContext   = hubContext;
        m_connectionId = connectionId;
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