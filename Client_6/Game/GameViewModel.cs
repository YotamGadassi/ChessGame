using System.Windows;
using System.Windows.Threading;
using ChessGame;
using Client.Board;
using Common;

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
}
