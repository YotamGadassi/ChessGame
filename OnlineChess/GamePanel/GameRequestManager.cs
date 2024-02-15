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

    private readonly IChessConnectionManager m_connctionManager;
    private readonly IChessServerAgent       m_serverAgent;

    public OnlineGameRequestManager(IChessConnectionManager connectionManager)
    {
        m_connctionManager = connectionManager;
        m_serverAgent      = connectionManager.ServerAgent;
    }

    public async Task RequestGame(string userName)
    {
        if (m_connctionManager.ConnectionStatus == ConnectionStatus.Disconnected)
        {
            bool isConnectedSuccessfully = await m_connctionManager.Connect();
            //TODO: Handle error
        }

        bool isRequestSuccessfully = await m_serverAgent.RequestGame(userName);
        //TODO: Handle error
    }

}