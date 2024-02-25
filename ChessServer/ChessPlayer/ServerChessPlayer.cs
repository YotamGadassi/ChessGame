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
    private IChessClientApi client => m_hubContext.Clients.Client(m_connectionId);
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

    public Task StartGame(GameConfig gameConfig) => client.StartGame(gameConfig);

    public Task EndGame(EndGameReason endGameReason) => client.EndGame(endGameReason);

    public Task ApplyBoardCommands(BoardCommand[] boardCommands) => client.ApplyBoardCommands(boardCommands);

    public Task AskPromote(BoardPosition positionToPromote) => client.AskPromote(positionToPromote);

    public Task UpdateTime(TeamId   teamId
                         , TimeSpan timeSpan) => client.UpdateTime(teamId, timeSpan);

    public Task UpdatePlayingTeam(TeamId teamId) => client.UpdatePlayingTeam(teamId);

    public Task UpdateToolsAndTeams(ToolAndTeamPair[] pairs) => client.UpdateToolsAndTeams(pairs);
}