using System.Windows.Threading;
using FrontCommon;

namespace Frameworks.OnlineChessGame
{
    public class OnlineChessGameButton : BaseGameButton
    {
        public override string PanelGameName => "OnlineChessGame";
        public override string CommandName   => "Online Chess Game";

        public OnlineChessGameButton(Dispatcher        dispatcer
               , IGamePanelManager panelManager) : base(dispatcer, panelManager) { }

        protected override BaseGamePanel getPanel()
        {
            bool isExists = m_panelManager.TryGetPanel(PanelGameName, out BaseGamePanel panel);
            if (false == isExists)
            {
                throw new KeyNotFoundException($"Panel: {PanelGameName} does not exist");
            }
            return panel;
        }
    }
}
