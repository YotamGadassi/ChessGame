using System.Windows;
using System.Windows.Controls;
using Client.Game;
using FrontCommon;
using log4net;
using OnlineChess.Client;

namespace OnlineChess.GamePanel;

public class OnlineChessGamePanel : BaseGamePanel
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGamePanel));

    public override DependencyObject       GameViewModel => m_gameViewModel;
    public override Control                GameControl   => m_gameControl;
    public          OnlineChessGameManager GameManager   { get; set; }

    private readonly GameControl          m_gameControl;
    private          OnlineChessViewModel m_gameViewModel;

    public OnlineChessGamePanel(string panelName) : base(panelName)
    {
        m_gameControl = new GameControl();
    }

    public void SetGameManager(OnlineChessGameManager gameManager)
    {
        GameManager             = gameManager;
        m_gameViewModel         = new OnlineChessViewModel(gameManager);
        GameControl.DataContext = null;
        GameControl.DataContext = m_gameViewModel;
        
        s_log.Info("Game Manager Set");
    }

    public override void Init()
    {
        //TODO: implement
        s_log.Info("Initialized");
    }

    public override void Reset()
    {
        //TODO: implement
        throw new NotImplementedException();
        s_log.Info("Reset");

    }

    public override void Dispose()
    {
        //TODO: implement
        throw new NotImplementedException();
    }
}