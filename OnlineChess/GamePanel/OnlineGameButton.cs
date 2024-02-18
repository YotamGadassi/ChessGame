﻿using System.Windows.Threading;
using Common;
using FrontCommon;
using log4net;
using OnlineChess.Common;
using OnlineChess.ConnectionManager;
using OnlineChess.Game;

namespace OnlineChess.GamePanel;

public class OnlineGameButton : BaseGameButton, IDisposable
{
    private static readonly ILog   s_log = LogManager.GetLogger(typeof(OnlineGameButton));
    public override         string PanelGameName => "OnlineChessGame";
    public override         string CommandName   => "Online Chess Game";

    private readonly OnlineGameRequestManager m_gameRequestManager;
    private readonly IChessServerAgent        m_serverAgent;

    public OnlineGameButton(Dispatcher               dispatcer
                          , IGamePanelManager        panelManager
                          , IChessConnectionManager connectionManager) : base(dispatcer, panelManager)
    {
        m_gameRequestManager = new OnlineGameRequestManager(connectionManager);
        m_serverAgent        = connectionManager.ServerAgent;
        registerToEvents();
    }

    public void Dispose()
    {
        unRegisterFromEvent();
    }

    protected override async void playCommandExecute(object parameter)
    {
        string      userName    = "A"; //TODO: use parameter as object name
        GameRequest gameRequest = new GameRequest(userName);
        //TODO: show message
        try
        {
            await m_gameRequestManager.RequestGame(gameRequest);
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

    private void onGameStart(OnlineChessGameConfiguration gameConfiguration)
    {
        OnlineChessGameManager onlineGameManager = createOnlineGameManager(gameConfiguration);

        BaseGamePanel panel = getPanel();
        if (panel is OnlineChessGamePanel onlineGamePanel)
        {
            onlineGamePanel.SetGameManager(onlineGameManager);
        }

        panel.Init();
        //TODO: Remove Waiting For Connection Panel\Message
        m_panelManager.Show(panel);
    }

    private OnlineChessGameManager createOnlineGameManager(OnlineChessGameConfiguration gameConfiguration)
    {
        OnlineChessTeam localTeam     = gameConfiguration.LocalTeam;
        OnlineChessTeam remoteTeam    = gameConfiguration.RemoteTeam;
        Team            firstTeamTurn = gameConfiguration.FirstTeamTurn;

        OnlineGameBoard        gameBoard              = new(m_serverAgent, gameConfiguration.BoardState);
        OnlineChessTeamManager teamManager            = new(localTeam, remoteTeam, firstTeamTurn, m_serverAgent);
        OnlineGameState        gameState              = new OnlineGameState(m_serverAgent);
        OnlineChessGameManager onlineChessGameManager = new(gameBoard, teamManager, gameState);
        return onlineChessGameManager;
    }
}