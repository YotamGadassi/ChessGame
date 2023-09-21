using System.Windows;
using System.Windows.Controls;
using Client.Game;
using FrontCommon;

namespace OnlineChess.GamePanel;

public class OnlineChessGamePanel : BaseGamePanel
{
    public override DependencyObject       GameViewModel { get; }
    public override Control                GameControl   => m_gameControl;
    public          OnlineChessGameManager GameManager   { get; set; }

    private GameControl m_gameControl;

    public OnlineChessGamePanel(string panelName) : base(panelName)
    {
        m_gameControl = new GameControl();
    }

    public void SetGameManager(OnlineChessGameManager gameManager)
    {
        GameManager = gameManager;
        //TODO: Create View Model
    }

    public override void Init() { }

    public override void Reset()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}