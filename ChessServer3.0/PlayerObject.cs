using System.Timers;
using Common;
using Timer = System.Timers.Timer;

namespace ChessServer3._0
{
    public class PlayerObject
    {
        public GameUnit GameUnit     { get; set; }
        public string   ConnectionId { get; }
        public string   Name         { get; }
        public Team     PlayersTeam  { get; set; }

        private Timer    m_timer;
        public TimeSpan TimeLeft;

        public PlayerObject(string  connectionId
                          , string  name)
        {
            ConnectionId = connectionId;
            Name         = name;
            m_timer      = new Timer(1000);
            TimeLeft     = TimeSpan.FromMinutes(10);
        }

        public void StartTimer()
        {
            m_timer.Start();
        }

        public void StopTimer()
        {
            m_timer.Stop();
        }

    }
}
