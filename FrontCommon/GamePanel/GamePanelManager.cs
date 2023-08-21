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
    }

    public void Remove(string panelName)
    {
        m_panels.Remove(panelName);
    }

    public bool Show(string panelName)
    {
        bool isPanelExist = TryGetPanel(panelName, out BaseGamePanel gamePanel);
        if (isPanelExist)
        {
            BaseGamePanel oldPanel = CurrentPanel;
            CurrentPanel = gamePanel;
            GamePanelChanged?.Invoke(this, new GamePanelChangedEventArgs(oldPanel, gamePanel));
            return true;
        }
        return false;
    }

    public bool TryGetPanel(string panelName, out BaseGamePanel gamePanel)
    {
        return m_panels.TryGetValue(panelName, out gamePanel);
    }

    public BaseGamePanel[] GetAllPanels()
    {
        return m_panels.Values.ToArray();
    }
}