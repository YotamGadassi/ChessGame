using Common;
using OnlineChess.Common;

namespace OnlineChess.TeamManager
{
    internal class OnlineTeamTimer : ITeamTimer, IDisposable
    {
        public event Action<TimeSpan>? TimeLeftChange;
        public event Action<bool>? TimerStateChanged;
        public TimeSpan TimeLeft
        {
            get
            {
                lock (m_timeLock)
                {
                    return m_timeLeft;
                }
            }
        }

        private TimeSpan m_timeLeft;
        private IChessServerAgent m_agent;
        private TeamId m_teamId;
        private object m_timeLock;

        public OnlineTeamTimer(IChessServerAgent agent
                             , TeamId teamId
                             , TimeSpan timeLeft)
        {
            m_timeLock = new object();
            m_timeLeft = timeLeft;
            m_agent = agent;
            m_teamId = teamId;
            registerToEvents();
        }

        public void Dispose()
        {
            unregisterFromEvents();
        }

        private void registerToEvents()
        {
            m_agent.UpdateTimeEvent += onTimeReceived;
        }

        private void unregisterFromEvents()
        {
            m_agent.UpdateTimeEvent -= onTimeReceived;
        }

        private void onTimeReceived(TeamId teamId
                                  , TimeSpan timeleft)
        {
            if (m_teamId.Equals(teamId))
            {
                handleTimeLeftChanged(timeleft);
            }
        }

        private void handleTimeLeftChanged(TimeSpan timeleft)
        {
            lock (m_timeLock)
            {
                m_timeLeft = timeleft;
            }
            TimeLeftChange?.Invoke(timeleft);
        }

    }
}
