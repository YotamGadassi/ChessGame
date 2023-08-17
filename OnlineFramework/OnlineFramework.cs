using System.Reflection;
using System.Windows.Threading;
using Board;
using ChessGame;
using ChessGame.Helpers;
using Client.Board;
using Client.Game;
using Client.Helpers;
using Client.Messages;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tools;

namespace Frameworks;

public class OnlineFramework
{
    private static readonly ILog   s_log        = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private                 Guid   m_lastGameVersion;

    private          IConnectionManager<IGameServerAgent> m_connectionManager;
    private          OnlineGameManager    m_gameManager;
    private          Team                 m_localMachineTeam;
    private          AvailableMovesHelper m_availableMovesHelper;
    private readonly Dispatcher           m_dispatcher;

    public OnlineGameViewModel                     ViewModel;
    public event EventHandler                      OnGameEnd;

    public OnlineFramework(IConnectionManager<IGameServerAgent> connectionManager)
    {
        m_connectionManager = connectionManager;
        m_dispatcher  = Dispatcher.CurrentDispatcher;
        
        ViewModel = new OnlineGameViewModel(SquareClickHandler, SquareClickHandlerCanExecute);
    }

    public void Init()
    {
        registerServerMethods();
        registerEvents();
    }

    private void registerEvents()
    {
        m_connectionManager.ConnectionClosed += onConnectionClosed;
    }

    public Task<bool> ConnectToServerAsync()
    {
        return m_connectionManager.Connect();
    }

    public Task<bool> AsyncRequestGameFromServer()
    {
        return m_connectionManager.ServerAgent.RequestGame();
    }

    public async Task DisconnectFromServerAsync()
    {
        s_log.Info("Disconnected from server request started");
        await m_connectionManager.Disconnect();
        s_log.Info("Disconnected from server succeeded");
    }

    private Task onConnectionClosed(Exception? arg)
    {
        m_dispatcher.Invoke(endGame);
        return new Task(() => s_log.Warn($"Connection closed: [exception: {arg}]"));
    }

    private void registerServerMethods()
    {
        IGameServerAgent agent = m_connectionManager.ServerAgent;

        agent.MoveRequestEvent      += handleMoveRequest;
        agent.StartGameRequestEvent += handleStartGameRequest;
        agent.EndtGameRequestEvent  += handleEndGameRequest;
        agent.TimeReceivedEvent     += handleTimeUpdate;
        agent.BoardReceivedEvent    += handleBoardReceived;
        agent.CheckmateEvent        += handleCheckMateEvent;
        agent.PromotionEvent        += handlePromotion;
    }

    #region serverExecutionsHandlers

    private void handleMoveRequest(BoardPosition start, BoardPosition end)
    {
        m_dispatcher.BeginInvoke(() =>
                                 {
                                     s_log.Info($"Move request arrived from server [start:{start}, end:{end}]");

                                     s_log.Info($"A move request received from server: " +
                                                $"[start:{start}], [end:{end}]");
                                     moveTool(start, end);
                                 });
    }

    private void handleStartGameRequest(Team localTeam, Team remoteTeam, Guid gameToken)
    {
        m_dispatcher.InvokeAsync(() => startGame(localTeam, remoteTeam, gameToken));
    }

    private void handleEndGameRequest()
    {
        m_dispatcher.InvokeAsync(endGame);
    }

    private void handleCheckMateEvent(BoardPosition start, BoardPosition end)
    {
        UserMessageViewModel checkMateMessage =
            new UserMessageViewModel($"CheckMate [move from {start} to {end}]", "OK", () => endGame());

        ViewModel.Message = checkMateMessage;
    }

    private void handlePromotion(BoardPosition positionToPromote, IToolWrapperForServer toolWrapper)
    {
        m_gameManager.Promote(positionToPromote,toolWrapper.Tool);
        ViewModel.Board.RemoveTool(positionToPromote, out _);
        ViewModel.Board.AddTool(toolWrapper.Tool, positionToPromote);
    }
    
    private async void handleNeedPromotionEvent(BoardPosition positionToPromote, ITool toolToPromote)
    {
        PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, positionToPromote);
        ViewModel.Message = promotionMessage;
    
        ITool chosenTool = await promotionMessage.ToolAwaiter;
        ViewModel.Message = null;
        bool promoteResult = await m_connectionManager.ServerAgent.PromoteRequest(positionToPromote
                                                                                , new IToolWrapperForServer(chosenTool)
                                                                                , m_lastGameVersion);
    
        if (false == promoteResult)
        {
            s_log.Warn($"Promote result is false [position:{positionToPromote}, tool:{chosenTool}]");
            //TODO: show message
            return;
        }
    
        handlePromotion(positionToPromote, new IToolWrapperForServer(chosenTool));
    
    }

    private void handleBoardReceived(BoardState boardState)
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

    private void startGame(Team localTeam, Team remoteTeam, Guid gameToken)
    {
        s_log.Info($"Stating Game. local team: {localTeam}, remote team: {remoteTeam}, game version: {gameToken}");
        m_lastGameVersion    = gameToken;
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
        await m_connectionManager.Disconnect();
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

    private async Task<MoveResult> sendMoveRequest(BoardPosition initial
                                           , BoardPosition end)
    {
        return await m_connectionManager.ServerAgent.SendMoveRequest(initial, end, m_lastGameVersion);
    }

    private bool m_isWaitingForServerResponse = false;
    protected async void SquareClickHandler(BoardPosition position
                                                   , ITool?        tool)
    {
        bool isLocalTeamTurn = await m_connectionManager.ServerAgent.IsMyTurn();
        if (false == isLocalTeamTurn)
        {
            return;
        }
        
        BoardViewModel  board                  = ViewModel.Board;
        bool isPositionToolSameTeam = null != tool && tool.Color.Equals(m_localMachineTeam.Color);
        if (isPositionToolSameTeam)
        {
            board.ClearSelectedAndHintedBoardPositions();
            board.SelectedBoardPosition = position;
            BoardPosition[] positionToMove = await m_availableMovesHelper.GetAvailablePositionToMove(position);
            board.SetHintedBoardPosition(positionToMove);
            return;
        }

        if (false == board.SelectedBoardPosition.IsEmpty())
        {
            m_isWaitingForServerResponse = true;
            MoveResult moveResult = await sendMoveRequest(board.SelectedBoardPosition, position);
            m_isWaitingForServerResponse = false;
            handleMoveResult(moveResult);
        }
        board.ClearSelectedAndHintedBoardPositions();
    }

    private void handleMoveResult(MoveResult moveResult)
    {
        MoveResultEnum resultEnum            = moveResult.Result;
        BoardPosition  initialPosition       = moveResult.InitialPosition;
        BoardPosition  endPosition = moveResult.EndPosition;
        if (resultEnum.HasFlag(MoveResultEnum.CheckMate))
        {
            s_log.Info($"Move result is CheckMate [from {initialPosition} to {endPosition}]");
            handleCheckMateEvent(initialPosition, endPosition);
            return;
        }

        if (resultEnum.HasFlag(MoveResultEnum.NeedPromotion))
        {
            s_log.Info($"Move result is NeedPromotion [from {initialPosition} to {endPosition}]");
            handleNeedPromotionEvent(endPosition, moveResult.ToolAtInitial);
        }

        if(resultEnum.HasFlag(MoveResultEnum.ToolMoved))
        {
            s_log.Info($"Move from {initialPosition} to {endPosition} approved by server");
            moveTool(initialPosition, endPosition);
        }
        else
        {
            s_log.Info($"Move from {initialPosition} to {endPosition} was not approved by server");
        }
    }

    protected bool SquareClickHandlerCanExecute(BoardPosition poistion
                                              , ITool?        tool)
    {
        if (m_isWaitingForServerResponse || m_connectionManager.ConnectionStatus != ConnectionStatus.Connected || null == m_gameManager)
        {
            return false;
        }

        return true;
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