using Common;
using OnlineChess.Common;

namespace OnlineChess.Game
{
    internal class OnlineGameState : IGameState
    {
        public event EventHandler<GameStateEnum>? StateChanged;
        public GameStateEnum                      State { get; private set; }

        private readonly IChessServerAgent m_serverAgent;

        public OnlineGameState(IChessServerAgent serverAgent)
        {
            State         = GameStateEnum.NotStarted;
            m_serverAgent = serverAgent;
            registerToEvent();
        }

        public void Dispose()
        {
            unRegisterFromEvents();
        }

        private void registerToEvent()
        {
            m_serverAgent.StartGameEvent += onGameStarted;
            m_serverAgent.EndGameEvent   += onGameEnded;
        }

        private void unRegisterFromEvents()
        {
            m_serverAgent.StartGameEvent -= onGameStarted;
            m_serverAgent.EndGameEvent   -= onGameEnded;
        }

        private void onGameEnded(EndGameReason reason)
        {
            State = GameStateEnum.Ended;
            StateChanged?.Invoke(this, State);
        }

        private void onGameStarted(GameConfig gameConfig)
        {
            State = GameStateEnum.Running;
            StateChanged?.Invoke(this, State);
        }
    }
}