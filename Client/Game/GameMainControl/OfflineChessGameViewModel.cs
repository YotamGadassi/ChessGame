using System.Windows.Threading;
using Board;
using ChessGame;
using ChessGame.Helpers;
using Client.Board;
using Client.Game.GameMainControl;
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
        public                  BaseGameControllerViewModel ControllerViewModel => m_gameControllerVM;

        private readonly OfflineChessGameManager m_chessGameManager;
        private readonly IAvailableMovesHelper   m_availableMovesHelper;
        private readonly Dispatcher              m_dispatcher;
        private readonly GameControllerViewModel m_gameControllerVM;

        public OfflineChessGameViewModel(OfflineChessGameManager gameManager) : base(gameManager, gameManager.BoardEvents, gameManager.TeamsManager)
        {
            m_dispatcher           = Dispatcher.CurrentDispatcher;
            m_chessGameManager     = gameManager;
            m_availableMovesHelper = new AvailableMovesHelper(gameManager.BoardQuery);
            m_gameControllerVM     = new GameControllerViewModel(gameManager.GameStateController);
        }

        public override void Dispose()
        {
            base.Dispose();
            m_gameControllerVM.Dispose();
        }

        protected override void onSquareClick(object?         sender
                                                   , SquareViewModel squareVM)
        {
            s_log.DebugFormat("Click on square: {0}", squareVM);

            ITool?        tool           = squareVM.Tool;
            BoardPosition position       = squareVM.Position;
            TeamId        teamTurnId = m_chessGameManager.TeamsManager.CurrentTeamTurnId;

            bool isToolBelongsToTeam = null != tool && m_chessGameManager.TeamsManager.GetTeamId(tool.ToolId).Equals(teamTurnId);
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
                m_chessGameManager.Move(start, end);
            }

            Board.ClearSelectedAndHintedBoardPositions();
        }

        protected override async void onPromotion(BoardPosition position
                                                , ITool         toolToPromote)
        {
            s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");

            PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
            Message = promotionMessage;

            ITool newTool = await promotionMessage.ToolAwaiter;
            Message = null;

            m_chessGameManager.Promote(position, newTool);
        }

        protected override void onCheckMate(BoardPosition position
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