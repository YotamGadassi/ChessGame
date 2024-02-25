using System.Collections.Concurrent;
using ChessServer.Users;

namespace ChessServer.ChessPlayer;

public class PlayersManager : IPlayersManager
{
    private ConcurrentDictionary<UserUniqueId, IServerChessPlayer> m_idToPlayer;

    public PlayersManager()
    {
        m_idToPlayer = new ConcurrentDictionary<UserUniqueId, IServerChessPlayer>();
    }

    public Task AddNewPlayerAsync(UserUniqueId       id
                                , IServerChessPlayer player)
    {
        if (false == m_idToPlayer.TryAdd(id, player))
        {
            throw new ArgumentException(string.Format("Player with id: {0} already exist", id));
        }
        return Task.CompletedTask;
    }

    public Task<IServerChessPlayer?> RemovePlayerAsync(UserUniqueId id)
    {
        m_idToPlayer.TryRemove(id, out IServerChessPlayer? player);

        return Task.FromResult(player);
    }

    public Task<IServerChessPlayer?> GetPlayerDataAsync(UserUniqueId id)
    {
        if (false == m_idToPlayer.TryGetValue(id, out IServerChessPlayer? player))
        {
            throw new ArgumentException(string.Format("Player with id: {0} already exist", id));
        }

        return Task.FromResult(player);
    }
}