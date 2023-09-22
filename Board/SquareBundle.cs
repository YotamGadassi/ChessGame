using Tools;

namespace Board;

public class SquareBundle
{
    public ITool         Tool     { get; }
    public BoardPosition Position { get; }

    public SquareBundle(ITool          tool
                       , BoardPosition position)
    {
        Tool     = tool;
        Position = position;
    }
}