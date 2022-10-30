using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ChessGame;
using Client.Board;
using Client.Helpers;
using Client.Messages;
using Common;
using Common.ChessBoardEventArgs;

namespace Client.Game
{
    public interface IGameViewModel
    {
        public BoardViewModel Board { get; }
    }

    public abstract class BaseGameViewModel : DependencyObject, IGameViewModel
    {
        protected   Dispatcher           m_dispatcher;

        public BoardViewModel  Board { get; }
        public object Message
        {
            get => GetValue(messageProperty);
            set => SetValue(messageProperty, value);
        }

        private static readonly DependencyProperty messageProperty = DependencyProperty.Register("Message", typeof(object), typeof(BaseGameViewModel));

        protected BaseGameViewModel()
        {
            m_dispatcher = Dispatcher.CurrentDispatcher;
            Board        = new BoardViewModel(SquareClickHandler, SquareClickHandlerCanExecute);
        }
        
        protected void MoveTool(BoardPosition start, BoardPosition end, ITool tool)
        {
            Board.ClearSelectedAndHintedBoardPositions();
            Board.RemoveTool(start, out ITool toolAtStart);
            Board.AddTool(tool, end);
        }

        protected abstract void SquareClickHandler(BoardPosition position
                                                , ITool?        tool);

        protected abstract bool SquareClickHandlerCanExecute(BoardPosition position
                                                           , ITool?        tool);
    }

    public class OfflineGameViewModel : BaseGameViewModel
    {
        protected IGameManager         m_gameManager;
        public    TeamStatusViewModel NorthTeamStatus { get; }
        public    TeamStatusViewModel SouthTeamStatus { get; }

        protected AvailableMovesHelper m_availableMovesHelper;

        public OfflineGameViewModel(IGameManager gameManager, Team northTeam, Team southTeam)
        {
            m_gameManager                =  gameManager;
            m_availableMovesHelper       =  new AvailableMovesHelper(gameManager);
            NorthTeamStatus              =  new TeamStatusViewModel(northTeam);
            SouthTeamStatus              =  new TeamStatusViewModel(southTeam);
            registerEvents();
        }

        private void registerEvents()
        {
            m_gameManager.ToolKilledEvent += moveHandler;
            m_gameManager.ToolMovedEvent  += moveHandler;
            m_gameManager.CheckmateEvent  += onCheckmateEventHandler;
            m_gameManager.PromotionEvent  += onPromotionAsyncEvent;
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

        private void onCheckmateEventHandler(object?            sender
                                           , CheckmateEventArgs e)
        {
            Message = new UserMessageViewModel("Checkmate", "OK", () => Message = null);
        }

        protected override void SquareClickHandler(BoardPosition position
                                                , ITool?        tool)
        {
            Color currTeamColor          = m_gameManager.CurrentColorTurn;
            bool  isPositionToolSameTeam = null != tool && tool.Color.Equals(currTeamColor);

            if (isPositionToolSameTeam)
            {
                Board.ClearSelectedAndHintedBoardPositions();
                Board.SelectedBoardPosition = position;
                BoardPosition[] positionToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
                Board.SetHintedBoardPosition(positionToMove);
                return;
            }

            if (false == Board.SelectedBoardPosition.IsEmpty())
            {
                m_gameManager.Move(Board.SelectedBoardPosition, position);
            }
            Board.ClearSelectedAndHintedBoardPositions();
        }

        protected override bool SquareClickHandlerCanExecute(BoardPosition poistion
                                                           , ITool?        tool)
        {
            return m_gameManager.IsGameRunning;
        }
    }
}
