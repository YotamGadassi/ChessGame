using System.Windows;
using ChessGame;
using Client.Board;
using Common_6;

namespace Client.Game
{
    public interface IGameViewModel
    {
        public BaseBoardViewModel Board { get; }
    }

    public abstract class BaseGameViewModel : DependencyObject, IGameViewModel
    {
        public BaseBoardViewModel Board { get; }

        protected BaseGameViewModel(BaseBoardViewModel boardVm)
        {
            Board = boardVm;
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
