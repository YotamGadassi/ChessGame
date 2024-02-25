using System.Collections.Concurrent;
using ChessServer.ChessPlayer;
using log4net;
using OnlineChess.Common;
using Utils.DataStructures;

namespace ChessServer.Game;

public class GameManager : IGamesManager, IDisposable
{
    private readonly GameRequestsManager                       m_gameRequestsManager;
    private readonly ConcurrentDictionary<PlayerId, IGameUnit> m_PlayerToGame;
    private readonly ConcurrentDictionary<GameId, IGameUnit>   m_games;
    private readonly ILogger                                   m_log;
    private readonly object                                    m_gameLock = new();


    public GameManager(ILogger log)
    {
        m_log                 = log;
        m_PlayerToGame        = new ConcurrentDictionary<PlayerId, IGameUnit>();
        m_games               = new ConcurrentDictionary<GameId, IGameUnit>();

        m_gameRequestsManager = new GameRequestsManager(log);
        registerToEvents();
    }

    private void registerToEvents()
    {
        m_gameRequestsManager.GameCreatedEvent += onGameCreated;
    }

    private void onGameCreated(object?   sender
                             , IGameUnit game)
    {
        addGame(game);
    }

    public Task<GameRequestId> SubmitGameRequestAsync(IServerChessPlayer player) => m_gameRequestsManager.SubmitGameRequestAsync(player);

    public Task CancelGameRequestAsync(GameRequestId requestId) => m_gameRequestsManager.CancelGameRequestAsync(requestId);

    public Task<IGameUnit?> GetGameAsync(PlayerId playerId)
    {
        m_log.LogDebug("Get Game Id Called for player id: {0}", playerId);
        if (false == m_PlayerToGame.TryGetValue(playerId, out IGameUnit? game))
        {
            m_log.LogError("No game unit for Player Id: {0}", playerId);
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
        m_gameRequestsManager.GameCreatedEvent -= onGameCreated;
    }

    private void addGame(IGameUnit game)
    {
        m_log.LogInformation("Game Added: [Game Id:{0}]", game.Id);
        lock (m_gameLock)
        {
            m_games.TryAdd(game.Id, game);
            foreach (ServerChessPlayer player in game.ChessPlayers)
            {
                m_PlayerToGame.TryAdd(player.PlayerId, game);
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
                m_log.LogError("Game Unit with id: {0} already removed", gameId);
                return false;
            }

            foreach (ServerChessPlayer player in gameUnit.ChessPlayers)
            {
                m_PlayerToGame.TryRemove(player.PlayerId,out _);
            }
        }

        return true;
    }
}