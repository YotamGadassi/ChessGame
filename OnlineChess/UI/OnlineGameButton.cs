using System.Windows.Threading;
using FrontCommon;
using log4net;
using OnlineChess.Common;
using OnlineChess.Game;

namespace OnlineChess.UI;

public class OnlineGameButton : BaseGameButton
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineGameButton));
    public override string PanelGameName => "OnlineChessGame";
    public override string CommandName => "Online Chess Game";

    private readonly OnlineGameRequestManager m_gameRequestManager;

    public OnlineGameButton(Dispatcher               dispatcher
                          , IGamePanelManager        panelManager
                          , OnlineGameRequestManager gameGameRequestManager) : base(dispatcher, panelManager)
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