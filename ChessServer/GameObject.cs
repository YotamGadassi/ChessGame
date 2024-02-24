using System.Windows.Media;
using Board;
using ChessGame;
using Common;
using Common.Chess;
using Microsoft.AspNetCore.SignalR;
using Tools;

namespace ChessServer;

public delegate void PlayerTimeChanged(PlayerObject player
                                     , TimeSpan     timeLeft);

public class GameUnit1 : IDisposable
{
    public GameUnit1(PlayerObject player1
                  , PlayerObject player2)
    {
        GroupName                     =  Guid.NewGuid().ToString();
        GameToken                     =  Guid.Empty;
        //m_gameController                 =  new OfflineChessGameManager();
        WhitePlayer1                  =  player1;
        WhitePlayer1.OneSecPassEvent  += onPlayerOneSecElapsed;
        BlackPlayer2                  =  player2;
        BlackPlayer2.OneSecPassEvent  += onPlayerOneSecElapsed;
        //m_gameController.TeamSwitchEvent += onTeamSwitch;
    }

    private OfflineChessGameManager m_gameManager;

    private IHubContext<ChessHub> m_hubContext;
    public  string                GroupName { get; }
    public  Guid                  GameToken { get; private set; }

    public PlayerObject WhitePlayer1 { get; }
    public PlayerObject BlackPlayer2 { get; }

    public event PlayerTimeChanged PlayerTimeChangedEvent;

    public bool IsStarted => !GameToken.Equals(Guid.Empty);

    public async Task<bool> StartGame()
    {
        if (IsStarted)
        {
            return false;
        }

        GameToken = Guid.NewGuid();

        m_gameManager.GameStateController.StartResumeGame();
        WhitePlayer1.PlayersTeam = new Team(WhitePlayer1.Name, Colors.White, GameDirection.North, TeamId.NewTeamId());
        WhitePlayer1.GameUnit    = this;
        BlackPlayer2.PlayersTeam = new Team(BlackPlayer2.Name, Colors.Black, GameDirection.South, TeamId.NewTeamId());
        BlackPlayer2.GameUnit    = this;

        WhitePlayer1.StartTimer();
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

    public void Dispose() { }

    private void onPlayerOneSecElapsed(object?  sender
                                     , TimeSpan timeLeft)
    {
        if (false == sender is PlayerObject player)
        {
            return;
        }

        PlayerTimeChangedEvent?.Invoke(player, timeLeft);
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
        return m_gameManager.TeamsManager.CurrentTeamTurnId.Equals(player.PlayersTeam.Id);
    }

    public BoardState GetBoardState()
    {
        return m_gameManager.GetBoardState();
    }
}