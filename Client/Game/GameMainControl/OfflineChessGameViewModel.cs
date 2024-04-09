using System.Windows.Threading;
using Board;
using ChessGame;
using ChessGame.Helpers;
using Client.Board;
using Client.Messages;
using Common;
using FrontCommon;
using log4net;
using Tools;

namespace Client.Game.GameMainControl
{
    public class OfflineChessGameViewModel : BaseChessGameViewModel
    {
        private static readonly ILog s_log = LogManager.GetLogger(typeof(OfflineChessGameViewModel));
        public                  BaseGameControllerViewModel ControllerViewModel => m_gameControllerVm;

        private readonly OfflineChessGameManager m_chessGameManager;
        private readonly IAvailableMovesHelper   m_availableMovesHelper;
        private readonly Dispatcher              m_dispatcher;
        private readonly GameControllerViewModel m_gameControllerVm;
        private          bool                    m_isCheckMate;

        public OfflineChessGameViewModel(OfflineChessGameManager gameManager) : base(gameManager)
        {
            m_isCheckMate          = false;
            m_dispatcher           = Dispatcher.CurrentDispatcher;
            m_chessGameManager     = gameManager;
            m_availableMovesHelper = new AvailableMovesHelper(gameManager.BoardQuery);
            m_gameControllerVm     = new GameControllerViewModel(gameManager.GameStateController);
            registerToEvents();
        }

        public override void Dispose()
        {
            base.Dispose();
            m_gameControllerVm.Dispose();
            unRegisterFromEvens();
        }

        protected override void onSquareClick(object?         sender
                                            , SquareViewModel squareVM)
        {
            s_log.DebugFormat("Click on square: {0}", squareVM);

            ITool?        tool       = squareVM.Tool;
            BoardPosition position   = squareVM.Position;
            TeamId        teamTurnId = m_chessGameManager.TeamsManager.CurrentTeamTurnId;

            bool isToolBelongsToTeam = null != tool
                                    && m_chessGameManager.TeamsManager.GetTeamId(tool.ToolId).Equals(teamTurnId);
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
                BoardPosition start = Board.SelectedBoardPosition;
                BoardPosition end   = position;
                m_chessGameManager.Move(start, end);
            }

            Board.ClearSelectedAndHintedBoardPositions();
        }
        
        protected override async void onPromotion(PromotionRequest promotionRequest)
        {
            BoardPosition position      = promotionRequest.Position;
            ITool         toolToPromote = promotionRequest.ToolToPromote;
            s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");

            PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
            promotionMessage.ToolAwaiter.Start();
            Message = promotionMessage;

            ITool newTool = await promotionMessage.ToolAwaiter;
            Message = null;

            m_chessGameManager.Promote(position, newTool);
        }

        protected override void onCheckMate(CheckMateData checkMateData)
        {
            m_isCheckMate = true;
            s_log.Info($"Checkmate Event: [{checkMateData}");

            UserMessageViewModel checkMateMessage = new("Checkmate", "OK", () =>
                                                                           {
                                                                               Message = null;
                                                                               m_dispatcher
                                                                                  .InvokeAsync(() =>
                                                                                       gameEnd(this
                                                                                         , null));
                                                                           });
            Message = checkMateMessage;
        }

        private void registerToEvents()
        {
            m_chessGameManager.GameStateController.StateChanged += onStateChanged;
        }

        private void unRegisterFromEvens()
        {
            m_chessGameManager.GameStateController.StateChanged -= onStateChanged;
        }

        private void onStateChanged(object?       sender
                                  , GameStateEnum e)
        {
            if (e == GameStateEnum.Ended && false == m_isCheckMate)
            {
                gameEnd(this, null);
            }
        }
    }
}