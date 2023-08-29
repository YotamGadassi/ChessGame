using System.Windows.Media;
using System.Windows.Threading;
using Board;
using ChessGame;
using ChessGame.Helpers;
using Client.Board;
using Client.Messages;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Tools;

namespace Client.Game
{
    public class OfflineChessGameViewModel : ChessGameViewModel
    {
        private static readonly ILog s_log = LogManager.GetLogger(typeof(OfflineChessGameViewModel));

        private IChessGameManager     m_chessGameManager;
        private IAvailableMovesHelper m_availableMovesHelper;
        private Dispatcher            m_dispatcher;

        public override BaseGameControllerViewModel ControllerViewModel { get; }

        public OfflineChessGameViewModel(OfflineChessGameManager gameManager) : base(gameManager.BoardEvents, gameManager.TeamsManager)
        {
            m_dispatcher           = Dispatcher.CurrentDispatcher;
            m_chessGameManager     = gameManager;
            m_availableMovesHelper = new AvailableMovesHelper(gameManager);
            ControllerViewModel    = new GameControllerViewModel(gameManager.GameStateController);
        }

        protected override void onSqualeClickHandler(object?         sender
                                                   , SquareViewModel squareVM)
        {
            s_log.DebugFormat("Click on square: {0}", squareVM);

            ITool?        tool          = squareVM.Tool;
            BoardPosition position      = squareVM.Position;
            Color         currTeamColor = m_chessGameManager.CurrentTeamTurn.Color;

            bool isToolBelongsToTeam = null != tool && tool.Color.Equals(currTeamColor);
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
                MoveResult    moveResult = m_chessGameManager.ChessBoardProxy.Move(start, end);
                handleMoveResult(moveResult);
            }

            BoardViewModel.ClearSelectedAndHintedBoardPositions();
        }

        protected override async void onPromotionEvent(BoardPosition position
                                                     , ITool         toolToPromote)
        {
            s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");

            PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
            Message = promotionMessage;

            ITool chosenTool = await promotionMessage.ToolAwaiter;
            Message = null;

            PromotionResult promoteResult = m_chessGameManager.ChessBoardProxy.Promote(position, chosenTool);
            handlePromotionResult(promoteResult);
        }

        protected override void onCheckMateEvent(BoardPosition position
                                               , ITool         tool)
        {
            s_log.Info($"Checkmate Event: Position:{position} | Tool:{tool}");

            UserMessageViewModel checkMateMessage = new UserMessageViewModel("Checkmate", "OK", () =>
                                                                                                {
                                                                                                    Message = null;
                                                                                                    m_dispatcher
                                                                                                       .InvokeAsync(() => m_chessGameManager.GameStateController.EndGame());
                                                                                                });
            Message = checkMateMessage;
        }
    }
}