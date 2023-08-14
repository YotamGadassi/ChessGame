using Board;
using Tools;

namespace Common.Chess
{
    public class PositionAndToolBundle
    {
        public BoardPosition Position { get; }
        public ITool Tool { get; }

        public PositionAndToolBundle(BoardPosition position, ITool tool)
        {
            Position = position;
            Tool = tool;
        }
    }
}
