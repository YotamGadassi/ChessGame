using ChessGame;
using Common;
using Common_6;

namespace Client.Board
{
    public class OnlineBoardViewModel : OfflineBoardViewModel
    {
        private readonly Team                 m_currentMachineTeam;
        public OnlineBoardViewModel(BaseGameManager gameManager, Team currentMachineTeam) : base(gameManager)
        {
            m_currentMachineTeam = currentMachineTeam;
        }

        protected override void ClickCommandExecute(BoardPosition position, ITool? tool)
        {
            if (m_gameManager.CurrentColorTurn != m_currentMachineTeam.Color)
            {
                return;
            }
            base.ClickCommandExecute(position, tool);
        }
    }
}
