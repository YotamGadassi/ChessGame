namespace ChessGame
{
    public class OnlineGameManager : OfflineGameManager
    {
        public Team CurrentMachineTeam { get; }

        public OnlineGameManager(Team currentMachineTeam)
        {
            CurrentMachineTeam = currentMachineTeam;
        }
    }
}
