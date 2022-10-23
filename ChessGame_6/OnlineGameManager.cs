using Common;

namespace ChessGame
{
    public class OnlineGameManager : BaseGameManager
    {
        public Team CurrentMachineTeam { get; }

        public OnlineGameManager(Team currentMachineTeam)
        {
            CurrentMachineTeam = currentMachineTeam;
        }
    }
}
