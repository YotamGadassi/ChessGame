using FrontCommon;
using log4net;
using OnlineChess.Common;
using OnlineChess.ConnectionManager;

namespace OnlineChess.GamePanel;

public class OnlineGameRequestManager
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineGameRequestManager));

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

    private readonly IChessConnectionManager m_connectionManager;
    private readonly IChessServerAgent       m_serverAgent;

    public OnlineGameRequestManager(IChessConnectionManager connectionManager)
    {
        m_connectionManager = connectionManager;
        m_serverAgent      = connectionManager.ServerAgent;
        s_log.Debug("Created");
    }

    public async Task RequestGame(GameRequest gameRequest)
    {
        s_log.DebugFormat("Request Game: {0}",gameRequest);
        if (m_connectionManager.ConnectionStatus == ConnectionStatus.Disconnected)
        {
            bool isConnectedSuccessfully = await m_connectionManager.Connect();
            if (false == isConnectedSuccessfully)
            {
                //TODO: Handle error
                s_log.ErrorFormat("Can't established connection to server");
                return;
            }
        }

        GameRequestResult gameRequestResult = await m_serverAgent.SubmitGameRequest(gameRequest);
        if (gameRequestResult.IsError)
        {
            //TODO: Handle error
            s_log.ErrorFormat("Can't request game from server");
            return;
        }
        s_log.Info("Game request sent successfully to server");
    }

}