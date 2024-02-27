using Board;
using Common;
using log4net;
using OnlineChess.Common;
using OnlineChess.TeamManager;

namespace OnlineChess.Game;

public class OnlineChessGameManager : IChessGameEvents, IDisposable
{
    private static readonly ILog           s_log = LogManager.GetLogger(typeof(OnlineChessGameManager));
    
    public event AskPromotionEventHandler? AskPromotionEvent
    {
        add => m_serverAgent.AskPromotionEvent += value;
        remove => m_serverAgent.AskPromotionEvent -= value;
    }

    public event CheckMateEventHandler? CheckMateEvent
    {
        add => m_serverAgent.CheckMateEvent += value;
        remove => m_serverAgent.CheckMateEvent -= value;
    }

    public IBoardEvents                 BoardEvents => GameBoard;
    public IBoardQuery                  BoardQuery  => GameBoard;

    public IGameState GameState { get; }
    public OnlineChessTeamManager TeamsManager { get; }
    public OnlineGameBoard GameBoard { get; }

    private IChessServerAgent m_serverAgent;

    public OnlineChessGameManager(IChessServerAgent      serverAgent
                                , OnlineGameBoard        gameBoard
                                , OnlineChessTeamManager teamManager
                                , IGameState             gameState)
    {
        m_serverAgent = serverAgent;
        GameBoard     = gameBoard;
        TeamsManager  = teamManager;
        GameState     = gameState;
        s_log.Info("Created");
    }

    public void Dispose()
    {
        GameBoard.Dispose();
        TeamsManager.Dispose();
        GameState.Dispose();
    }
}