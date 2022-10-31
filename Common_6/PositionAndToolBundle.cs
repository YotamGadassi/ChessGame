using System.Windows.Media;

namespace Common
{
    public class PositionAndToolBundle
    {
        public  BoardPosition Position { get; }
        public ITool         Tool     { get; }

        public PositionAndToolBundle(BoardPosition position, ITool tool)
        {
            Position = position;
            Tool     = tool;
        }
    }
}
