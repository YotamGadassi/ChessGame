using System.Windows.Media;
using Common_6;

namespace Tools
{
    public class Bishop : ITool
    {
        public Color Color { get; }

        public string Type => "Bishop";

        public Bishop(Color Color_)
        {
            Color = Color_;
        }

        public ITool GetCopy()
        {
            return new Bishop(Color);
        }
    }
}
