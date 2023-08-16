using System.Threading.Tasks;
using System.Windows.Media;
using Board;
using Common;
using Common.Chess;
using FrontCommon;
using Tools;

namespace Client.Game
{
    public class OfflineChessGameViewModel : ChessGameViewModel
    {
        private IAsyncAvailableMovesHelper m_availableMovesHelper;
        private IAsyncChessGameManager     m_chessGameManager;

        public OfflineChessGameViewModel(IChessGameManager     gameManager
                                       , IAvailableMovesHelper availableMovesHelper) :
            base(gameManager, new AvailableMovesHelperWrapper(availableMovesHelper))
        {
            m_availableMovesHelper = availableMovesHelper;
            m_chessGameManager     = gameManager;
        }

        protected override void onSwitchTeamEvent(Color currentTeamTurn)
        {
            // Do nothing
        }
    }

    public class AvailableMovesHelperWrapper : IAsyncAvailableMovesHelper
    {

        IAvailableMovesHelper m_availableMovesHelper;

        public AvailableMovesHelperWrapper(IAvailableMovesHelper availableMovesHelper)
        {
            m_availableMovesHelper = availableMovesHelper;
        }

        public Task<bool> ValidatePositionOnBoard(BoardPosition position) => Task.FromResult(m_availableMovesHelper.ValidatePositionOnBoard(position));


        public Task<BoardPosition[]> GetAvailablePositionToMove(BoardPosition position) =>
            Task.FromResult(m_availableMovesHelper.GetAvailablePositionToMove(position));
    }

    public class ChessGameManagerWrapper : IAsyncChessGameManager
    {
        private IChessGameManager m_chessGameManager;

        public ChessGameManagerWrapper(IChessGameManager chessGameManager)
        {
            m_chessGameManager = chessGameManager;
        }

        public Task<Color> CurrentColorTurn => Task.FromResult(m_chessGameManager.CurrentColorTurn);
        public Task<bool>  IsGameRunning    => Task.FromResult(m_chessGameManager.IsGameRunning);

        public Task StartGame()
        {
            m_chessGameManager.StartGame();
            return Task.CompletedTask;
        }

        public Task EndGame()
        {
            m_chessGameManager.EndGame();
            return Task.CompletedTask;
        }

        public Task<bool> TryGetTool(BoardPosition position
                                   , out ITool     tool)
        {
            throw new System.NotImplementedException();
        }

        public Task<MoveResult> Move(BoardPosition start
                                   , BoardPosition end)
        {
            throw new System.NotImplementedException();
        }

        public Task<PromotionResult> Promote(BoardPosition start
                                           , ITool         newTool)
        {
            throw new System.NotImplementedException();
        }
    }
}