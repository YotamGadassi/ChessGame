using FrontCommon;
using FrontCommon.GamePanel;
using log4net;
using OnlineChess.Common;
using OnlineChess.Game;

namespace OnlineChess.UI;

public class OnlineGameButtonViewModel : BaseGameButtonViewModel
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineGameButtonViewModel));
    public override string PanelGameName => "OnlineChessGame";
    public override string CommandName => "Online Chess Game";

    private readonly OnlineGameRequestManager m_gameRequestManager;

    public OnlineGameButtonViewModel(IGamePanelManager        panelManager
                          , OnlineGameRequestManager gameGameRequestManager) : base(panelManager)
    {
        m_gameRequestManager = gameGameRequestManager;
    }

    protected override void playCommandExecute(object parameter)
    {
        base.playCommandExecute(parameter);
        sendGameRequest();
    }

    private async void sendGameRequest()
    {
        string      userName    = "A"; //TODO: use parameter as object name
        GameRequest gameRequest = new GameRequest(userName);
        //TODO: show message
        try
        {
            await m_gameRequestManager.SubmitGameRequest(gameRequest);
        }
        catch (Exception e)
        {
            s_log.Error(e.Message);
            //TODO: Handle Error
        }
        //TODO: Show Waiting For Connection Panel\Message
    }

}