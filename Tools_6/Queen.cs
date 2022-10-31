using System.Windows.Media;
using Common;

namespace Tools
{
    public class Queen : ITool
    {
        public string Type => "Queen";

        public Color Color { get; }

        public Queen(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            Queen newQueen = new Queen(Color);

            return newQueen;
        }

    }
}
