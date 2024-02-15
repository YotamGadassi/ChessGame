﻿using Board;
using Common;
using log4net;

namespace OnlineChess.GamePanel;

public class OnlineChessGameManager : IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessGameManager));

    public IBoardEvents           BoardEvents  => GameBoard;
    public IBoardQuery            BoardQuery   => GameBoard;
    public OnlineChessTeamManager TeamsManager { get; }

    public OnlineGameBoard        GameBoard    { get; }

    public OnlineChessGameManager(OnlineGameBoard        gameBoard
                                , OnlineChessTeamManager teamManager)
    {
        GameBoard    = gameBoard;
        TeamsManager = teamManager;

        s_log.Info("Created");
    }

    public void Dispose()
    {
        GameBoard.Dispose();
        TeamsManager.Dispose();
    }
}