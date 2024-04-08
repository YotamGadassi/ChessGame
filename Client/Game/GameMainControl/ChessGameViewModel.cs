using System;
using System.Windows;
using ChessGame;
using Client.Board;
using Common;
using log4net;

namespace Client.Game.GameMainControl;

public abstract class ChessGameViewModel : DependencyObject, IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(ChessGameViewModel));

    private static readonly DependencyProperty NorthTeamStatusProperty =
        DependencyProperty.Register("NorthTeamStatus"
                                  , typeof(TeamStatusViewModel)
                                  , typeof(ChessGameViewModel));

    private static readonly DependencyProperty SouthTeamStatusProperty =
        DependencyProperty.Register("SouthTeamStatus"
                                  , typeof(TeamStatusViewModel)
                                  , typeof(ChessGameViewModel));

    private static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message"
                                  , typeof(object)
                                  , typeof(ChessGameViewModel));

    private static readonly DependencyProperty BoardProperty =
        DependencyProperty.Register("Board"
                                  , typeof(BoardViewModel)
                                  , typeof(ChessGameViewModel));

    
    public event EventHandler GameEnd;

    public TeamStatusViewModel NorthTeamStatus
    {
        get => (TeamStatusViewModel)GetValue(NorthTeamStatusProperty);
        set => SetValue(NorthTeamStatusProperty, value);
    }

    public TeamStatusViewModel SouthTeamStatus
    {
        get => (TeamStatusViewModel)GetValue(SouthTeamStatusProperty);
        set => SetValue(SouthTeamStatusProperty, value);
    }
    
    public BoardViewModel Board
    {
        get => (BoardViewModel)GetValue(BoardProperty);
        private set => SetValue(BoardProperty, value);
    }

    public object? Message
    {
        get => GetValue(MessageProperty);
        set
        {
            Board.IsEnabled = value == null;
            SetValue(MessageProperty, value);
        }
    }

    private readonly IGameEvents m_gameEvents;

    protected ChessGameViewModel(IChessGameManager gameManager)
    {
        setBoard(new BoardViewModel(gameManager.BoardEvents));
        initTeams(gameManager.TeamsManager);
        m_gameEvents = gameManager.GameEvents;
        registerToEvents();
        s_log.Info("Created");
    }

    public virtual void Dispose()
    {
        Board.OnSquareClick -= onSquareClick;
        Board.Dispose();
        SouthTeamStatus?.Dispose();
        NorthTeamStatus?.Dispose();
        unRegisterFromEvents();
    }

    protected abstract void onSquareClick(object?         sender
                                        , SquareViewModel squareVM);

    protected abstract void onPromotion(PromotionRequest promotionRequest);

    protected abstract void onCheckMate(CheckMateData checkMateData);

    protected void gameEnd(object sender,EventArgs e) => GameEnd?.Invoke(sender, e);

    private void setBoard(BoardViewModel newBoard)
    {
        if (null != Board)
        {
            Board.OnSquareClick -= onSquareClick;
            Board.Dispose();
        }
        newBoard.OnSquareClick += onSquareClick;
        Board                  =  newBoard;
    }

    private void setNorthTeam(TeamStatusViewModel teamStatusViewModel)
    {
        NorthTeamStatus?.Dispose();
        NorthTeamStatus = teamStatusViewModel;
    }

    private void setSouthTeam(TeamStatusViewModel teamStatusViewModel)
    {
        SouthTeamStatus?.Dispose();
        SouthTeamStatus = teamStatusViewModel;
    }

    private void initTeams(IChessTeamManager teamsManager)
    {
        Team       team1      = teamsManager.Teams[0];
        ITeamTimer team1Timer = teamsManager.GetTeamTimer(team1.Id);
        Team       team2      = teamsManager.Teams[1];
        ITeamTimer team2Timer = teamsManager.GetTeamTimer(team2.Id);

        if (team1.MoveDirection == GameDirection.North)
        {
            setSouthTeam(new TeamStatusViewModel(team1, team1Timer));
            setNorthTeam(new TeamStatusViewModel(team2, team2Timer));
        }
        else
        {
            setSouthTeam(new TeamStatusViewModel(team2, team2Timer));
            setNorthTeam(new TeamStatusViewModel(team1, team1Timer));
        }
    }

    private void registerToEvents()
    {
        m_gameEvents.AskPromotionEvent += onPromotion;
        m_gameEvents.CheckMateEvent    += onCheckMate;
    }

    private void unRegisterFromEvents()
    {
        m_gameEvents.AskPromotionEvent -= onPromotion;
        m_gameEvents.CheckMateEvent    -= onCheckMate;
    }
}