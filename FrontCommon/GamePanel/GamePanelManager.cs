using System.Windows.Threading;
using FrontCommon.Facade;
using FrontCommon.GamePanel;

namespace FrontCommon;

public class GamePanelManager : IGamePanelManager
{
    public event EventHandler<GamePanelChangedEventArgs>? GamePanelChanged;
    public IGamePanel                                  CurrentPanel { get; private set; }

    private readonly Dictionary<string, IGamePanel> m_panels;
    private readonly Dispatcher                     m_dispatcher;

    public GamePanelManager()
    {
        m_dispatcher = Dispatcher.CurrentDispatcher;
        m_panels     = new Dictionary<string, IGamePanel>();
    }

    public void Add(string     panelName
                  , IGamePanel panel)
    {
        m_panels.Add(panelName, panel);
        panel.GameEnded += onGameEnd;
    }

    public void Remove(string panelName)
    {
        bool isPanelExist = m_panels.TryGetValue(panelName, out IGamePanel gamePanel);
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

    public bool Show(IGamePanel gamePanel)
    {
        IGamePanel oldPanel = CurrentPanel;
        CurrentPanel = gamePanel;
        GamePanelChanged?.Invoke(this
                               , new GamePanelChangedEventArgs(oldPanel.PanelName
                                                             , CurrentPanel));
        m_dispatcher.Invoke(() =>
                            {
                                BaseGameFacade.Instance.MainWindowViewModel.CurrentViewModel = CurrentPanel.GameControl;
                            });
        return true;
    }

    public void ResetCurrentPanel()
    {
        IGamePanel oldPanel = CurrentPanel;
        CurrentPanel = null;
        GamePanelChanged?.Invoke(this, new GamePanelChangedEventArgs(oldPanel.PanelName, CurrentPanel));
        m_dispatcher.Invoke(() =>
                            {
                                oldPanel.Reset();
                                BaseGameFacade.Instance.MainWindowViewModel.CurrentViewModel = null;
                            });
    }

    public bool TryGetPanel(string         panelName
                          , out IGamePanel gamePanel)
    {
        return m_panels.TryGetValue(panelName, out gamePanel);
    }

    public IGamePanel[] GetAllPanels()
    {
        return m_panels.Values.ToArray();
    }

    private void onGameEnd(IGamePanel gamePanel)
    {
        if (gamePanel == CurrentPanel)
            ResetCurrentPanel();
    }
}