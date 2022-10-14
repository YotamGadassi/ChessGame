using System.Windows.Media;
using ChessGame;
using Common_6;

namespace Client.Board
{
    public class OfflineBoardPanel : BaseBoardPanel
    {
        public OfflineBoardPanel(BaseGameManager gameManager) : base(gameManager)
        {
        }

        protected override void clickCommand(object sender, BoardClickEventArgs args)
        {
            Color         currTeamColor          = m_gameManager.CurrentTeamTurn.Color;
            ITool         tool                   = args.Tool;
            BoardPosition position               = args.Position;
            bool          isPositionToolSameTeam = null != tool && tool.Color.Equals(currTeamColor);

            if (isPositionToolSameTeam)
            {
                BoardVm.ClearSelectedAndHintedBoardPositions();
                BoardVm.SelectedBoardPosition = position;
                BoardPosition[] positionToMove = m_availableMoveHelper.GetAvailablePositionToMove(position);
                BoardVm.SetHintedBoardPosition(positionToMove);
                return;
            }

            if (false == BoardVm.SelectedBoardPosition.IsEmpty())
            {
                m_gameManager.Move(BoardVm.SelectedBoardPosition, position);
            }
            BoardVm.ClearSelectedAndHintedBoardPositions();
        }
    }
}
