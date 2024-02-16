using System;
using System.Windows;
using Board;
using Client.Board;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Tools;

namespace Client.Game;

public abstract class ChessGameViewModel : DependencyObject, IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(ChessGameViewModel));

    private static readonly DependencyProperty NorthTeamStatusProperty =
        DependencyProperty.Register("NorthTeamStatus", typeof(TeamStatusViewModel), typeof(ChessGameViewModel));

    private static readonly DependencyProperty SouthTeamStatusProperty =
        DependencyProperty.Register("SouthTeamStatus", typeof(TeamStatusViewModel), typeof(ChessGameViewModel));

    private static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(object), typeof(ChessGameViewModel));

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

    public object? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public BoardViewModel BoardViewModel { get; }

    protected ChessGameViewModel(IBoardEvents boardEvents, IChessTeamManager teamsManager)
    {
        BoardViewModel               =  new BoardViewModel(boardEvents);
        BoardViewModel.OnSquareClick += onSquareClick;
        
        initTeams(teamsManager);
        s_log.Info("Created");
    }

    public virtual void Dispose()
    {
        BoardViewModel.OnSquareClick -= onSquareClick;
        BoardViewModel.Dispose();
        SouthTeamStatus?.Dispose();
        NorthTeamStatus?.Dispose();
    }

    protected abstract void onSquareClick(object?         sender
                                               , SquareViewModel squareVM);

    protected abstract void onPromotion(BoardPosition position
                                           , ITool         toolToPromote);

    protected abstract void onCheckMate(BoardPosition position
                                           , ITool         tool);

    protected void handleMoveResult(MoveResult moveResult)
    {
        s_log.Info($"Handles move result: {moveResult}");

        MoveResultEnum moveResultEnum = moveResult.Result;
        if (moveResultEnum.HasFlag(MoveResultEnum.NoChangeOccurred))
        {
            return;
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.NeedPromotion))
        {
            onPromotion(moveResult.EndPosition, moveResult.ToolAtInitial);
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.CheckMate))
        {
            onCheckMate(moveResult.EndPosition, moveResult.ToolAtInitial);
        }
    }

    protected void handlePromotionResult(PromotionResult promotionResult)
    {
        s_log.Info($"Handles promotion result: {promotionResult}");

        switch (promotionResult.Result)
        {
            case PromotionResultEnum.PositionIsEmpty:
            case PromotionResultEnum.ToolIsNotValidForPromotion:
            {
                break;
            }
            case PromotionResultEnum.PromotionSucceeded:
            {
                BoardViewModel.AddTool(promotionResult.NewTool, promotionResult.PromotionPosition);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void initTeams(IChessTeamManager teamsManager)
    {
        Team       team1      = teamsManager.Teams[0];
        ITeamTimer team1Timer = teamsManager.GetTeamTimer(team1);
        Team       team2      = teamsManager.Teams[1];
        ITeamTimer team2Timer = teamsManager.GetTeamTimer(team2);

        if (team1.MoveDirection == GameDirection.North)
        {
            SouthTeamStatus = new TeamStatusViewModel(team1, team1Timer);
            NorthTeamStatus = new TeamStatusViewModel(team2, team2Timer);
        }
        else
        {
            SouthTeamStatus = new TeamStatusViewModel(team2, team2Timer);
            NorthTeamStatus = new TeamStatusViewModel(team1, team1Timer);
        }
    }
}