using System.Timers;
using Common;
using Timer = System.Timers.Timer;

namespace ChessServer
{
    public class PlayerObject
    {
        public GameUnit1 GameUnit     { get; set; }
        public string   ConnectionId { get; }
        public string   Name         { get; }
        public Team     PlayersTeam  { get; set; }

        public event EventHandler<TimeSpan> OneSecPassEvent; 

        private Timer    m_timer;
        public TimeSpan TimeLeft;

        public PlayerObject(string  connectionId
                          , string  name)
        {
            ConnectionId = connectionId;
            Name         = name;
            m_timer      = new Timer(1000);
            TimeLeft     = TimeSpan.FromMinutes(10);
            m_timer.Elapsed += onElapsed;
        }

        private void onElapsed(object?          sender
                                  , ElapsedEventArgs e)
        {
            TimeLeft -= TimeSpan.FromSeconds(1);
            OneSecPassEvent?.Invoke(this, TimeLeft);
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
