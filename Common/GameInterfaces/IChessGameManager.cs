using Board;
using Tools;

namespace Common.GameInterfaces
{
    public interface IChessGameManager : IGameManager
    {
        IBoardEvents BoardEvents { get; }
        public TeamWithTimer CurrentTeamTurn { get; }

        public TeamWithTimer[] Teams { get; }

        bool TryGetTool(BoardPosition position
                      , out ITool tool);
    }
}