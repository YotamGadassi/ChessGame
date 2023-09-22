using Tools;

namespace Board;

public interface IBoardQuery
{
    bool TryGetTool(BoardPosition position, out ITool         tool);
    bool TryGetPosition(ITool     tool,     out BoardPosition position);
    BoardState GetBoardState();
}