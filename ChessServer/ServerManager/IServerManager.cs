using ChessServer.ChessPlayer;
using ChessServer.Game;
using ChessServer.Users;

namespace ChessServer.ServerManager
{
    public interface IServerManager<in T>
    {
        public IUsersManager<T> UsersManager { get; }

        public IGamesManager GamesManager { get; }

        public IPlayersManager PlayersManager { get; }
    }
}
