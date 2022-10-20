using System.Data.OleDb;
using System.Diagnostics;
using System.Windows.Media;
using ChessGame;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using Microsoft.AspNetCore.SignalR;
using static System.Windows.Media.Colors;
using Timer = System.Timers.Timer;

namespace ChessServer3._0;

public class GameUnit : IDisposable
{
    public GameUnit(PlayerObject player1, PlayerObject player2)
    {
        GroupName          = Guid.NewGuid().ToString();
        CurrentGameVersion = Guid.Empty;
        m_gameManager      = new OfflineGameManager();
        WhitePlayer1            = player1;
        BlackPlayer2            = player2;
    }

    public  string          GroupName          { get; }
    public  Guid            CurrentGameVersion { get; private set; }

    private BaseGameManager m_gameManager;
    public  PlayerObject    WhitePlayer1 { get; }
    public  PlayerObject    BlackPlayer2 { get; }

    public bool IsStarted => !CurrentGameVersion.Equals(Guid.Empty);

    public async Task StartGame(Hub hub)
    {
        if (IsStarted)
        {
            return;
        }

        CurrentGameVersion = Guid.NewGuid();

        m_gameManager.StartGame();
        WhitePlayer1.PlayersTeam = new Team(WhitePlayer1.Name, White, GameDirection.North);
        WhitePlayer1.GameUnit    = this;
        BlackPlayer2.PlayersTeam = new Team(BlackPlayer2.Name, Black, GameDirection.South);
        BlackPlayer2.GameUnit    = this;

        await Task.WhenAll(hub.Groups.AddToGroupAsync(WhitePlayer1.ConnectionId, GroupName),
                           hub.Groups.AddToGroupAsync(BlackPlayer2.ConnectionId, GroupName));
    }

    public async void EndGame(Hub hub)
    {
        if (false == IsStarted)
        {
            return;
        }

        CurrentGameVersion = Guid.Empty;

        m_gameManager.EndGame();

        await Task.WhenAll(hub.Groups.RemoveFromGroupAsync(WhitePlayer1.ConnectionId, GroupName),
                           hub.Groups.RemoveFromGroupAsync(BlackPlayer2.ConnectionId, GroupName)
                          );
    }

    public MoveResult Move(Hub           hub
                         , Guid          gameVersion
                         , BoardPosition start
                         , BoardPosition end)
    {
        if (false == gameVersion.Equals(CurrentGameVersion))
        {
            return MoveResult.NoChangeOccurredResult;
        }

        MoveResult result = m_gameManager.Move(start, end);
        if (result.Result != MoveResultEnum.NoChangeOccurred)
        {
            CurrentGameVersion = Guid.NewGuid();
        }

        return result;
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

    }
}