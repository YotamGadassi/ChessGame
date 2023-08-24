using Board;
using Common;
using Common.Chess;
using Common.GameInterfaces;
using log4net;
using Tools;

namespace ChessGame;

public class BaseChessGameManager : IChessGameManager
{
    private static readonly int        s_teamsAmount = 2;

    public event EventHandler<GameState>? StateChanged;
    public GameState                      State           { get; protected set; }
    public IBoardEvents                   BoardEvents     => m_gameBoard;
    public TeamWithTimer?                 CurrentTeamTurn => Teams?[m_currentTeamIndex];
    public TeamWithTimer[]?               Teams           { get; protected set; }

    protected readonly      ChessBoard m_gameBoard;
    private readonly        ILog       m_log;
    private                 int        m_currentTeamIndex;

    public BaseChessGameManager(ChessBoard gameBoard
                              , ILog       log)
    {
        State              = GameState.NotStarted;
        m_gameBoard        = gameBoard;
        m_log              = log;
        m_currentTeamIndex = 0;
    }

    public virtual bool StartResumeGame()
    {
        m_log.Info($"Game Started");
        return setState(GameState.Running);
    }

    public virtual bool PauseGame()
    {
        m_log.Info($"Game Paused");
        return setState(GameState.Paused);
    }

    public virtual void EndGame()
    {
        m_log.Info("End Game");

        m_gameBoard.Clear();
        m_currentTeamIndex = 0;
        setState(GameState.Ended);
    }

    public bool TryGetTool(BoardPosition position
                         , out ITool     tool)
    {
        return m_gameBoard.TryGetTool(position, out tool);
    }

    protected void setTeams(TeamWithTimer[] teams)
    {
        Teams = teams;
    }
    
    protected void switchCurrentTeam()
    {
        CurrentTeamTurn.StopTimer();
        m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
        CurrentTeamTurn.StartTimer();
    }

    protected bool setState(GameState state)
    {
        if (state == State)
            return false;

        State = state;
        StateChanged?.Invoke(this, state);
        return true;
    }
}