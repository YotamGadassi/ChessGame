using Board;
using Common;
using log4net;

namespace OnlineChess.GamePanel;

public class OnlineChessGameManager : IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGameManager));

    public IBoardEvents BoardEvents => m_gameBoard;

    public OnlineChessTeamManager TeamsManager => m_teamManager;

    public OnlineChessBoardProxy BoardProxy { get; }

    public IBoardQuery BoardQuery => m_gameBoard;

    private OnlineGameBoard        m_gameBoard;
    private OnlineChessTeamManager m_teamManager;

    private IChessServerAgent m_serverAgent;

    public OnlineChessGameManager(OnlineGameBoard        gameBoard
                                , OnlineChessTeamManager teamManager
                                , IChessServerAgent      serverAgent)
    {
        m_gameBoard   = gameBoard;
        m_teamManager = teamManager;
        m_serverAgent = serverAgent;
        BoardProxy    = new OnlineChessBoardProxy(m_serverAgent);

        s_log.Info("Created");
    }

    public void Dispose()
    {
        m_gameBoard.Dispose();
        m_teamManager.Dispose();
    }
}