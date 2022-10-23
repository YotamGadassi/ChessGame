using System.Windows.Media;
using Common;

namespace Tools
{
    public class Rook : ITool
    {
        public Color Color { get; }

        public string Type => "Rook";

        public Rook(Color Color_)
        {
            Color = Color_;
        }

        public ITool GetCopy()
        {
            return new Rook(Color);
        }
    }
}
