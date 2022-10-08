using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tools;

namespace Client.Helpers
{
    public static class ToolsImageHelper
    {
        private class TypeColorPair : Tuple<Type, Color>
        {
            public Type  Type  => Item1;
            public Color Color => Item2;
            
            public TypeColorPair(Type type, Color color) : base(type, color)
            {
            }

            private bool equals(TypeColorPair other)
            {
                if (null == other)
                {
                    return false;
                }

                return other.Type == Type && other.Color.Equals(Color);
            }

            public override bool Equals(object obj)
            {
                return equals(obj as TypeColorPair);
            }
        }

        private static readonly Dictionary<TypeColorPair, BitmapImage> m_ToolToImageSource =
            new Dictionary<TypeColorPair, BitmapImage>();

        static ToolsImageHelper()
        {
            TypeColorPair blackPawn = new TypeColorPair(typeof(Pawn), Colors.Black);
            BitmapImage   src       = new BitmapImage(new Uri("pack://application:,,,/Resources/b_pawn.png"));
            m_ToolToImageSource[blackPawn] = src;

            TypeColorPair blackRook = new TypeColorPair(typeof(Rook), Colors.Black);
            src                             = new BitmapImage(new Uri("pack://application:,,,/Resources/b_rook.png"));
            m_ToolToImageSource[blackRook ] = src;

            TypeColorPair blackBishop = new TypeColorPair(typeof(Bishop), Colors.Black);
            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_bishop.png"));
            m_ToolToImageSource[blackBishop] = src;

            TypeColorPair blackKnight = new TypeColorPair(typeof(Knight), Colors.Black);
            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_knight.png"));
            m_ToolToImageSource[blackKnight] = src;

            TypeColorPair blackQueen = new TypeColorPair(typeof(Queen), Colors.Black);
            src                             = new BitmapImage(new Uri("pack://application:,,,/Resources/b_queen.png"));
            m_ToolToImageSource[blackQueen] = src;

            TypeColorPair blackKing = new TypeColorPair(typeof(King), Colors.Black);
            src                            = new BitmapImage(new Uri("pack://application:,,,/Resources/b_king.png"));
            m_ToolToImageSource[blackKing] = src;

            TypeColorPair whitePawn = new TypeColorPair(typeof(Pawn), Colors.White);
            src                            = new BitmapImage(new Uri("pack://application:,,,/Resources/w_pawn.png"));
            m_ToolToImageSource[whitePawn] = src;

            TypeColorPair whiteRook = new TypeColorPair(typeof(Rook), Colors.White);
            src                            = new BitmapImage(new Uri("pack://application:,,,/Resources/w_rook.png"));
            m_ToolToImageSource[whiteRook] = src;

            TypeColorPair whiteBishop = new TypeColorPair(typeof(Bishop), Colors.White);
            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_bishop.png"));
            m_ToolToImageSource[whiteBishop] = src;

            TypeColorPair whiteKnight = new TypeColorPair(typeof(Knight), Colors.White);
            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_knight.png"));
            m_ToolToImageSource[whiteKnight] = src;

            TypeColorPair whiteQueen = new TypeColorPair(typeof(Queen), Colors.White);
            src                             = new BitmapImage(new Uri("pack://application:,,,/Resources/w_queen.png"));
            m_ToolToImageSource[whiteQueen] = src;

            TypeColorPair whiteKing = new TypeColorPair(typeof(King), Colors.White);

            src                            = new BitmapImage(new Uri("pack://application:,,,/Resources/w_king.png"));
            m_ToolToImageSource[whiteKing] = src;
        }

        public static bool TryGetBitmapImage(Type toolType, Color toolColor, out BitmapImage bitmapImage)
        {
            TypeColorPair pair = new TypeColorPair(toolType, toolColor);
            return m_ToolToImageSource.TryGetValue(pair, out bitmapImage);
        }
    }
}