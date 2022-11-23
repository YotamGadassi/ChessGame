using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Client.Helpers;
using Client.Messages;
using Common;
using Common.ChessBoardEventArgs;

namespace Client.Game;

public class OfflineGameViewModel : BaseGameViewModel
{
    protected IGameManager        m_gameManager;
    public    TeamStatusViewModel NorthTeamStatus { get; }
    public    TeamStatusViewModel SouthTeamStatus { get; }

    protected AvailableMovesHelper m_availableMovesHelper;

    public OfflineGameViewModel(IGameManager gameManager, Team northTeam, Team southTeam)
    {
        m_gameManager          = gameManager;
        m_availableMovesHelper = new AvailableMovesHelper(gameManager);
        NorthTeamStatus        = new TeamStatusViewModel(northTeam);
        SouthTeamStatus        = new TeamStatusViewModel(southTeam);
        registerEvents();
    }

    private void registerEvents()
    {
        m_gameManager.ToolKilledEvent   += moveHandler;
        m_gameManager.ToolMovedEvent    += moveHandler;
        m_gameManager.PromotionEvent    += onPromotionAsyncEvent;
        m_gameManager.ToolPromotedEvent += onToolPromotedEvent;
    }

    private void onToolPromotedEvent(object?               sender
                                   , ToolPromotedEventArgs e)
    {
        Board.RemoveTool(e.ToolPosition, out _);
        Board.AddTool(e.ToolAfterPromotion, e.ToolPosition);
    }

    private void moveHandler(object sender, ToolMovedEventArgs e)
    {
        Action<BoardPosition, BoardPosition, ITool> act = MoveTool;
        m_dispatcher.BeginInvoke(act, e.InitialPosition, e.EndPosition, e.MovedTool);
    }

    private async Task<ITool> onPromotionAsyncEvent(object             sender
                                                  , PromotionEventArgs e)
    {
        PromotionMessageViewModel promotionViewModel = new(e.ToolToPromote.Color, e.ToolPosition);
        Message = promotionViewModel;
        ITool tool =  await promotionViewModel.ToolAwaiter;
        Message = null;
        return tool;
    }

    private void handleCheckMate()
    {
        Message = new UserMessageViewModel("Checkmate", "OK", () => Message = null);
    }

    protected override void SquareClickHandler(BoardPosition position
                                             , ITool?        tool)
    {
        Color         currTeamColor          = m_gameManager.CurrentColorTurn;
        bool          isPositionToolSameTeam = null != tool && tool.Color.Equals(currTeamColor);
        BoardPosition selectedBoardPosition  = Board.SelectedBoardPosition;
        Board.ClearSelectedAndHintedBoardPositions();

        if (isPositionToolSameTeam)
        {
            Board.SelectedBoardPosition = position;
            BoardPosition[] positionToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
            Board.SetHintedBoardPosition(positionToMove);
            return;
        }

        if (false == Board.SelectedBoardPosition.IsEmpty())
        {
            MoveResult     moveResult     = m_gameManager.Move(selectedBoardPosition, position);
            MoveResultEnum moveResultEnum = moveResult.Result;
            if (moveResultEnum.HasFlag(MoveResultEnum.CheckMate))
            {
                handleCheckMate();
                return;
            }
        }
        // Board.ClearSelectedAndHintedBoardPositions();
    }

    protected override bool SquareClickHandlerCanExecute(BoardPosition poistion
                                                       , ITool?        tool)
    {
        return m_gameManager.IsGameRunning;
    }
}