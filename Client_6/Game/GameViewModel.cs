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
        protected BaseGameManager      m_gameManager;
        private   Dispatcher           m_dispatcher;
        protected AvailableMovesHelper m_availableMovesHelper;

        public BoardViewModel  Board { get; }
        public object Message
        {
            get => GetValue(messageProperty);
            set => SetValue(messageProperty, value);
        }

        private static readonly DependencyProperty messageProperty = DependencyProperty.Register("Message", typeof(object), typeof(BaseGameViewModel));

        protected BaseGameViewModel(BaseGameManager gameManager)
        {
            m_dispatcher                 =  Dispatcher.CurrentDispatcher;
            m_gameManager                =  gameManager;
            m_availableMovesHelper       =  new AvailableMovesHelper(gameManager);
            m_gameManager.CheckmateEvent += onCheckmateEventHandler;
            m_gameManager.PromotionEvent += onPromotionAsyncEvent;
            Board                        =  new BoardViewModel(SquareClickHandler, SquareClickHandlerCanExecute);

            registerEvents();
        }
        
        private async Task<ITool> onPromotionAsyncEvent(object             sender
                                                 , PromotionEventArgs e)
        {
            PromotionMessageViewModel promotionViewModel = new(e.ToolToPromote.Color);
            Message = promotionViewModel;
            ITool tool =  await promotionViewModel.ToolAwaiter;
            Message = null;
            Board.RemoveTool(e.ToolPosition, out _);
            Board.AddTool(tool, e.ToolPosition);
            return tool;
        }

        private void onCheckmateEventHandler(object?            sender
                                           , CheckmateEventArgs e)
        {
            Message = new UserMessageViewModel("Checkmate", "OK", () => Message = null);
        }

        private void registerEvents()
        {
            m_gameManager.ToolKilledEvent += moveHandler;
            m_gameManager.ToolMovedEvent  += moveHandler;
            m_gameManager.CheckmateEvent  += onCheckmateEvent;
        }

        private void moveHandler(object sender, ToolMovedEventArgs e)
        {
            Action<BoardPosition, BoardPosition, ITool> act = MoveTool;
            m_dispatcher.BeginInvoke(act, e.InitialPosition, e.EndPosition, e.MovedTool);
        }

        protected void MoveTool(BoardPosition start, BoardPosition end, ITool tool)
        {
            Board.ClearSelectedAndHintedBoardPositions();
            Board.RemoveTool(start, out ITool toolAtStart);
            Board.AddTool(tool, end);
        }

        private void onCheckmateEvent(object?            sender
                                    , CheckmateEventArgs e)
        {
            m_dispatcher.Invoke(m_gameManager.EndGame);
        }

        protected virtual void SquareClickHandler(BoardPosition position
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

        protected virtual bool SquareClickHandlerCanExecute(BoardPosition poistion
                                                          , ITool?        tool)
        {
            return m_gameManager.IsGameRunning;
        }

    }

    public class OfflineGameViewModel : BaseGameViewModel
    {
        public  TeamStatusViewModel NorthTeamStatus { get; }
        public  TeamStatusViewModel SouthTeamStatus { get; }

        protected AvailableMovesHelper m_availableMovesHelper;
        public OfflineGameViewModel(BaseGameManager gameManager, Team northTeam, Team southTeam) : base(gameManager)
        {
            m_availableMovesHelper = new AvailableMovesHelper(gameManager);
            NorthTeamStatus        = new TeamStatusViewModel(northTeam);
            SouthTeamStatus        = new TeamStatusViewModel(southTeam);
        }
    }

    public class OnlineGameViewModel : BaseGameViewModel
    {
        public TeamStatusViewModel NorthTeamStatus { get; }
        public TeamStatusViewModel SouthTeamStatus { get; }

        private Team                 m_localTeam;

        public OnlineGameViewModel(BaseGameManager gameManager, Team northTeam, Team southTeam, Team localTeam) : base(gameManager)
        {
            m_localTeam            = localTeam;
            NorthTeamStatus        = new TeamStatusViewModel(northTeam);
            SouthTeamStatus        = new TeamStatusViewModel(southTeam);
        }

        protected override bool SquareClickHandlerCanExecute(BoardPosition poistion
                                                           , ITool?        tool)
        {
            if (m_gameManager.CurrentColorTurn != m_localTeam.Color)
            {
                return false;
            }

            return base.SquareClickHandlerCanExecute(poistion, tool);
        }
    }
}
