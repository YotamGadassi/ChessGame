using Board;
using ChessGame;
using Common;
using log4net;
using OnlineChess.Common;
using OnlineChess.TeamManager;

namespace OnlineChess.Game;

public class OnlineChessGameManager : IChessGameManager, IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGameManager));

    public IGameEvents  GameEvents  { get; }
    public IBoardEvents BoardEvents => GameBoard;
    public IBoardQuery  BoardQuery  => GameBoard;

    public IGameState?            GameState          { get; }
    public IChessTeamManager      TeamsManager       => m_teamManager;
    public OnlineChessTeamManager OnlineTeamsManager => m_teamManager;
    public OnlineGameBoard        GameBoard          { get; }

    private readonly OnlineChessTeamManager m_teamManager;

    public OnlineChessGameManager(OnlineGameBoard        gameBoard
                                , OnlineChessTeamManager teamManager
                                , OnlineGameEvents       gameEvents
                                , IGameState?            gameState)
    {
        GameBoard     = gameBoard;
        m_teamManager = teamManager;
        GameState     = gameState;
        GameEvents    = gameEvents;
        s_log.Info("Created");
    }

    public void Dispose()
    {
        GameBoard.Dispose();
        m_teamManager.Dispose();
        GameState.Dispose();
    }
}