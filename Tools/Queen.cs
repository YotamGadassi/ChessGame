using System.Windows.Media;
using ChessBoard;

namespace Tools
{
    public class Queen : ITool
    {
        public string Type => "Queen";

        public Color Color { get; }

        public Queen(Color Color_)
        {
            Color = Color_;
        }

        public ITool GetCopy()
        {
            Queen newQueen = new Queen(Color);

            return newQueen;
        }

    }
}
