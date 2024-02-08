using System.Windows.Media;
using Board;
using ChessGame;
using ChessServer3._0.ClientInterface;
using Common;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using Tools;

namespace ChessServer3._0.Game;

public class ServerGameMangaer : IDisposable
{
    private OfflineTeamsManager                m_teamManager;
    private OfflineChessBoardProxy             m_chessBoardProxy;
    private IChessPlayer[]                     m_players;
    private Dictionary<Guid, Action<TimeSpan>> m_timeLeftEventDict;

    public ServerGameMangaer(GameConfiguration gameConfiguration)
    {
        m_players           = gameConfiguration.Players;
        m_timeLeftEventDict = new Dictionary<Guid, Action<TimeSpan>>();
        ChessTeam[] chessTeams = m_players.Select((player) => player.Team).ToArray();
        m_teamManager = new(chessTeams);

        m_chessBoardProxy = new OfflineChessBoardProxy()
        registerToEvents();
    }

    public async Task<bool> StartGame()
    {
        m_gameManager.GameStateController.StartResumeGame();
        return true;
    }

    public async void EndGame()
    {
        if (false == IsStarted)
        {
            return;
        }

        GameToken = Guid.Empty;

        //m_gameController.EndGame();

        await Task.WhenAll(m_hubContext.Groups.RemoveFromGroupAsync(WhitePlayer1.ConnectionId, GroupName),
                           m_hubContext.Groups.RemoveFromGroupAsync(BlackPlayer2.ConnectionId, GroupName)
                          );
    }

    public MoveResult Move(Guid          gameToken
                         , BoardPosition start
                         , BoardPosition end)
    {
        if (false == gameToken.Equals(GameToken))
        {
            return MoveResult.NoChangeOccurredResult;
        }

        return m_gameManager.ChessBoardProxy.Move(start, end);
        ;
    }

    public bool Promote(Guid          gameToken
                      , BoardPosition position
                      , ITool         promotedTool)
    {
        if (false == gameToken.Equals(GameToken))
        {
            return false;
        }

        m_gameManager.ChessBoardProxy.Promote(position, promotedTool);
        return true;
    }

    public PlayerObject GetOtherPlayer(PlayerObject player)
    {
        if (player.Equals(WhitePlayer1))
        {
            return BlackPlayer2;
        }

        return WhitePlayer1;
    }

    public void Dispose()
    {
        unRegisterFromEvents();
        m_gameManager.Dispose();
    }

    private void registerToEvents()
    {
        foreach (IChessPlayer player in m_players)
        {
            Guid             teamId = player.Team.Id;
            Action<TimeSpan> del    = (timeLeft) => player.UpdateTime(teamId, timeLeft);
            m_timeLeftEventDict[teamId]          =  del;
            player.Team.TeamTimer.TimeLeftChange += del;
        }
    }

    private void unRegisterFromEvents()
    {
        foreach (IChessPlayer player in m_players)
        {
            Guid teamId = player.Team.Id;
            player.Team.TeamTimer.TimeLeftChange -= m_timeLeftEventDict[teamId];
        }
    }

    private void onTeamSwitch(object? sender
                            , Color   teamColor)
    {
        if (teamColor == Colors.Black)
        {
            WhitePlayer1.StopTimer();
            BlackPlayer2.StartTimer();
            return;
        }

        BlackPlayer2.StopTimer();
        WhitePlayer1.StartTimer();
    }

    public bool IsPlayerTurn(PlayerObject player)
    {
        return m_gameManager.TeamsManager.CurrentTeamTurn.Color.Equals(player.PlayersTeam.Color);
    }

    public BoardState GetBoardState()
    {
        return m_gameManager.GetBoardState();
    }
}