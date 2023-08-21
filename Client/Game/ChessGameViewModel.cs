using System;
using System.Windows;
using System.Windows.Media;
using Board;
using Client.Board;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Tools;

namespace Client.Game;

public abstract class ChessGameViewModel : BaseGameViewModel
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

    protected ChessGameViewModel(IChessGameManager gameManager) : base(gameManager)
    {
        BoardViewModel               =  new BoardViewModel(gameManager.BoardEvents);
        BoardViewModel.OnSquareClick += onSqualeClickHandler;
        initTeams(gameManager.Teams[0], gameManager.Teams[1]);
        s_log.Info("Created");
    }

    protected abstract void onSqualeClickHandler(object?         sender
                                               , SquareViewModel squareVM);

    protected abstract void onPromotionEvent(BoardPosition position
                                           , ITool         toolToPromote);

    protected abstract void onCheckMateEvent(BoardPosition position
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
            onPromotionEvent(moveResult.EndPosition, moveResult.ToolAtInitial);
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.CheckMate))
        {
            onCheckMateEvent(moveResult.EndPosition, moveResult.ToolAtInitial);
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

    private void initTeams(TeamWithTimer team1
                         , TeamWithTimer team2)
    {
        if (team1.MoveDirection == GameDirection.North)
        {
            SouthTeamStatus = new TeamStatusViewModel(team1);
            NorthTeamStatus = new TeamStatusViewModel(team2);
        }
        else
        {
            SouthTeamStatus = new TeamStatusViewModel(team2);
            NorthTeamStatus = new TeamStatusViewModel(team1);
        }
    }
}