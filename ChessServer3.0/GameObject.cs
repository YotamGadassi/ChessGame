using System.Collections.Concurrent;
using System.Windows.Media;
using ChessGame;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0;

public class GameUnit : IDisposable
{
    public GameUnit(PlayerObject player1, PlayerObject player2)
    {
        GroupName            = Guid.NewGuid().ToString();
        CurrentGameVersion   = Guid.Empty;
        m_gameManager        = new OfflineGameManager();
    }

    public  string          GroupName          { get; }
    public  Guid            CurrentGameVersion { get; private set; }

    private BaseGameManager m_gameManager;
    public  PlayerObject    Player1 { get; }
    public  PlayerObject    Player2 { get; }

    public bool IsStarted => !CurrentGameVersion.Equals(Guid.Empty);

    public async Task StartGame(Hub hub)
    {
        if (IsStarted)
        {
            return;
        }

        CurrentGameVersion = Guid.NewGuid();

        m_gameManager.StartGame();
        Player1.PlayersTeam = new Team(Player1.Name, Colors.White, GameDirection.North);
        Player1.GameUnit    = this;
        Player2.PlayersTeam = new Team(Player2.Name, Colors.Black, GameDirection.South);
        Player2.GameUnit    = this;

        await Task.WhenAll(hub.Groups.AddToGroupAsync(Player1.ConnectionId, GroupName),
                           hub.Groups.AddToGroupAsync(Player2.ConnectionId, GroupName));
    }

    public async void EndGame(Hub hub)
    {
        if (false == IsStarted)
        {
            return;
        }

        CurrentGameVersion = Guid.Empty;

        m_gameManager.EndGame();

        await Task.WhenAll(hub.Groups.RemoveFromGroupAsync(Player1.ConnectionId, GroupName),
                           hub.Groups.RemoveFromGroupAsync(Player2.ConnectionId, GroupName)
                          );
    }

    public void Move(Hub           hub
                   , Guid          gameVersion
                   , BoardPosition start
                   , BoardPosition end)
    {
        if (false == gameVersion.Equals(CurrentGameVersion))
        {
            return;
        }

        MoveResult result = m_gameManager.Move(start, end);
        if (result.Result == MoveResultEnum.NoChangeOccurred)
        {
            return;
        }

        CurrentGameVersion = Guid.NewGuid();
        switch (result.Result)
        {
            case MoveResultEnum.ToolMoved:
            case MoveResultEnum.ToolKilled:
            {
                hub.Clients.Group(GroupName)
                   .SendAsync("Move", start, end, CurrentGameVersion);
                break;
            }
        }
    }

    public PlayerObject GetOtherPlayer(PlayerObject player)
    {
        if (player.Equals(Player1))
        {
            return Player2;
        }

        return Player1;
    }

    public void Dispose()
    {

    }
}