using System.Windows.Media;
using Common_6;

namespace Tools
{
    public class King : ITool
    {
        public Color Color { get; }

        public string Type => "King";

        public King(Color Color_)
        {
            Color = Color_;
        }

        public ITool GetCopy()
        {
            return new King(Color);
        }
    }
}
