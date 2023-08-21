namespace FrontCommon;

public interface IGamePanelManager
{
    public BaseGamePanel CurrentPanel { get; }

    public event EventHandler<GamePanelChangedEventArgs> GamePanelChanged; 

    public void Add(string panelName, BaseGamePanel panel);

    public void Remove(string panelName);

    public bool Show(string panelName);

    public bool TryGetPanel(string panelName, out BaseGamePanel gamePanel);

    public BaseGamePanel[] GetAllPanels();

}