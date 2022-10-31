using System.Windows.Media;
using Common;

namespace Tools
{
    public class King : ITool
    {
        public Color Color { get; }

        public string Type => "King";

        public King(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            return new King(Color);
        }
    }
}
