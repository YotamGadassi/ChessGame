using System.Threading.Tasks;
using System.Windows;
using ChessGame;
using Client.Board;
using Client.Messages;
using Common;
using Common.ChessBoardEventArgs;

namespace Client.Game
{
    public interface IGameViewModel
    {
        public BaseBoardViewModel Board { get; }
    }

    public abstract class BaseGameViewModel : DependencyObject, IGameViewModel
    {
        public BaseBoardViewModel Board { get; }

        private static readonly DependencyProperty messageProperty = DependencyProperty.Register("Message", typeof(object), typeof(BaseGameViewModel));

        public object Message
        {
            get => GetValue(messageProperty);
            set => SetValue(messageProperty, value);
        }

        protected BaseGameViewModel(BaseBoardViewModel boardVm)
        {
            Board = boardVm;
            Board.CheckmateEventHandler += onCheckmateEventHandler;
            Board.PromotionEvent += onPromotionAsyncEvent;
        }
        
        private async Task<ITool> onPromotionAsyncEvent(object             sender
                                                 , PromotionEventArgs e)
        {
            PromotionMessageViewModel promotionViewModel = new PromotionMessageViewModel(e.ToolToPromote.Color);
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
    }

    public class OfflineGameViewModel : BaseGameViewModel
    {
        public TeamStatusViewModel NorthTeamStatus { get; }
        public TeamStatusViewModel SouthTeamStatus { get; }

        public OfflineGameViewModel(BaseGameManager gameManager, Team northTeam, Team southTeam) : base(new OfflineBoardViewModel(gameManager))
        {
            NorthTeamStatus = new TeamStatusViewModel(northTeam);
            SouthTeamStatus = new TeamStatusViewModel(southTeam);
        }
    }

    public class OnlineGameViewModel : BaseGameViewModel
    {
        public TeamStatusViewModel NorthTeamStatus { get; }
        public TeamStatusViewModel SouthTeamStatus { get; }

        public OnlineGameViewModel(BaseGameManager gameManager, Team northTeam, Team southTeam, Team localTeam) : base(new OnlineBoardViewModel(gameManager, localTeam))
        {
            NorthTeamStatus = new TeamStatusViewModel(northTeam);
            SouthTeamStatus = new TeamStatusViewModel(southTeam);
        }
    }
}
