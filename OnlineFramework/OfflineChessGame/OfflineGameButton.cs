using System.Windows.Threading;
using FrontCommon;

namespace Frameworks.ChessGame;

public class OfflineGameButton : BaseGameButton
{
    public override string PanelGameName => "OfflineChessGame";
    public override string CommandName   => "Offline Chess Game";

    public OfflineGameButton(Dispatcher dispatcher, IGamePanelManager panelManager) : base(dispatcher, panelManager)
    {

    }

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