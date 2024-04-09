namespace FrontCommon.GamePanel;

public interface IGamePanelManager
{
    public IGamePanel CurrentPanel { get; }

    public event EventHandler<GamePanelChangedEventArgs> GamePanelChanged; 

    public void Add(string panelName, IGamePanel panel);

    public void Remove(string panelName);

    public bool Show(IGamePanel gamePanel);

    public void ResetCurrentPanel();

    public bool TryGetPanel(string panelName, out IGamePanel gamePanel);

    public IGamePanel[] GetAllPanels();

}