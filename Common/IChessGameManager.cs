using System.Threading.Tasks;
using System.Windows.Media;
using Board;
using Common.Chess;
using Tools;

namespace Common
{
    public interface IChessGameManager
    {
        Color CurrentColorTurn { get; }
        bool  IsGameRunning    { get; }
        void  StartGame();
        void  EndGame();

        bool TryGetTool(BoardPosition position
                      , out ITool     tool);

        MoveResult Move(BoardPosition start
                      , BoardPosition end);

        PromotionResult Promote(BoardPosition start
                              , ITool         newTool);
    }

    public interface IAsyncChessGameManager
    {
        Task<Color> CurrentColorTurn { get; }
        Task<bool>  IsGameRunning    { get; }
        Task        StartGame();
        Task        EndGame();

        Task<bool> TryGetTool(BoardPosition position
                            , out ITool     tool);

        Task<MoveResult> Move(BoardPosition start
                            , BoardPosition end);

        Task<PromotionResult> Promote(BoardPosition start
                                    , ITool         newTool);
    }
}