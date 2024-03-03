using System.Collections.Concurrent;
using ChessServer.ChessPlayer;
using OnlineChess.Common;

namespace ChessServer.Game;

public class GameManager : IGamesManager, IDisposable
{
    public event EventHandler<IGameUnit>? GameEndedEvent;
    
    private readonly GameRequestsManager                       m_gameRequestsManager;
    private readonly ConcurrentDictionary<PlayerId, IGameUnit> m_playerToGame;
    private readonly ConcurrentDictionary<GameId, IGameUnit>   m_games;
    private readonly ILogger                                   m_log;
    private readonly object                                    m_gameLock = new();

    public GameManager(ILogger log)
    {
        m_log                 = log;
        m_playerToGame        = new ConcurrentDictionary<PlayerId, IGameUnit>();
        m_games               = new ConcurrentDictionary<GameId, IGameUnit>();

        m_gameRequestsManager = new GameRequestsManager(log);
        registerToEvents();
    }

    public Task<GameRequestId>            SubmitGameRequestAsync(IServerChessPlayer player) => m_gameRequestsManager.SubmitGameRequestAsync(player);

    public Task CancelGameRequestAsync(GameRequestId requestId) => m_gameRequestsManager.CancelGameRequestAsync(requestId);

    public Task<IGameUnit?> GetGameAsync(PlayerId playerId)
    {
        m_log.LogDebug("Get Game Id Called for player id: {0}", playerId);
        if (false == m_playerToGame.TryGetValue(playerId, out IGameUnit? game))
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
                m_playerToGame.TryAdd(player.PlayerId, game);
            }
        }
        game.StartGame();
    }

    private void onGameEndedEvent(object sender, GameEndedEventArgs args)
    {
        IGameUnit game = args.GameUnit;
        if (game is IDisposable disposableGame)
        {
            disposableGame.Dispose();
        }
        removeGame(args.GameUnit.Id, out _);
        GameEndedEvent?.Invoke(this, args.GameUnit);
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
                if (false == m_playerToGame.TryRemove(player.PlayerId, out _))
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

}