using System.Collections.Concurrent;
using ChessServer.ChessPlayer;
using ChessServer.Users;
using OnlineChess.Common;

namespace ChessServer.Game;

public class GameManager : IGamesManager, IDisposable
{
    private readonly GameRequestsManager                           m_gameRequestsManager;
    private readonly ConcurrentDictionary<UserUniqueId, IGameUnit> m_userToGame;
    private readonly ConcurrentDictionary<GameId, IGameUnit>       m_games;
    private readonly ILogger                                       m_log;
    private readonly object                                        m_gameLock = new();

    public GameManager(ILogger log)
    {
        m_log        = log;
        m_userToGame = new ConcurrentDictionary<UserUniqueId, IGameUnit>();
        m_games      = new ConcurrentDictionary<GameId, IGameUnit>();

        m_gameRequestsManager = new GameRequestsManager(log);
        registerToEvents();
    }

    public Task<GameRequestId> SubmitGameRequestAsync(IServerChessPlayer player) => m_gameRequestsManager.SubmitGameRequestAsync(player);

    public Task CancelGameRequestAsync(GameRequestId requestId) => m_gameRequestsManager.CancelGameRequestAsync(requestId);

    public Task<IGameUnit?> GetGameAsync(UserUniqueId userId)
    {
        m_log.LogDebug("Get Game Id Called for user id: {0}", userId);
        if (false == m_userToGame.TryGetValue(userId, out IGameUnit? game))
        {
            m_log.LogError("No game unit for Player Id: {0}", userId);
        }

        return Task.FromResult(game);
    }

    public Task<IGameUnit?> RemoveGameAsync(GameId gameId)
    {
        removeGame(gameId, out IGameUnit? gameUnit);
        return Task.FromResult(gameUnit);
    }
    
    public void Dispose()
    {
        unRegitesrFromEvents();
    }

    private void addGame(IGameUnit game)
    {
        m_log.LogInformation("Game Added: [Game Id:{0}]", game.Id);
        game.GameEndedEvent += onGameEndedEvent;
        lock (m_gameLock)
        {
            m_games.TryAdd(game.Id, game);
            foreach (IServerChessPlayer player in game.ChessPlayers)
            {
                m_userToGame.TryAdd(player.UserUniqueId, game);
            }
        }
        game.StartGame();
    }

    private bool removeGame(GameId gameId, out IGameUnit? gameUnit)
    {
        m_log.LogInformation("Game Removed: [Game Id:{0}]", gameId);

        lock (m_gameLock)
        {
            if (false == m_games.TryRemove(gameId, out gameUnit))
            {
                m_log.LogError("Game Unit with id: {0} cannot be removed", gameId);
                return false;
            }

            gameUnit.GameEndedEvent -= onGameEndedEvent;

            foreach (IServerChessPlayer player in gameUnit.ChessPlayers)
            {
                if (false == m_userToGame.TryRemove(player.UserUniqueId, out _))
                {
                    m_log.LogError("Player id: {0} cannot be removed dictionary", player.PlayerId);
                }
            }
        }

        return true;
    }

    private void registerToEvents()
    {
        m_gameRequestsManager.GameCreatedEvent += onGameCreated;
    }

    private void unRegitesrFromEvents()
    {
        m_gameRequestsManager.GameCreatedEvent -= onGameCreated;
    }

    private void onGameCreated(object?   sender
                             , IGameUnit game)
    {
        addGame(game);
    }

    private void onGameEndedEvent(object sender, GameEndedEventArgs args)
    {
        IGameUnit game = args.GameUnit;
        if (game is IDisposable disposableGame)
        {
            disposableGame.Dispose();
        }
        removeGame(args.GameUnit.Id, out _);
    }

}