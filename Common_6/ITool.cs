using System.Windows.Media;

namespace Common_6
{
    public interface ITool
    {
        Color Color { get; }

        string Type { get; }

        ITool GetCopy();
    }
}
