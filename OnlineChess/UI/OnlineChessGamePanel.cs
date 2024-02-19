using System.Windows;
using System.Windows.Controls;
using Client.Game;
using Common;
using FrontCommon;
using log4net;
using OnlineChess.Game;

namespace OnlineChess.UI;

public class OnlineChessGamePanel : BaseGamePanel
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGamePanel));

    public override DependencyObject GameViewModel => m_gameViewModel;
    public override Control GameControl => m_gameControl;
    public OnlineChessGameManager? GameManager { get; set; }
    public IGameState GameState { get; private set; }
    private GameControl m_gameControl;
    private OnlineChessViewModel? m_gameViewModel;

    public OnlineChessGamePanel(string panelName) : base(panelName)
    {
        m_gameControl = new GameControl();
    }

    public void SetGameManager(OnlineChessGameManager gameManager)
    {
        GameManager = gameManager;
        GameState = gameManager.GameState;
        GameState.StateChanged += onStateChanged;
        m_gameViewModel = new OnlineChessViewModel(gameManager);
        GameControl.DataContext = null;
        GameControl.DataContext = m_gameViewModel;

        s_log.Info("Game Manager Set");
    }

    public override void Init()
    {
        s_log.Info("Initialized");
    }

    public override void Reset()
    {
        disposeResources();
        GameManager = null;
        m_gameViewModel = null;
        GameState = null;
        m_gameControl = new GameControl();
        s_log.Info("Reset");
    }

    public override void Dispose()
    {
        disposeResources();
    }

    private void onStateChanged(object? sender
                              , GameStateEnum newState)
    {
        s_log.InfoFormat("State Changed: {0}", newState);
        if (newState == GameStateEnum.Ended)
        {
            TimeSpan delayTime = m_gameViewModel.Message == null ? TimeSpan.Zero : TimeSpan.FromSeconds(10);
            Task.Delay(delayTime)
                .ContinueWith((_) =>
                              {
                                  onGameEnd();
                                  Reset();
                              });
        }
    }

    private void disposeResources()
    {
        GameManager.Dispose();
        m_gameViewModel.Dispose();
        GameState.StateChanged -= onStateChanged;
    }
}