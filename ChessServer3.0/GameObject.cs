using System.Windows.Media;
using ChessGame;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using Microsoft.AspNetCore.SignalR;
using static System.Windows.Media.Colors;

namespace ChessServer3._0;

public delegate void PlayerTimeChanged(PlayerObject player
                                     , TimeSpan     timeLeft);
public class GameUnit : IDisposable
{
    public GameUnit(PlayerObject player1, PlayerObject player2)
    {
        GroupName                     =  Guid.NewGuid().ToString();
        CurrentGameVersion            =  Guid.Empty;
        m_gameManager                 =  new OfflineGameManager();
        WhitePlayer1                  =  player1;
        WhitePlayer1.OneSecPassEvent  += onPlayerOneSecElapsed;
        BlackPlayer2                  =  player2;
        BlackPlayer2.OneSecPassEvent  += onPlayerOneSecElapsed;
        m_gameManager.TeamSwitchEvent += onTeamSwitch;
    }

    private BaseGameManager       m_gameManager;
    private IHubContext<ChessHub> m_hubContext;
    public  string                GroupName          { get; }
    public  Guid                  CurrentGameVersion { get; private set; }

    public  PlayerObject    WhitePlayer1 { get; }
    public  PlayerObject    BlackPlayer2 { get; }

    public event PlayerTimeChanged PlayerTimeChangedEvent;

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
        WhitePlayer1.StartTimer();
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

    public MoveResult Move(Guid          gameVersion
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

    private void onPlayerOneSecElapsed(object? sender, TimeSpan timeLeft)
    {
        if (false == sender is PlayerObject player)
        {
            return;
        }

        PlayerTimeChangedEvent?.Invoke(player, timeLeft);
    }

    private void onTeamSwitch(object? sender, Color teamColor)
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
}