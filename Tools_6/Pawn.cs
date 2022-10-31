﻿using System.Windows.Media;
using Common;

namespace Tools
{
    public class Pawn : ITool
    {
        public string Type => "Pawn";

        public Color Color { get; }

        public Pawn(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            Pawn newPawn = new Pawn(Color);

            return newPawn;
        }

    }
}
