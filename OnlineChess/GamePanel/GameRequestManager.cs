using Frameworks;
using FrontCommon;

namespace OnlineChess.GamePanel;

public class OnlineGameRequestManager
{
    public event StartGameHandler StartGameEvent
    {
        add => m_serverAgent.StartGameEvent += value;
        remove => m_serverAgent.StartGameEvent -= value;
    }
    public event EndGameHandler EndGameEvent
    {
        add => m_serverAgent.EndGameEvent += value;
        remove => m_serverAgent.EndGameEvent -= value;
    }

    private IChessConnctionManager m_connctionManager;
    private IChessServerAgent m_serverAgent;


    public OnlineGameRequestManager(IChessConnctionManager connctionManager)
    {
        m_connctionManager = connctionManager;
        m_serverAgent      = connctionManager.ServerAgent;
    }

    public async void RequestGame(string userName)
    {
        if (m_connctionManager.ConnectionStatus == ConnectionStatus.Disconnected)
        {
            bool isConnectedSuccefully = await m_connctionManager.Connect();
            //TODO: Handle error
        }

        bool isRequestSuccessfully = await m_serverAgent.RequestGame(userName);
        //TODO: Handle error
    }

}