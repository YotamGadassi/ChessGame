using System.Windows.Media;
using Common_6;

namespace Tools
{
    public class Pawn : ITool
    {
        public string Type => "Pawn";

        public Color Color { get; }

        public Pawn(Color Color_)
        {
            Color = Color_;
        }

        public ITool GetCopy()
        {
            Pawn newPawn = new Pawn(Color);

            return newPawn;
        }

    }
}
