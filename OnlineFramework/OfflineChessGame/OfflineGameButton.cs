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


}