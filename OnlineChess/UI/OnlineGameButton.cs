using System.Windows.Threading;
using Common;
using FrontCommon;
using log4net;
using OnlineChess.Common;
using OnlineChess.ConnectionManager;
using OnlineChess.Game;
using OnlineChess.TeamManager;

namespace OnlineChess.UI;

public class OnlineGameButton : BaseGameButton, IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineGameButton));
    public override string PanelGameName => "OnlineChessGame";
    public override string CommandName => "Online Chess Game";

    private readonly OnlineGameRequestManager m_gameRequestManager;
    private readonly IChessServerAgent m_serverAgent;

    public OnlineGameButton(Dispatcher dispatcer
                          , IGamePanelManager panelManager
                          , IChessConnectionManager connectionManager) : base(dispatcer, panelManager)
    {
        m_gameRequestManager = new OnlineGameRequestManager(connectionManager);
        m_serverAgent = connectionManager.ServerAgent;
        registerToEvents();
    }

    public void Dispose()
    {
        unRegisterFromEvent();
    }

    protected override async void playCommandExecute(object parameter)
    {
        string userName = "A"; //TODO: use parameter as object name
        GameRequest gameRequest = new GameRequest(userName);
        //TODO: show message
        try
        {
            await m_gameRequestManager.SubmitGameRequest(gameRequest);
        }
        catch (Exception e)
        {
            s_log.Error(e.Message);
            //TODO: Handle Error
        }
        //TODO: Show Waiting For Connection Panel\Message
    }

    private void registerToEvents()
    {
        m_gameRequestManager.StartGameEvent += onGameStart;
    }

    private void unRegisterFromEvent()
    {
        m_gameRequestManager.StartGameEvent -= onGameStart;
    }

    private void onGameStart(GameConfig gameConfiguration)
    {
        OnlineChessGameManager onlineGameManager = createOnlineGameManager(gameConfiguration);

        BaseGamePanel panel = getPanel();
        panel.Init();
        if (panel is OnlineChessGamePanel onlineGamePanel)
        {
            onlineGamePanel.SetGameManager(onlineGameManager);
        }

        //TODO: Remove Waiting For Connection Panel\Message
        m_panelManager.Show(panel);
    }

    private OnlineChessGameManager createOnlineGameManager(GameConfig gameConfiguration)
    {
        TeamConfig[]    teamConfigArr   = gameConfiguration.TeamConfigs;
        OnlineChessTeam localTeam = teamConfigArr.FirstOrDefault((teamConfig) => teamConfig.IsLocal)
                                                       .ToOnlineChessTeam(m_serverAgent);
        OnlineChessTeam remoteTeam = teamConfigArr.FirstOrDefault((teamConfig) => false == teamConfig.IsLocal)
                                                  .ToOnlineChessTeam(m_serverAgent);
        if (null == localTeam || null == remoteTeam)
        {
            s_log.ErrorFormat("One of the teams could not be created");
            return null;
        }

        TeamId firstTeamTurnId = teamConfigArr.First((teamConfig) => teamConfig.IsFirst).Id;

        OnlineGameBoard        gameBoard              = new(m_serverAgent, null);
        OnlineChessTeamManager teamManager            = new(localTeam, remoteTeam, firstTeamTurnId, m_serverAgent);
        OnlineGameState        gameState              = new(m_serverAgent);
        OnlineChessGameManager onlineChessGameManager = new(gameBoard, teamManager, gameState);
        return onlineChessGameManager;
    }
}