using System.Windows;
using System.Windows.Controls;
using Client.Game;
using Client.GameManagers;
using Common.Chess;
using FrontCommon;

namespace Frameworks.OnlineChessGame;

public class OnlineChessGamePanel : BaseGamePanel
{
    public override DependencyObject GameViewModel { get; }
    public override Control          GameControl   { get; }

    private OnlineChessGameViewModel m_chessGameViewModel;
    private OnlineChessGameManager   m_gameManager;
    private AppConnectionManager m_connectionManager;

    public OnlineChessGamePanel(string panelName) : base(panelName)
    {
        GameControl = new GameControl();
    }

    public override void Init()
    {
        m_connectionManager  = new AppConnectionManager("User");
        m_gameManager        = new OnlineChessGameManager(new ChessBoard(), m_connectionManager);
        m_chessGameViewModel = new OnlineChessGameViewModel(m_gameManager, m_connectionManager.ServerAgent);
        GameControl.DataContext = m_chessGameViewModel;

        m_connectionManager.Connect();
        m_connectionManager.ServerAgent.RequestGame();
    }

    public override void Reset()
    {
        m_connectionManager.Disconnect();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}