using System.Windows.Media;
using Common;

namespace ChessBoard
{
    public interface ITool
    {
        Color Color { get; }

        string Type { get; }

        ITool GetCopy();
    }
}
