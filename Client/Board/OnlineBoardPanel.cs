using ChessGame;

namespace Client.Board
{
    public class OnlineBoardPanel : OfflineBoardPanel
    {
        private readonly Team m_currentMachineTeam;
        public OnlineBoardPanel(OnlineGameManager gameManager) : base(gameManager)
        {
            m_currentMachineTeam = gameManager.CurrentMachineTeam;
        }

        protected override void clickCommand(object sender, BoardClickEventArgs args)
        {
            if (m_gameManager.CurrentTeamTurn.Color != m_currentMachineTeam.Color)
            {
                return;
            }
            base.clickCommand(sender, args);
        }
    }
}
