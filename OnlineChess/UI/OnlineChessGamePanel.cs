﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Client.Game;
using Common;
using FrontCommon.GamePanel;
using log4net;
using OnlineChess.Common;
using OnlineChess.ConnectionManager;
using OnlineChess.Game;
using OnlineChess.TeamManager;

namespace OnlineChess.UI;

public class OnlineChessGamePanel : IGamePanel
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGamePanel));

    public event Action<IGamePanel>? GameEnded;
    public          string           PanelName     { get; }
    public DependencyObject GameViewModel => m_gameViewModel;
    public Control          GameControl   => m_gameControl;

    private          GameControl              m_gameControl;
    private          OnlineChessViewModel?    m_gameViewModel;
    private readonly Dispatcher               m_dispatcher;
    private          OnlineChessGameManager   m_gameManager;
    private readonly OnlineGameRequestManager m_gameRequestManager;
    private readonly IChessConnectionManager  m_connectionManager;
    private readonly IChessServerAgent        m_serverAgent;

    public OnlineChessGamePanel(string                   panelName
                              , Dispatcher               dispatcher
                              , OnlineGameRequestManager gameRequestManager
                              , IChessConnectionManager  connectionManager)
    {
        PanelName            = panelName;
        m_dispatcher         = dispatcher;
        m_gameControl        = new GameControl();
        m_connectionManager  = connectionManager;
        m_serverAgent        = m_connectionManager.ServerAgent;
        m_gameRequestManager = gameRequestManager;
    }

    public void StartGame(OnlineChessGameManager gameManager, TeamId localTeamId)
    {
        m_dispatcher.Invoke(() =>
                            {
                                m_gameManager           =  gameManager;
                                m_gameViewModel         =  new OnlineChessViewModel(gameManager, localTeamId, m_dispatcher);
                                m_gameViewModel.GameEnd += onGameEnd;
                                GameControl.DataContext =  null;
                                GameControl.DataContext =  m_gameViewModel;
                            });

        s_log.Info("Game Manager Set");
    }

    public void Init()
    {
        s_log.Info("Initialized");
        registerToEvents();
    }

    public void Reset()
    {
        disposeResources();
        m_gameViewModel = null;
        m_gameControl   = new GameControl();
        s_log.Info("Reset");
    }

    public void Dispose()
    {
        disposeResources();
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
        StartGame(onlineGameManager, onlineGameManager.OnlineTeamsManager.LocalMachineTeamId);
       
        m_serverAgent.AckGameReceive();
    }

    private OnlineChessGameManager createOnlineGameManager(GameConfig gameConfiguration)
    {
        TeamConfig[] teamConfigArr = gameConfiguration.TeamConfigs;
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
        OnlineGameState?       gameState              = new(m_serverAgent);
        OnlineGameEvents       gameEvents             = new(m_serverAgent);
        OnlineChessGameManager onlineChessGameManager = new(gameBoard, teamManager, gameEvents, gameState);
        return onlineChessGameManager;
    }

    private async void onGameEnd(object?   sender
                               , EventArgs e)
    {
        s_log.Info("Game Ended");
        GameEnded?.Invoke(this);
    }

    private void disposeResources()
    {
        m_gameManager.Dispose();
        m_gameViewModel.GameEnd -= onGameEnd;
        m_gameViewModel.Dispose();
        unRegisterFromEvent();
    }
}