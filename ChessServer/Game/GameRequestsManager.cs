using ChessServer.ChessPlayer;
using OnlineChess.Common;
using Utils.DataStructures;

namespace ChessServer.Game
{
    public class GameRequestsManager
    {
        public event EventHandler<IGameUnit> GameCreatedEvent;

        private readonly UniqueQueue<GameRequestId>                 m_requestsQueue;
        private readonly Dictionary<GameRequestId, GameRequestData> m_requestsDict;
        private readonly ILogger                                    m_log;
        private readonly object                                     m_gameRequestLock = new();

        public GameRequestsManager(ILogger log)
        {
            m_log           = log;
            m_requestsQueue = new UniqueQueue<GameRequestId>();
            m_requestsDict  = new Dictionary<GameRequestId, GameRequestData>();
        }

        public Task<GameRequestId> SubmitGameRequestAsync(PlayerData playerData)
        {
            GameRequestId   requestId   = GameRequestId.NewGameRequestId();
            GameRequestData requestData = new(requestId, playerData);
            m_log.LogInformation("Game Request Submitted: ID: {0}, RequestData: {1}", requestId, requestData);

            lock (m_gameRequestLock)
            {
                if (false == m_requestsDict.TryAdd(requestId, requestData))
                {
                    throw new ArgumentException($"Request Id: {requestId} already exists in dictionary");
                }

                if (false == m_requestsQueue.TryEnqueue(requestId))
                {
                    throw new ArgumentException($"Request Id: {requestId} already exists in queue");
                }
            }

            return Task.FromResult(requestId);
        }

        public Task CancelGameRequestAsync(GameRequestId requestId)
        {
            m_log.LogInformation("Game Request Canceled: ID: {0}", requestId);
            lock (m_gameRequestLock)
            {
                if (false == m_requestsQueue.TryRemove(requestId))
                {
                    m_log.LogInformation("Game Request Canceled - Request Id: {0} not exits in queue", requestId);
                }

                if (false == m_requestsDict.Remove(requestId, out _))
                {
                    m_log.LogError("Game Request Canceled - Request Id: {0} not exits in dictionary", requestId);
                }
            }

            return Task.CompletedTask;
        }

        private void matchMakerCallback()
        {
            while (m_requestsDict.Count > 1)
            {
                GameRequestId requestId1;
                GameRequestId requestId2;
                lock (m_gameRequestLock)
                {
                    if (false == m_requestsQueue.TryDequeue(out requestId1)
                     || false == m_requestsQueue.TryDequeue(out requestId2))
                    {
                        throw new Exception("Something is wrong with the request queue");
                    }
                    m_requestsDict.Remove(requestId1);
                    m_requestsDict.Remove(requestId2);
                }
                matchTwoRequests(requestId1, requestId2);
            }
        }

        private void matchTwoRequests(GameRequestId requestId1
                                     , GameRequestId requestId2)
        {



        }
    }
}