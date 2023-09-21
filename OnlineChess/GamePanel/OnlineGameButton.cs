using System.Windows.Threading;
using FrontCommon;

namespace OnlineChess.GamePanel;

public class OnlineGameButton : BaseGameButton, IDisposable
{
    public override string PanelGameName => "OnlineChessGame";
    public override string CommandName   => "Online Chess Game";

    private OnlineGameRequestManager m_requestGameRequestManager;
    private IChessServerAgent m_serverAgent;

    public OnlineGameButton(Dispatcher               dispatcer
                          , IGamePanelManager        panelManager
                          , OnlineGameRequestManager requestGameRequestManager) : base(dispatcer, panelManager)
    {
        m_requestGameRequestManager                =  requestGameRequestManager;
    }

    public void Dispose()
    {
        m_requestGameRequestManager.StartGameEvent -= onGameStart;
    }
    protected override async void playCommandExecute(object parameter)
    {
        string userName = "A";
        m_requestGameRequestManager.RequestGame(userName);
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
        //TODO: Remove Waiting For Connection Panel\Message
        m_panelManager.Show(panel);
    }


}