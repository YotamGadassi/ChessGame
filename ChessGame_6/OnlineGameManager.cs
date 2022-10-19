using Common;
using Common_6;

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
