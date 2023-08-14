using Client.Board;
using Common;

namespace Client.Game;

public class OfflineGameViewModel : GameViewModel
{
    public    TeamStatusViewModel NorthTeamStatus { get; }
    public    TeamStatusViewModel SouthTeamStatus { get; }

    public OfflineGameViewModel(Team                                         northTeam
                              , Team                                         southTeam
                              , SquareViewModel.SquareClickCommandExecute    squareClickCommandExecute
                              , SquareViewModel.SquareClickCommandCanExecute squareClickCommandCanExecute) :
        base(squareClickCommandExecute, squareClickCommandCanExecute)
    {
        NorthTeamStatus        = new TeamStatusViewModel(northTeam);
        SouthTeamStatus        = new TeamStatusViewModel(southTeam);
    }
}