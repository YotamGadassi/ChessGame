using System.Reflection;
using System.Windows.Threading;
using ChessGame;
using Client.Board;
using Client.Game;
using Client.Helpers;
using Client.Messages;
using Common;
using Common_6;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Frameworks;

public class OnlineFramework
{
    private static readonly ILog   s_log        = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly string s_hubAddress = @"https://localhost:7034/ChessHub";
    private                 Guid   lastGameVersion;

    private          HubConnection        m_connection;
    private          OnlineGameManager    m_gameManager;
    private          Team                 m_localMachineTeam;
    private          AvailableMovesHelper m_availableMovesHelper;
    private readonly Dispatcher           m_dispatcher;

    public OnlineGameViewModel                     ViewModel;
    public event EventHandler                      OnGameEnd;

    public HubConnectionState ConnectionState => m_connection.State;

    public OnlineFramework()
    {
        m_dispatcher  = Dispatcher.CurrentDispatcher;
        
        ViewModel = new OnlineGameViewModel(SquareClickHandler, SquareClickHandlerCanExecute);
    }

    public async Task<bool> ConnectToServerAsync(string name)
    {
        bool isConnected = m_connection != null && m_connection.State != HubConnectionState.Disconnected;
        if (isConnected)
        {
            s_log.Info($"Connection state is {m_connection.State}. Cannot connect again.");
            return true;
        }

        UserMessageViewModel msgViewModel = new("Waiting for connection with server", "Cancel", (() => endGame()));
        ViewModel.Message = msgViewModel;

        m_connection = new HubConnectionBuilder().WithUrl(s_hubAddress + $"?name={name}")
                                                 .ConfigureLogging(builder => builder.AddLog4Net("LogConfiguration.xml"))
                                                 .AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(new IToolConverter()))
                                                 .Build();

        m_connection.Closed += onConnectionClosed;
        registerClientMethods();
        s_log.Info($"Starting connection to client. server state:{m_connection.State}");
        
        try
        {
            await m_connection.StartAsync();
        }
        catch (Exception e)
        {
            s_log.Error(e.Message);
            return false;
        }

        s_log.Info($"connection to client completed. server state:{m_connection.State}");

        return true;
    }

    public async Task<bool> AsyncRequestGameFromServer()
    {
        return await m_connection.InvokeAsync<bool>("RequestGame");
    }

    public async Task DisconnectFromServerAsync()
    {
        s_log.Info("Disconnected from server request started");
        await m_connection.StopAsync();
        s_log.Info("Disconnected from server succeeded");
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
        m_connection.On<BoardState>("ForceAddToBoard", handleForceAdd);
    }

    #region serverExecutionsHandlers

    private void handleMoveRequest(BoardPosition start, BoardPosition end, Guid newGameVersion)
    {
        m_dispatcher.BeginInvoke(() =>
                                 {
                                     s_log.Info($"Move request arrived from server [start:{start}, end:{end}, version:{newGameVersion}]");

                                     lastGameVersion = newGameVersion;
                                     s_log.Info($"A move request received from server: " +
                                                $"[start:{start}], [end:{end}], [game version:{newGameVersion}]");
                                     moveTool(start, end);
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

    private void handlePromotionEvent()
    {

    }

    private void handleForceAdd(BoardState boardState)
    {
        s_log.Info($"Force Add Invoked: {boardState}");
        m_dispatcher.InvokeAsync(
                                 () =>
                                 {
                                     foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
                                     {
                                         forceAddTool(pair.Key, pair.Value);
                                     }
                                 });
    }

    private void handleTimeUpdate(Team     team
                                , TimeSpan timeLeft)
    {
        m_dispatcher.Invoke(() => updateTime(team, timeLeft));
    }

    #endregion

    private void updateTime(Team     team
                          , TimeSpan timeLeft)
    {
        if (null == ViewModel)
        {
            return;
        }

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

        m_gameManager                 =  new OnlineGameManager(m_localMachineTeam);
        m_availableMovesHelper        =  new AvailableMovesHelper(m_gameManager);

        if (localTeam.MoveDirection.Equals(GameDirection.South))
        {
            ViewModel.StartGame(localTeam, remoteTeam);
        }
        else
        {
            ViewModel.StartGame(remoteTeam ,localTeam);
        }
        m_gameManager.StartGame();
        s_log.Info("Game Started");
    }

    private async void endGame()
    {
        s_log.Info("Game ended");
        await m_connection.StopAsync();
        UserMessageViewModel endGameMessage = new UserMessageViewModel("Game has ended", 
                                                                       "OK", 
                                                                       () =>
                                                                       {
                                                                           m_gameManager = null;
                                                                           ViewModel.EndGame();
                                                                           OnGameEnd?.Invoke(this, null);
                                                                       });
        ViewModel.Message = endGameMessage;
    }

    private async Task<bool> sendMoveRequest(BoardPosition initial
                                           , BoardPosition end)
    {
        MoveResult moveResult = await m_connection.InvokeAsync<MoveResult>("MoveRequest", initial, end, lastGameVersion);
        return moveResult.Result != MoveResultEnum.NoChangeOccurred;
    }

    private bool m_isWaitingForServerResponse = false;
    protected async void SquareClickHandler(BoardPosition position
                                                   , ITool?        tool)
    {
        BoardViewModel  board                  = ViewModel.Board;
        bool isPositionToolSameTeam = null != tool && tool.Color.Equals(m_localMachineTeam.Color);
        if (isPositionToolSameTeam)
        {
            board.ClearSelectedAndHintedBoardPositions();
            board.SelectedBoardPosition = position;
            BoardPosition[] positionToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
            board.SetHintedBoardPosition(positionToMove);
            return;
        }

        if (false == board.SelectedBoardPosition.IsEmpty())
        {
            m_isWaitingForServerResponse = true;
            bool isMoveApproved = await sendMoveRequest(board.SelectedBoardPosition, position);
            m_isWaitingForServerResponse = false;
            if (isMoveApproved)
            {
                s_log.Info($"Move from {board.SelectedBoardPosition} to {position} approved by server");
                moveTool(board.SelectedBoardPosition, position);
            }
            else
            {
                s_log.Info($"Move from {board.SelectedBoardPosition} to {position} was not approved by server");
            }
        }
        board.ClearSelectedAndHintedBoardPositions();
    }

    protected bool SquareClickHandlerCanExecute(BoardPosition poistion
                                                       , ITool?        tool)
    {
        if (m_isWaitingForServerResponse || ConnectionState != HubConnectionState.Connected || null == m_gameManager)
        {
            return false;
        }
        bool isLocalTeamTurn = m_gameManager.CurrentColorTurn.Equals(m_localMachineTeam.Color);
        return isLocalTeamTurn;
    }

    private void forceAddTool(BoardPosition position
                            , ITool         tool)
    {
        m_gameManager.ForceAddTool(position, tool);
        ViewModel.MoveTool(BoardPosition.Empty, position, tool);
    }

    private void moveTool(BoardPosition start
                        , BoardPosition end)
    {
        MoveResult result = m_gameManager.Move(start, end);
        if (result.Result.HasFlag(MoveResultEnum.ToolMoved))
        {
            ViewModel.MoveTool(result.InitialPosition, result.EndPosition, result.ToolAtInitial);
        }
    }
}