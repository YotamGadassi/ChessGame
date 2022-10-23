using System.Windows.Media;

namespace Common;

public interface ITool
{
    Color Color { get; }

    string Type { get; }

    ITool GetCopy();
}