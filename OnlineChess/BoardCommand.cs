using Board;
using Tools;

namespace OnlineChess
{
    public enum BoardCommandType
    {
        Add
      , Remove
    }

    public class BoardCommand
    {
        public BoardCommandType Type { get; }
        public BoardPosition Position { get; }
        public ITool? Tool { get; }

        public BoardCommand(BoardCommandType type
                          , BoardPosition position
                          , ITool? tool = null)
        {
            Type = type;
            Position = position;
            Tool = tool;
        }
    }
}