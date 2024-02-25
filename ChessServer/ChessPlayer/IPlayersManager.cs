using ChessServer.Users;

namespace ChessServer.ChessPlayer
{
    public interface IPlayersManager
    {
        public Task AddNewPlayerAsync(UserUniqueId id, IServerChessPlayer player);

        public Task<IServerChessPlayer?> RemovePlayerAsync(UserUniqueId id);

        public Task<IServerChessPlayer?> GetPlayerDataAsync(UserUniqueId id);

    }
}
