using ChessServer.Users;

namespace ChessServer.ChessPlayer
{
    public interface IPlayersManager
    {
        public void AddNewPlayer(UserUniqueId userId
                               , PlayerData   playerData);

        public PlayerData RemovePlayer(UserUniqueId playerId);

        public PlayerData GetPlayerData(UserUniqueId playerId);

    }
}
