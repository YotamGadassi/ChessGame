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

            Task.Run(matchMakerCallback);
        }

        public Task<GameRequestId> SubmitGameRequestAsync(IServerChessPlayer player)
        {
            GameRequestId   requestId   = GameRequestId.NewGameRequestId();
            GameRequestData requestData = new(requestId, player);
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
                GameRequestData requestData1;
                GameRequestData requestData2;
                lock (m_gameRequestLock)
                {
                    if (false == m_requestsQueue.TryDequeue(out GameRequestId requestId1)
                     || false == m_requestsQueue.TryDequeue(out GameRequestId? requestId2))
                    {
                        throw new Exception("Something is wrong with the request queue");
                    }

                    requestData1 = m_requestsDict[requestId1];
                    requestData2 = m_requestsDict[requestId2];

                    m_requestsDict.Remove(requestId1);
                    m_requestsDict.Remove(requestId2);
                }
                matchTwoRequests(requestData1, requestData2);
            }

            Task.Run(matchMakerCallback);
        }

        private void matchTwoRequests(GameRequestData requestData1
                                    , GameRequestData requestData2)
        {
            IServerChessPlayer player1 = requestData1.Player;
            IServerChessPlayer player2 = requestData2.Player;

            GameId   gameId   = GameId.NewGameId();
            GameUnit gameUnit = new(new[] { player1, player2 }, gameId, m_log);
            GameCreatedEvent?.Invoke(this, gameUnit);
        }
    }
}