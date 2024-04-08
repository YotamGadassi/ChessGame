using ChessServer.Game;
using ChessServer.Users;

namespace ChessServer.ServerManager
{
    public interface IServerManager<T>
    {
        public IUsersManager<T> UsersManager { get; }

        public IGamesManager GamesManager { get; }
    }
}
