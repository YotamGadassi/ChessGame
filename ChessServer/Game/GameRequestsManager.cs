using System.Collections.Concurrent;
using ChessServer.ChessPlayer;
using OnlineChess.Common;
using Utils.DataStructures;

namespace ChessServer.Game
{
    public class GameRequestsManager
    {
        private UniqueQueue<GameRequestId>                           m_requestsQueue;
        private ConcurrentDictionary<GameRequestId, GameRequestData> m_requestsDict;
        private ILogger                                              m_log;
        private object                                               m_gameRequestLock = new();

        public event EventHandler<IGameUnit> GameCreatedEvent; 

        public GameRequestsManager(ILogger log)
        {
            m_log           = log;
            m_requestsQueue = new UniqueQueue<GameRequestId>();
            m_requestsDict  = new ConcurrentDictionary<GameRequestId, GameRequestData>();
        }

        public Task<GameRequestId> SubmitGameRequestAsync(PlayerData playerData)
        {
            GameRequestId   requestId   = GameRequestId.NewGameRequestId();
            GameRequestData requestData = new(requestId, playerData);
            m_log.LogInformation("Game Request Submitted: ID: {0}, RequestData: {1}", requestId, requestData);

            if (false == m_requestsDict.TryAdd(requestId, requestData))
            {
                throw new ArgumentException($"Request Id: {requestId} already exists in dictionary");
            }

            if (false == m_requestsQueue.TryEnqueue(requestId))
            {
                throw new ArgumentException($"Request Id: {requestId} already exists in queue");
            }

            return Task.FromResult(requestId);
        }

        public Task CancelGameRequestAsync(GameRequestId requestId)
        {
            m_log.LogInformation("Game Request Canceled: ID: {0}", requestId);
            if (false == m_requestsQueue.TryRemove(requestId))
            {
                m_log.LogInformation("Game Request Canceled - Request Id: {0} not exits in queue", requestId);
            }

            if (false == m_requestsDict.Remove(requestId, out _))
            {
                m_log.LogError("Game Request Canceled - Request Id: {0} not exits in dictionary", requestId);
            }

            return Task.CompletedTask;
        }

        private void matchMakerCallback()
        {

        }
    }
}
