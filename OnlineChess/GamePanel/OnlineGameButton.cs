using System.Windows.Threading;
using FrontCommon;
using log4net;

namespace OnlineChess.GamePanel;

public class OnlineGameButton : BaseGameButton, IDisposable
{
    private static readonly ILog   s_log = LogManager.GetLogger(typeof(OnlineGameButton));
    public override         string PanelGameName => "OnlineChessGame";
    public override         string CommandName   => "Online Chess Game";

    private OnlineGameRequestManager m_requestGameRequestManager;
    private IChessServerAgent m_serverAgent;

    public OnlineGameButton(Dispatcher               dispatcer
                          , IGamePanelManager        panelManager
                          , OnlineGameRequestManager requestGameRequestManager) : base(dispatcer, panelManager)
    {
        m_requestGameRequestManager                =  requestGameRequestManager;
        registerToEvents();
    }

    public void Dispose()
    {
        unRegisterFromEvent();
    }

    protected override async void playCommandExecute(object parameter)
    {
        string userName = "A"; //TODO: use parameter as object name
        //TODO: show message
        try
        {
            await m_requestGameRequestManager.RequestGame(userName);
        }
        catch (Exception e)
        {
            s_log.Error(e.Message);
            //TODO: Handle Error
        }
        //TODO: Show Waiting For Connection Panel\Message
    }

    private void registerToEvents()
    {
        m_requestGameRequestManager.StartGameEvent += onGameStart;
    }

    private void unRegisterFromEvent()
    {
        m_requestGameRequestManager.StartGameEvent -= onGameStart;
    }

    private void onGameStart(OnlineChessGameManager onlineGameManager)
    {
        BaseGamePanel         panel           = getPanel();
        if (panel is OnlineChessGamePanel onlineGamePanel)
        {
            onlineGamePanel.SetGameManager(onlineGameManager);
        }
        panel.Init();
        //TODO: Remove Waiting For Connection Panel\Message
        m_panelManager.Show(panel);
    }


}