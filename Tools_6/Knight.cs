using System.Windows.Media;
using Common;

namespace Tools
{
    public class Knight : ITool
    {
        public string Type => "Knight";

        public Color Color { get; }

        public Knight(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            Knight newKnight = new Knight(Color);

            return newKnight;
        }

    }
}
