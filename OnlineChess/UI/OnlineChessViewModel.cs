using System.Windows.Threading;
using Board;
using ChessGame.Helpers;
using Client.Board;
using Client.Game.GameMainControl;
using Client.Messages;
using Common;
using log4net;
using OnlineChess.Game;
using OnlineChess.TeamManager;
using Tools;

namespace OnlineChess.UI;

public class OnlineChessViewModel : ChessGameViewModel
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessViewModel));

    private readonly OnlineGameBoard        m_gameBoard;
    private readonly OnlineChessTeamManager m_teamManager;
    private readonly IAvailableMovesHelper  m_availableMovesHelper;
    private readonly Dispatcher             m_dispatcher;
    private readonly OnlineChessGameManager m_gameManager;
    private readonly ManualResetEvent m_resetEvent;

    public OnlineChessViewModel(OnlineChessGameManager gameManager
                              , Dispatcher             dispatcher) : base(gameManager)
    {
        m_dispatcher           = dispatcher;
        m_resetEvent           = new ManualResetEvent(false) { };
        m_gameManager          = gameManager;
        m_gameBoard            = gameManager.GameBoard;
        m_teamManager          = gameManager.OnlineTeamsManager;
        m_availableMovesHelper = new AvailableMovesHelper(gameManager.BoardQuery);
        initBoardState(gameManager.BoardQuery);
        regiterToEvents();
    }

    private void regiterToEvents()
    {
        m_gameManager.GameState.StateChanged += onStateChanged;
    }

    private async void onStateChanged(object?       sender
                              , GameStateEnum e)
    {
        if (e == GameStateEnum.Ended)
        {
            m_resetEvent.WaitOne(TimeSpan.FromSeconds(10));
            gameEnd(this, null);
        }
    }

    private void initBoardState(IBoardQuery boardQuery)
    {
        m_dispatcher.Invoke(() =>
                            {
                                BoardState boardState = boardQuery.GetBoardState();
                                foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
                                {
                                    Board.AddTool(pair.Value, pair.Key);
                                }
                            });
    }

    protected override async void onSquareClick(object?         sender
                                              , SquareViewModel squareVM)
    {
        s_log.DebugFormat("Click on square: {0}", squareVM);

        bool isLocalMachineTeamTurn = m_teamManager.IsLocalMachineTeamTurn();

        if (!isLocalMachineTeamTurn)
        {
            return;
        }

        ITool         tool                = squareVM.Tool;
        BoardPosition position            = squareVM.Position;
        TeamId        localMachineTeamId  = m_teamManager.LocalMachineTeamId;
        bool          isToolBelongsToTeam = null != tool && m_teamManager.GetTeamId(tool.ToolId).Equals(localMachineTeamId);
        if (isToolBelongsToTeam)
        {
            Board.ClearSelectedAndHintedBoardPositions();
            Board.SelectedBoardPosition = position;
            BoardPosition[] availablePositionsToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
            Board.SetHintedBoardPosition(availablePositionsToMove);
            return;
        }

        if (false == Board.SelectedBoardPosition.IsEmpty())
        {
            BoardPosition start      = Board.SelectedBoardPosition;
            BoardPosition end        = position;
            await m_gameBoard.Move(start, end);
        }

        Board.ClearSelectedAndHintedBoardPositions();
    }

    protected override async void onPromotion(PromotionRequest promotionRequest)
    {
        BoardPosition position      = promotionRequest.Position;
        ITool         toolToPromote = promotionRequest.ToolToPromote;
        s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");
        PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
        await m_dispatcher.Invoke(
                                  async () =>
                                  {
                                      promotionMessage.ToolAwaiter.Start();
                                      Message = promotionMessage;
                                      ITool newTool = await promotionMessage.ToolAwaiter;
                                      Message = null;
                                      await m_gameBoard.PromoteTool(position, newTool);
                                  });
    }

    protected override void onCheckMate(CheckMateData checkMateData)
    {
        s_log.Info($"Checkmate Event: Position: {checkMateData}");
        m_resetEvent.Reset();

        UserMessageViewModel checkMateMessage = new("Checkmate", "OK", () =>
                                                                       {
                                                                           s_log.Info("OK Clicked");
                                                                           if (false == m_resetEvent.Set())
                                                                           {
                                                                               s_log.Error("Failed to set conditional variable");
                                                                           };
                                                                       });
        m_dispatcher.Invoke(
                            () =>
                            {
                                Message = checkMateMessage;
                            });
    }
}