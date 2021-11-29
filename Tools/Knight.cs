using ChessBoard;
using System;
using System.Windows.Media;

namespace Tools
{
    public class Knight : ITool
    {
        public string Type => "Knight";

        public Color Color { get; }

        public Knight(Color Color_)
        {
            Color = Color_;
        }

        public ITool GetCopy()
        {
            Knight newKnight = new Knight(Color);

            return newKnight;
        }

    }
}
