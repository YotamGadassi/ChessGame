using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Common;

public interface ITeamTimer
{
    public event Action<TimeSpan> TimeLeftChange;
    public event Action<bool>     TimerStateChanged;

    TimeSpan TimeLeft { get; }
}

public class TeamTimer : ITeamTimer, IDisposable
{
    public event Action<TimeSpan> TimeLeftChange;
    public event Action<bool> TimerStateChanged;
    
    private readonly Timer m_timer;
    private readonly object m_lock = new();

    public TeamTimer(TimeSpan      totalTime, TimeSpan timerInterval)
    {
        m_timer           =  new Timer(timerInterval.TotalMilliseconds);
        m_timer.AutoReset =  true;
        TimeLeft          =  totalTime;
        m_timer.Elapsed   += TimerOnElapsed;
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

    private void TimerOnElapsed(object?          sender
                              , ElapsedEventArgs e)
    {
        lock (m_lock)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            ThreadPool.QueueUserWorkItem((_) => TimeLeftChange?.Invoke(TimeLeft));
        }
    }

    public void Dispose()
    {
        m_timer.Dispose();
    }
}