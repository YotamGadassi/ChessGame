using Tools;

namespace Board;

public interface IBoard
{
    void Add(BoardPosition        position, ITool tool);
    bool Remove(BoardPosition     position);
    void Clear();

    BoardState GetBoard { get; }
}