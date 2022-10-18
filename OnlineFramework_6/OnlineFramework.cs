using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;
using ChessGame;
using Client.Board;
using Client.Game;
using Common_6;
using Common_6.ChessBoardEventArgs;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;

namespace Frameworks;

public class OnlineFramework
{
    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly        HubConnection     m_connection;
    private                 BaseGameManager   m_gameManager;
    private                 Team              m_localMachineTeam;
    private readonly        Dispatcher        m_dispatcher;
    public                  BaseGameViewModel ViewModel;
    private static readonly string            s_hubAddress = @"https://localhost:7034/ChessHub";

    public event EventHandler<BaseGameViewModel> OnGameStarted;
    public event EventHandler                    OnGameEnd;

    public HubConnectionState ConnectionState => m_connection.State;

    public OnlineFramework()
    {
        m_dispatcher = Dispatcher.CurrentDispatcher;
        m_connection = new HubConnectionBuilder().WithUrl(s_hubAddress).Build();
    }

    public async void AsyncRequestGameFromServer()
    {
        if (m_connection.State != HubConnectionState.Disconnected)
        {
            s_log.Info($"Connection state is {m_connection.State}.");
            return;
        }

        await connectToHubAsync();
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
            s_log.Info($"Local move event has been invoked");
            await m_connection.InvokeAsync("Move", e.InitialPosition, e.EndPosition);
            s_log.Info($"Move invocation has been sent to server: [start:{e.InitialPosition}] [end:{e.EndPosition}]");
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

    private async Task<bool> connectToHubAsync()
    {
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
        m_connection.On<BoardPosition, BoardPosition>("Move", handleMoveRequest);
        m_connection.On<Color>("StartGame", handleStartGameRequest);
        m_connection.On("EndGame", handleEndGameRequest);
    }

    private void handleMoveRequest(BoardPosition start, BoardPosition end)
    {
        m_dispatcher.BeginInvoke(() =>
                                 {
                                     s_log
                                        .Info($"A move request received from server: [start:{start}], [end:{end}]");
                                     m_gameManager.Move(start, end);
                                 });
    }

    private void handleStartGameRequest(Color localTeamColor)
    {
        m_dispatcher.InvokeAsync(() => startGame(localTeamColor));
    }

    private void handleEndGameRequest()
    {
        m_dispatcher.InvokeAsync(endGame);
    }

    private void startGame(Color otherTeamColor)
    {
        s_log.Info("Stating Game");

        Color currentTeamColor = otherTeamColor == Colors.Black ? Colors.White : Colors.Black;

        m_localMachineTeam = resolveTeam(currentTeamColor);

        OnlineGameManager gameManager = new(m_localMachineTeam);
        m_gameManager = gameManager;

        m_gameManager.ToolMovedEvent  += GameManagerOnToolMovedEvent;
        m_gameManager.ToolKilledEvent += GameManagerOnToolMovedEvent;

        Team northTeam;
        Team southTeam;

        if (currentTeamColor == Colors.White)
        {
            southTeam = new Team("Local",  Colors.White, GameDirection.North);
            northTeam = new Team("Remote", Colors.Black,  GameDirection.South);
        }
        else
        {
            southTeam = new Team("Remote",  Colors.White, GameDirection.North);
            northTeam = new Team("Local", Colors.Black,   GameDirection.South);
        }

        ViewModel = new OnlineGameViewModel(m_gameManager, northTeam, southTeam, m_localMachineTeam);
        OnGameStarted?.Invoke(this, ViewModel);
        gameManager.StartGame();
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