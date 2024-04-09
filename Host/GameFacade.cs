using Common.MainWindow;
using FrontCommon;
using FrontCommon.Facade;
using FrontCommon.GamePanel;

namespace Host;

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

    public void SetMainWindowViewModel(BaseMainWindowViewModel mainWindowViewModel)
    {
        MainWindowViewModel = mainWindowViewModel;
    }
}