using System.Windows.Media;

namespace Common
{
    public class PositionAndToolBundle
    {
        public BoardPosition Position  { get; }
        public string        ToolName  { get; }
        public Color        ToolColor { get; }

        public PositionAndToolBundle(BoardPosition position
                                    , string       toolName
                                    , Color        toolColor)
        {
            Position  = position;
            ToolName  = toolName;
            ToolColor = toolColor;
        }
    }
}
