using FrontCommon.Facade;

namespace FrontCommon;

public class GamePanelManager : IGamePanelManager
{
    public event EventHandler<GamePanelChangedEventArgs>? GamePanelChanged;
    public BaseGamePanel                                  CurrentPanel { get; private set; }

    private Dictionary<string, BaseGamePanel> m_panels;

    public GamePanelManager()
    {
        m_panels = new Dictionary<string, BaseGamePanel>();
    }

    public void Add(string        panelName
                  , BaseGamePanel panel)
    {
        m_panels.Add(panelName, panel);
        panel.GameEnded += onGameEnd;
    }

    public void Remove(string panelName)
    {
        bool isPanelExist = m_panels.TryGetValue(panelName, out BaseGamePanel gamePanel);
        if (isPanelExist)
        {
            if (CurrentPanel == gamePanel)
            {
                ResetCurrentPanel();
            }
            
            gamePanel.GameEnded -= onGameEnd;
            gamePanel.Dispose();
            m_panels.Remove(panelName);
        }
    }

    public bool Show(BaseGamePanel gamePanel)
    {
        BaseGamePanel oldPanel = CurrentPanel;
        CurrentPanel = gamePanel;
        GamePanelChanged?.Invoke(this, new GamePanelChangedEventArgs(oldPanel.PanelName, CurrentPanel));
        BaseGameFacade.Instance.MainWindowViewModel.CurrentViewModel = CurrentPanel.GameControl;
        return true;
    }

    public void ResetCurrentPanel()
    {
        BaseGamePanel oldPanel = CurrentPanel;
        CurrentPanel = null;
        GamePanelChanged?.Invoke(this, new GamePanelChangedEventArgs(oldPanel.PanelName, CurrentPanel));
        BaseGameFacade.Instance.MainWindowViewModel.CurrentViewModel = null;
    }

    public bool TryGetPanel(string            panelName
                          , out BaseGamePanel gamePanel)
    {
        return m_panels.TryGetValue(panelName, out gamePanel);
    }

    public BaseGamePanel[] GetAllPanels()
    {
        return m_panels.Values.ToArray();
    }

    private void onGameEnd(BaseGamePanel gamePanel)
    {
        if(gamePanel == CurrentPanel)
            ResetCurrentPanel();
    }
}