using Common.MainWindow;
using FrontCommon.Facade;
using Host;

namespace FrontCommon;

public class GameFacade : BaseGameFacade, IGameFacade
{
    public  IGamePanelManager       GamePanelManager    { get; }
    public BaseMainWindowViewModel MainWindowViewModel { get; }

    static GameFacade()
    {
        Instance = new GameFacade();
    }

    private GameFacade()
    {
        GamePanelManager    = new GamePanelManager();
        MainWindowViewModel = new MainWindowViewModel();
    }
}