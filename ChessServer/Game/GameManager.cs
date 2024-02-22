using System.Collections.Concurrent;
using ChessServer.ChessPlayer;
using log4net;
using OnlineChess.Common;
using Utils.DataStructures;

namespace ChessServer.Game;

public class GameRequestData
{
    public GameRequestId GameRequestId { get; }

    public PlayerData PlayerData { get; }

    public GameRequestData(GameRequestId gameRequestId
                         , PlayerData    playerData)
    {
        GameRequestId = gameRequestId;
        PlayerData    = playerData;
    }
}

public class GameManager : IGamesManager, IDisposable
{
    private GameRequestsManager                       m_gameRequestsManager;
    private ConcurrentDictionary<PlayerId, IGameUnit> m_PlayerToGame;
    private ConcurrentDictionary<GameId, IGameUnit>   m_games;
    private ILogger                                   m_log;
    private object                                    m_gameLock = new();


    public GameManager(ILogger log)
    {
        m_log                 = log;
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

    public Task<GameRequestId> SubmitGameRequestAsync(PlayerData playerData) => m_gameRequestsManager.SubmitGameRequestAsync(playerData);

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

    public Task<IGameUnit> RemoveGameAsync(GameId gameId)
    {
        removeGame(gameId);
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
    }

    private void removeGame(GameId gameId)
    {
        m_log.LogInformation("Game Removed: [Game Id:{0}]", gameId);
        lock (m_gameLock)
        {
            if (false == m_games.TryRemove(gameId, out IGameUnit? game))
            {
                m_log.LogError("Game Unit with id: {0} already removed", gameId);
                return;
            }

            foreach (ServerChessPlayer player in game.ChessPlayers)
            {
                m_PlayerToGame.TryRemove(player.PlayerId,out _);
            }
        }

    }
}