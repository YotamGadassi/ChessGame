using System;
using System.Threading;
using System.Timers;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace Common;

public class TeamWithTimer : Team
{
    private static readonly double s_elapsedTimerIntervalMs = 1000;
    private readonly        Timer  m_timer;
    private                 object m_lock = new();

    public event Action<TimeSpan> TimeLeftChange;
    public event Action<bool> TimerStateChanged;

    public TeamWithTimer(string        name
                       , Color         color
                       , GameDirection moveDirection
                       , TimeSpan      totalTime) :
        base(name, color, moveDirection)
    {
        m_timer           =  new Timer(s_elapsedTimerIntervalMs);
        m_timer.AutoReset =  true;
        TimeLeft          =  totalTime;
        m_timer.Elapsed   += TimerOnElapsed;
    }

    private void TimerOnElapsed(object?          sender
                              , ElapsedEventArgs e)
    {
        lock (m_lock)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            ThreadPool.QueueUserWorkItem((_) => TimeLeftChange?.Invoke(TimeLeft));
        }
    }

    public TimeSpan TimeLeft { get; private set; }

    public void StartTimer()
    {
        m_timer.Start();
        TimerStateChanged?.Invoke(true);
    }

    public void StopTimer()
    {
        m_timer.Stop();
        TimerStateChanged?.Invoke(false);
    }

    public void SetTimeLeft(TimeSpan timeLeft)
    {
        lock (m_lock)
        {
            TimeLeft = timeLeft;
            ThreadPool.QueueUserWorkItem((_) => TimeLeftChange?.Invoke(TimeLeft));
        }

    }
}