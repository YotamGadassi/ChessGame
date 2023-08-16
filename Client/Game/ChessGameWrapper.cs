using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using Board;
using Common;
using Common.Chess;
using Tools;

namespace Client.Game;

public class ChessGameWrapper
{
    public Color CurrentColorTurn => m_chessGameManager.CurrentColorTurn;
    public bool  IsGameRunning => m_chessGameManager.IsGameRunning;

    private IChessGameManager m_chessGameManager;
    private Dispatcher        m_dispatcher;

    public ChessGameWrapper(IChessGameManager chessGameManager, Dispatcher dispatcher)
    {
        m_dispatcher       = dispatcher;
        m_chessGameManager = chessGameManager;
    }

    public void  StartGame()
    {
        m_dispatcher.Invoke(() => m_chessGameManager.StartGame());
    }

    public void EndGame()
    {
        m_dispatcher.Invoke(() => EndGame());
    }

    public bool TryGetTool(BoardPosition position
                         , out ITool     tool)
    {
        return m_chessGameManager.TryGetTool(position, out tool);
    }

    public Task<MoveResult> Move(BoardPosition start
                               , BoardPosition end)
    {
        DispatcherOperation<MoveResult>? operationResult = m_dispatcher.InvokeAsync(() => m_chessGameManager.Move(start, end));
        return operationResult.Task;
    }

    public Task<PromotionResult> Promote(BoardPosition position, ITool newTool)
    {
        DispatcherOperation<PromotionResult>? dispatcherOperation = m_dispatcher.InvokeAsync(() => m_chessGameManager.Promote(position, newTool));
        return dispatcherOperation.Task;
    }
}