using System.Windows;
using System.Windows.Controls;
using Client.Game;
using FrontCommon;
using OnlineChess.Client;

namespace OnlineChess.GamePanel;

public class OnlineChessGamePanel : BaseGamePanel
{
    public override DependencyObject       GameViewModel => m_gameViewModel;
    public override Control                GameControl   => m_gameControl;
    public          OnlineChessGameManager GameManager   { get; set; }

    private GameControl          m_gameControl;
    private OnlineChessViewModel m_gameViewModel;

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
    }

    public override void Init()
    {
        //TODO: implement
    }

    public override void Reset()
    {
        //TODO: implement
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        //TODO: implement
        throw new NotImplementedException();
    }
}