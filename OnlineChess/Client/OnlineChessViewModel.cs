using System.Windows.Threading;
using Board;
using ChessGame.Helpers;
using Client.Board;
using Client.Game;
using Client.Messages;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using OnlineChess.GamePanel;
using Tools;

namespace OnlineChess.Client;

public class OnlineChessViewModel : ChessGameViewModel
{
    private static readonly ILog                        s_log = LogManager.GetLogger(typeof(OnlineChessViewModel));
    public override         BaseGameControllerViewModel ControllerViewModel { get; }

    private OnlineChessGameManager m_gameManager;
    private OnlineChessBoardProxy  m_boardProxy;
    private OnlineChessTeamManager m_teamManager;
    private IAvailableMovesHelper  m_availableMovesHelper;
    private Dispatcher             m_dispatcher;

    public OnlineChessViewModel(OnlineChessGameManager gameManager) : base(gameManager.BoardEvents, gameManager.TeamsManager)
    {
        m_dispatcher           = Dispatcher.CurrentDispatcher;
        m_gameManager          = gameManager;
        m_boardProxy           = gameManager.BoardProxy;
        m_teamManager          = gameManager.TeamsManager;
        m_availableMovesHelper = new AvailableMovesHelper(gameManager.BoardQuery);
        initBoardState(gameManager.BoardQuery);
    }

    private void initBoardState(IBoardQuery boardQuery)
    {
        BoardState boardState = boardQuery.GetBoardState();
        foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
        {
            BoardViewModel.AddTool(pair.Value, pair.Key);
        }
    }

    protected override async void onSquareClick(object?         sender
                                              , SquareViewModel squareVM)
    {
        s_log.DebugFormat("Click on square: {0}", squareVM);

        bool isLocalMachineTeamTurn = m_teamManager.IsLocalMachineTeamTurn();

        if (isLocalMachineTeamTurn)
        {
            ITool         tool                = squareVM.Tool;
            BoardPosition position            = squareVM.Position;
            Team          localMachineTeam    = m_teamManager.LocalMachineTeam;
            bool          isToolBelongsToTeam = null != tool && tool.Color.Equals(localMachineTeam);
            if (isToolBelongsToTeam)
            {
                BoardViewModel.ClearSelectedAndHintedBoardPositions();
                BoardViewModel.SelectedBoardPosition = position;
                BoardPosition[] availablePositionsToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
                BoardViewModel.SetHintedBoardPosition(availablePositionsToMove);
                return;
            }

            if (false == BoardViewModel.SelectedBoardPosition.IsEmpty())
            {
                BoardPosition start      = BoardViewModel.SelectedBoardPosition;
                BoardPosition end        = position;
                MoveResult    moveResult = await m_boardProxy.Move(start, end);
                handleMoveResult(moveResult);
            }

            BoardViewModel.ClearSelectedAndHintedBoardPositions();
        }
    }

    protected override async void onPromotion(BoardPosition position
                                            , ITool         toolToPromote)
    {
        s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");

        PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
        Message = promotionMessage;

        ITool chosenTool = await promotionMessage.ToolAwaiter;
        Message = null;

        PromotionResult promoteResult = await m_boardProxy.RequestPromotion(position, chosenTool);
        handlePromotionResult(promoteResult);
    }

    protected override void onCheckMate(BoardPosition position
                                           , ITool         tool)
    {
        s_log.Info($"Checkmate Event: Position:{position} | Tool:{tool}");

        // UserMessageViewModel checkMateMessage = new UserMessageViewModel("Checkmate", "OK", () =>
        //                                                                                     {
        //                                                                                         Message = null;
        //                                                                                         m_dispatcher
        //                                                                                            .InvokeAsync(() => m_chessGameManager.GameStateController.EndGame());
        //                                                                                     });
        // Message = checkMateMessage;
    }
}