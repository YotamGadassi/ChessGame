using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;
using ChessGame;
using Client.Game;
using Common;
using Common_6;
using Common_6.ChessBoardEventArgs;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;

namespace Frameworks;

public class OnlineFramework
{
    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private                 HubConnection       m_connection;
    private                 BaseGameManager     m_gameManager;
    private                 Team                m_localMachineTeam;
    private readonly        Dispatcher          m_dispatcher;
    public                  OnlineGameViewModel ViewModel;
    private static readonly string              s_hubAddress = @"https://localhost:7034/ChessHub";

    private static Guid                            lastGameVersion;
    public event EventHandler<OnlineGameViewModel> OnGameStarted;
    public event EventHandler                      OnGameEnd;

    public HubConnectionState ConnectionState => m_connection.State;

    public OnlineFramework()
    {
        m_dispatcher = Dispatcher.CurrentDispatcher;
    }

    public async Task AsyncRequestGameFromServer()
    {
        await m_connection.InvokeAsync("RequestGame");
    }

    private async void GameManagerOnToolMovedEvent(object sender, ToolMovedEventArgs e)
    {
        bool isMovedFromServer = e.MovedTool.Color != m_localMachineTeam.Color;
        bool isFirstArrangement = e.InitialPosition.IsEmpty();
        if (isMovedFromServer || isFirstArrangement)
        {
            return;
        }

        try
        {
            s_log.Info($"Local move event has been invoked. start:{e.InitialPosition}, end:{e.EndPosition}");
            await m_connection.InvokeAsync("Move", e.InitialPosition, e.EndPosition, lastGameVersion);
            s_log.Info($"Move invocation has been sent to server: [start:{e.InitialPosition}] [end:{e.EndPosition}] [game version:{lastGameVersion}]");
        }
        catch (Exception exception)
        {
            s_log.Error(exception);
        }
    }

    private Team resolveTeam(Color color)
    {
        if (color == Colors.White)
            return new Team("White_A", Colors.White, GameDirection.North);
        else
            return new Team("Black_B", Colors.Black, GameDirection.South);
    }

    public async Task<bool> ConnectToHubAsync(string name)
    {
        if (m_connection != null && m_connection.State != HubConnectionState.Disconnected)
        {
            s_log.Info($"Connection state is {m_connection.State}. Cannot connect again.");
            return true;
        }

        m_connection        =  new HubConnectionBuilder().WithUrl(s_hubAddress + $"?name={name}").Build();
        m_connection.Closed += onConnectionClosed;
        registerClientMethods();
        s_log.Info($"Starting connection to client. server state:{m_connection.State}");
        try
        {
            await m_connection.StartAsync(); // The await returns to the same dispatcher
        }
        catch (Exception e)
        {
            s_log.Error(e.Message);
            return false;
        }

        s_log.Info($"connection to client completed. server state:{m_connection.State}");

        return true;
    }

    private Task onConnectionClosed(Exception? arg)
    {
        return new Task(() => s_log.Warn($"Connection closed: [exception: {arg}]"));

    }

    private void registerClientMethods()
    {
        m_connection.On<BoardPosition, BoardPosition,Guid>("Move", handleMoveRequest);
        m_connection.On<Team, Team, Guid>("StartGame", handleStartGameRequest);
        m_connection.On("EndGame", handleEndGameRequest);
        m_connection.On<Team, TimeSpan>("UpdateTime", handleTimeUpdate);
    }

    private void handleMoveRequest(BoardPosition start, BoardPosition end, Guid newGameVersion)
    {
        m_dispatcher.BeginInvoke(() =>
                                 {
                                     lastGameVersion = newGameVersion;
                                     s_log
                                        .Info($"A move request received from server: [start:{start}], [end:{end}], [game version:{newGameVersion}]");
                                     m_gameManager.Move(start, end);
                                 });
    }

    private void handleStartGameRequest(Team localTeam, Team remoteTeam, Guid gameVersion)
    {
        m_dispatcher.InvokeAsync(() => startGame(localTeam, remoteTeam, gameVersion));
    }

    private void handleEndGameRequest()
    {
        m_dispatcher.InvokeAsync(endGame);
    }

    private void handleTimeUpdate(Team     team
                                , TimeSpan timeLeft)
    {
        m_dispatcher.Invoke(() => updateTime(team, timeLeft));
    }

    private void updateTime(Team     team
                          , TimeSpan timeLeft)
    {
        if (team.MoveDirection == GameDirection.North)
        {
            ViewModel.SouthTeamStatus.TimeLeft = timeLeft;
        }
        else
        {
            ViewModel.NorthTeamStatus.TimeLeft = timeLeft;
        }
    }

    private void startGame(Team localTeam, Team remoteTeam, Guid gameVersion)
    {
        s_log.Info($"Stating Game. local team: {localTeam}, remote team: {remoteTeam}, game version: {gameVersion}");
        lastGameVersion    = gameVersion;
        m_localMachineTeam = localTeam;

        m_gameManager = new OnlineGameManager(m_localMachineTeam);

        m_gameManager.ToolMovedEvent  += GameManagerOnToolMovedEvent;
        m_gameManager.ToolKilledEvent += GameManagerOnToolMovedEvent;
        if (localTeam.MoveDirection.Equals(GameDirection.South))
        {
            ViewModel = new OnlineGameViewModel(m_gameManager, localTeam, remoteTeam, m_localMachineTeam);
        }
        else
        {
            ViewModel = new OnlineGameViewModel(m_gameManager, remoteTeam ,localTeam, m_localMachineTeam);
        }
        OnGameStarted?.Invoke(this, ViewModel);
        m_gameManager.StartGame();
        s_log.Info("Game Started");
    }

    private async void endGame()
    {
        s_log.Info("Game ended");
        await m_connection.StopAsync();
        m_gameManager.EndGame();
        m_gameManager = null;
        OnGameEnd?.Invoke(this, null);
    }

}