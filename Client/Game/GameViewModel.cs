using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Client.Board;
using Common;
using log4net;
using static Client.Board.SquareViewModel;

namespace Client.Game
{
    public interface IGameViewModel
    {
        public BoardViewModel Board { get; }
    }

    public class GameViewModel : DependencyObject, IGameViewModel
    {
        protected   Dispatcher           m_dispatcher;

        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly DependencyProperty NorthTeamStatusProperty = DependencyProperty.Register("NorthTeamStatus", typeof(TeamStatusViewModel), typeof(OnlineGameViewModel));
        private static readonly DependencyProperty SouthTeamStatusProperty = DependencyProperty.Register("SouthTeamStatus", typeof(TeamStatusViewModel), typeof(OnlineGameViewModel));

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

        public BoardViewModel  Board { get; }
        public object Message
        {
            get => GetValue(messageProperty);
            set => SetValue(messageProperty, value);
        }

        private static readonly DependencyProperty messageProperty = DependencyProperty.Register("Message", typeof(object), typeof(GameViewModel));

        public GameViewModel(SquareClickCommandExecute squareClickHandler, SquareClickCommandCanExecute squareClickCanExecute)
        {
            m_dispatcher = Dispatcher.CurrentDispatcher;
            Board        = new BoardViewModel(squareClickHandler, squareClickCanExecute);
        }
        
        public void MoveTool(BoardPosition start, BoardPosition end, ITool tool)
        {
            Board.ClearSelectedAndHintedBoardPositions();
            Board.RemoveTool(start, out ITool toolAtStart);
            Board.AddTool(tool, end);
        }
    }
}
