using Common.MainWindow;
using FrontCommon.Facade;
using Host;

namespace FrontCommon;

public class GameFacade : BaseGameFacade, IGameFacade
{
    public IGamePanelManager       GamePanelManager    { get; private set; }
    public BaseMainWindowViewModel MainWindowViewModel { get; private set; }

    static GameFacade()
    {
        Instance = new GameFacade();
    }

    private GameFacade()
    {
    }

    public void SetGamePanelManager(IGamePanelManager gamePanelManager)
    {
        GamePanelManager = gamePanelManager;
    }

    public void SetMainWindoesViewModel(BaseMainWindowViewModel mainWindoesViewModel)
    {
        MainWindowViewModel = mainWindoesViewModel;
    }
}