using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
    public override Control          GameControl   => m_gameControl;

    private          GameControl            m_gameControl;
    private          OnlineChessViewModel?  m_gameViewModel;
    private readonly Dispatcher             m_dispatcher;
    private          OnlineChessGameManager m_gameManager;

    public OnlineChessGamePanel(string     panelName
                              , Dispatcher dispatcher) : base(panelName)
    {
        m_dispatcher  = dispatcher;
        m_gameControl = new GameControl();
    }

    public void SetGameManager(OnlineChessGameManager gameManager)
    {
        m_dispatcher.Invoke(() =>
                            {
                                m_gameManager           =  gameManager;
                                m_gameViewModel         =  new OnlineChessViewModel(gameManager, m_dispatcher);
                                m_gameViewModel.GameEnd += onGameEnd;
                                GameControl.DataContext =  null;
                                GameControl.DataContext =  m_gameViewModel;
                            });

        s_log.Info("Game Manager Set");
    }

    public override void Init()
    {
        s_log.Info("Initialized");
    }

    public override void Reset()
    {
        disposeResources();
        m_gameViewModel = null;
        m_gameControl   = new GameControl();
        s_log.Info("Reset");
    }

    public override void Dispose()
    {
        disposeResources();
    }

    private async void onGameEnd(object?   sender
                               , EventArgs e)
    {
        s_log.Info("Game Ended");
        gameEnd();
    }

    private void disposeResources()
    {
        m_gameManager.Dispose();
        m_gameViewModel.GameEnd -= onGameEnd;
        m_gameViewModel.Dispose();
    }
}