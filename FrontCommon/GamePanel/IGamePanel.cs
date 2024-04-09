using System.Windows;
using System.Windows.Controls;

namespace FrontCommon.GamePanel;

public interface IGamePanel : IDisposable
{
    event Action<IGamePanel> GameEnded;
    string                      PanelName     { get; }
    DependencyObject            GameViewModel { get; }
    Control                     GameControl   { get; }

    void Init();
    void Reset();
    void Dispose();
}