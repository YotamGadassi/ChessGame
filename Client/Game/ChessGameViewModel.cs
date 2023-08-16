using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Board;
using Client.Board;
using Client.Messages;
using Common;
using Common.Chess;
using FrontCommon;
using log4net;
using Tools;

namespace Client.Game;

public abstract class ChessGameViewModel: DependencyObject
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(ChessGameViewModel));

    private static readonly DependencyProperty NorthTeamStatusProperty = DependencyProperty.Register("NorthTeamStatus", typeof(TeamStatusViewModel), typeof(OnlineGameViewModel));
    private static readonly DependencyProperty SouthTeamStatusProperty = DependencyProperty.Register("SouthTeamStatus", typeof(TeamStatusViewModel), typeof(OnlineGameViewModel));
    private static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(object), typeof(GameViewModel));

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

    private          ChessBoardViewModel        m_boardViewModel;
    private          IAsyncChessGameManager     m_gameManager;
    private readonly IAsyncAvailableMovesHelper m_availableMovesHelper;

    protected ChessGameViewModel(IAsyncChessGameManager gameManager, IAsyncAvailableMovesHelper availableMovesHelper)
    {
        m_boardViewModel       = new ChessBoardViewModel();
        m_gameManager          = gameManager;
        m_availableMovesHelper = availableMovesHelper;
    }

    protected void setClickablePositions(BoardPosition[] positions, bool isClickable)
    {
        foreach (BoardPosition position in positions)
        {
            m_boardViewModel.SetSquareClickableState(position, isClickable);
        }
    }

    protected abstract void onSwitchTeamEvent(Color currentTeamTurn);

    protected virtual async void onSqualeClickHandler(object?         sender
                                                    , SquareViewModel squareVM)
    {
        s_log.DebugFormat("Click on square: {0}", squareVM);

        ITool?        tool          = squareVM.Tool;
        BoardPosition position      = squareVM.Position;
        Color         currTeamColor = await m_gameManager.CurrentColorTurn;

        bool isToolBelongsToTeam = null != tool && tool.Color == currTeamColor;
        if (isToolBelongsToTeam)
        {
            m_boardViewModel.ClearSelectedAndHintedBoardPositions();
            m_boardViewModel.SelectedBoardPosition = position;
            BoardPosition[] availablePositionsToMove = await m_availableMovesHelper.GetAvailablePositionToMove(position);
            m_boardViewModel.SetHintedBoardPosition(availablePositionsToMove);
            return;
        }

        if (false == m_boardViewModel.SelectedBoardPosition.IsEmpty())
        {
            BoardPosition start      = m_boardViewModel.SelectedBoardPosition;
            BoardPosition end        = position;
            MoveResult    moveResult = await m_gameManager.Move(start, end);
            handleMoveResult(moveResult);
        }
    }
    
    private void handleMoveResult(MoveResult moveResult)
    {
        s_log.Info($"Handles move result: {moveResult}");

        MoveResultEnum moveResultEnum = moveResult.Result;
        if (moveResultEnum.HasFlag(MoveResultEnum.NoChangeOccurred))
        {
            return;
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.ToolKilled))
        {
            m_boardViewModel.RemoveTool(moveResult.EndPosition, out _);
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.ToolMoved))
        {
            m_boardViewModel.RemoveTool(moveResult.InitialPosition, out _);
            m_boardViewModel.AddTool(moveResult.ToolAtInitial, moveResult.EndPosition);
            onSwitchTeamEvent(moveResult.ToolAtInitial.Color);
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.NeedPromotion))
        {
            onPromotionEvent(moveResult.EndPosition, moveResult.ToolAtInitial);
        }

        if (moveResultEnum.HasFlag(MoveResultEnum.CheckMate))
        {
            //TODO: Handle check mate
        }

    }

    private async void onPromotionEvent(BoardPosition position
                                      , ITool         toolToPromote)
    {
        s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");

        PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
        Message = promotionMessage;

        ITool chosenTool = await promotionMessage.ToolAwaiter;
        Message = null;

        PromotionResult promoteResult = await m_gameManager.Promote(position, chosenTool);
        handlePromotionResult(promoteResult);
    }

    private void handlePromotionResult(PromotionResult promotionResult)
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
                m_boardViewModel.AddTool(promotionResult.NewTool, promotionResult.PromotionPosition);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}