using Common;

namespace Board;

public interface IBoard
{
    void Add(BoardPosition        position, ITool tool);
    bool Remove(BoardPosition     position);
    bool TryGetTool(BoardPosition position, out ITool         tool);
    bool TryGetPosition(ITool     tool,     out BoardPosition position);
    void Clear();
}