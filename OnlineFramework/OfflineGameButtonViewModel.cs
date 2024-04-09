using FrontCommon;
using FrontCommon.GamePanel;

namespace OfflineChess;

public class OfflineGameButtonViewModel : BaseGameButtonViewModel
{
    public override string PanelGameName => "OfflineChessGame";
    public override string CommandName => "Offline Chess Game";

    public OfflineGameButtonViewModel(IGamePanelManager panelManager) : base(panelManager)
    {

    }


}