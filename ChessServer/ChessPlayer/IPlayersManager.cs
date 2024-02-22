using ChessServer.Users;

namespace ChessServer.ChessPlayer
{
    public interface IPlayersManager
    {
        public Task AddNewPlayerAsync(UserUniqueId userId
                               , PlayerData   playerData);

        public Task<PlayerData> RemovePlayerAsync(UserUniqueId playerId);

        public Task<PlayerData> GetPlayerDataAsync(UserUniqueId playerId);

    }
}
