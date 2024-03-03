using ChessServer.Users;
using Common;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using OnlineChess.Common;

namespace ChessServer.ChessPlayer;

public class ServerChessPlayer : IServerChessPlayer
{
    public UserUniqueId UserUniqueId { get; }
    public PlayerId     PlayerId     { get; }

    public string Name { get; }

    public ChessTeam ChessTeam { get; set; }

    private readonly IHubContext<ChessHub, IChessClientApi> m_hubContext;
    private readonly string                                 m_connectionId;
    private          IChessClientApi                        client => m_hubContext.Clients.Client(m_connectionId);

    public ServerChessPlayer(UserUniqueId                           userId
                           , PlayerId                               playerId
                           , string                                 name
                           , IHubContext<ChessHub, IChessClientApi> hubContext
                           , string                                 connectionId)
    {
        UserUniqueId   = userId;
        PlayerId       = playerId;
        Name           = name;
        m_hubContext   = hubContext;
        m_connectionId = connectionId;
    }

    public Task StartGame(GameConfig gameConfig) => client.StartGame(gameConfig);

    public Task EndGame() => client.EndGame();
    public Task CheckMate(CheckMateData           checkMateData) => client.CheckMate(checkMateData);
    public Task ApplyBoardCommands(BoardCommand[] boardCommands) => client.ApplyBoardCommands(boardCommands);

    public Task AskPromote(PromotionRequest requst) => client.AskPromote(requst);

    public Task UpdateTime(TeamId   teamId
                         , TimeSpan timeSpan) => client.UpdateTime(teamId, timeSpan);

    public Task UpdatePlayingTeam(TeamId teamId) => client.UpdatePlayingTeam(teamId);

    public Task UpdateToolsAndTeams(ToolAndTeamPair[] pairs) => client.UpdateToolsAndTeams(pairs);
}