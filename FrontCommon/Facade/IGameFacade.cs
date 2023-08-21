using Common.MainWindow;

namespace FrontCommon;

public interface IGameFacade
{
    static IGameFacade Instance { get; }

    IGamePanelManager GamePanelManager { get; }

    BaseMainWindowViewModel MainWindowViewModel { get; }
}