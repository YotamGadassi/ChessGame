using Client.Board;
using Common;

namespace Client.Game;

public class OnlineGameViewModel : GameViewModel
{
    public OnlineGameViewModel(SquareViewModel.SquareClickCommandExecute squareClickHandler
                             , SquareViewModel.SquareClickCommandCanExecute squareClickCanExecute) : base(squareClickHandler, 
                                                                                                          squareClickCanExecute) { }

    public void StartGame(Team northTeam
                        , Team southTeam)
    {
        Message                = null;
        NorthTeamStatus        = new TeamStatusViewModel(northTeam);
        SouthTeamStatus        = new TeamStatusViewModel(southTeam);
    }

    public void EndGame()
    {
        Message           = null;
        NorthTeamStatus   = null;
        SouthTeamStatus   = null;
        Board.RemoveAllTools();
    }
}