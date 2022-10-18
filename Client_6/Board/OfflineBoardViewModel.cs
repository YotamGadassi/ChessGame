using System.Windows.Media;
using ChessGame;
using Client.Helpers;
using Common_6;

namespace Client.Board
{
    public class OfflineBoardViewModel : BaseBoardViewModel
    {
        protected AvailableMovesHelper m_availableMovesHelper;

        public OfflineBoardViewModel(BaseGameManager gameManager) : base(gameManager)
        {
            m_availableMovesHelper = new AvailableMovesHelper(gameManager);
        }

        protected override void ClickCommandExecute(BoardPosition position, ITool? tool)
        {
            Color currTeamColor          = m_gameManager.CurrentColorTurn;
            bool  isPositionToolSameTeam = null != tool && tool.Color.Equals(currTeamColor);

            if (isPositionToolSameTeam)
            {
                ClearSelectedAndHintedBoardPositions();
                SelectedBoardPosition = position;
                BoardPosition[] positionToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
                SetHintedBoardPosition(positionToMove);
                return;
            }

            if (false == SelectedBoardPosition.IsEmpty())
            {
                m_gameManager.Move(SelectedBoardPosition, position);
            }
            ClearSelectedAndHintedBoardPositions();
            base.ClickCommandExecute(position, tool);
        }
    }
}
