using System.Windows.Media;
using Board;
using ChessGame;
using ChessGame.Helpers;
using Client.Board;
using Client.Game;
using Client.Helpers;
using Client.Messages;
using Common;
using Common.Chess;
using Common.Chess.ChessBoardEventArgs;
using Tools;

namespace Frameworks
{
    public class OfflineFramework
    {
        public  OfflineGameViewModel ViewModel { get; }
        private IChessGameManager    m_gameManager;
        private AvailableMovesHelper m_availableMovesHelper;
        public OfflineFramework(Team northTeam
                              , Team southTeam)
        {
            m_gameManager = new OfflineChessGameManager();
            m_availableMovesHelper = new AvailableMovesHelper(m_gameManager);
            ViewModel     = new OfflineGameViewModel( northTeam, southTeam, SquareClickHandler, SquareClickHandlerCanExecute);
            m_gameManager.StartGame();
        }

        public void SquareClickHandler(BoardPosition position
                                     , ITool?        tool)
        {
            BoardViewModel board                  = ViewModel.Board;
            Color          currTeamColor          = m_gameManager.CurrentColorTurn;
            bool           isPositionToolSameTeam = null != tool && tool.Color.Equals(currTeamColor);
            BoardPosition  selectedBoardPosition  = board.SelectedBoardPosition;
            board.ClearSelectedAndHintedBoardPositions();

            if (isPositionToolSameTeam)
            {
                board.SelectedBoardPosition = position;
                BoardPosition[] positionToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
                board.SetHintedBoardPosition(positionToMove);
                return;
            }

            if (false == board.SelectedBoardPosition.IsEmpty())
            {
                MoveResult     moveResult     = m_gameManager.Move(selectedBoardPosition, position);
                MoveResultEnum moveResultEnum = moveResult.Result;
                if (moveResultEnum.HasFlag(MoveResultEnum.CheckMate))
                {
                    handleCheckMate();
                    return;
                }
            }
            // Board.ClearSelectedAndHintedBoardPositions();
        }

        public bool SquareClickHandlerCanExecute(BoardPosition poistion
                                               , ITool?        tool)
        {
            return m_gameManager.IsGameRunning;
        }

        private void onToolPromotedEvent(object?               sender
                                       , ToolPromotedEventArgs e)
        {
            ViewModel.Board.RemoveTool(e.ToolPosition, out _);
            ViewModel.Board.AddTool(e.ToolAfterPromotion, e.ToolPosition);
        }

        private async Task<ITool> onPromotionAsyncEvent(object             sender
                                                      , PromotionEventArgs e)
        {
            PromotionMessageViewModel promotionViewModel = new(e.ToolToPromote.Color, e.ToolPosition);
            ViewModel.Message = promotionViewModel;
            ITool tool = await promotionViewModel.ToolAwaiter;
            ViewModel.Message = null;
            return tool;
        }

        private void handleCheckMate()
        {
            ViewModel.Message = new UserMessageViewModel("Checkmate", "OK", () => ViewModel.Message = null);
        }
    }
}
