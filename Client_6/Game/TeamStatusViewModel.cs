using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Common_6;

namespace Client.Game
{
    public class TeamStatusViewModel : DependencyObject
    {
        private static readonly double   s_elapsedTimerIntervalMs = 1000;
        private                 DateTime m_lastElapsedTime;

        private static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color",typeof(Color), typeof(TeamStatusViewModel), new FrameworkPropertyMetadata(Colors.White));
        private static readonly DependencyProperty MovingDirectionProperty = DependencyProperty.Register("MovingDirection", typeof(GameDirection), typeof(TeamStatusViewModel));
        private static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(TeamStatusViewModel));
        private static readonly DependencyProperty IsTeamTurnProperty = DependencyProperty.Register("IsTeamTurn", typeof(bool), typeof(TeamStatusViewModel), new FrameworkPropertyMetadata(false));
        private static readonly DependencyProperty TimerProperty = DependencyProperty.Register("Timer", typeof(TimeSpan), typeof(TeamStatusViewModel), new PropertyMetadata(TimeSpan.FromMinutes(10)));


        private readonly Dispatcher m_dispatcher;

        public bool IsTeamTurn
        {
            get=> (bool)GetValue(IsTeamTurnProperty);
            set=> SetValue(IsTeamTurnProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public GameDirection MovingDirection
        {
            get => (GameDirection)GetValue(MovingDirectionProperty);
            set => SetValue(MovingDirectionProperty, value);
        }

        public string Name
        {
            get=> (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public TimeSpan Timer
        {
            get=> (TimeSpan)GetValue(TimerProperty);
            set=> SetValue(TimerProperty, value);
        }

        private readonly Timer m_timer;

        public TeamStatusViewModel(Team team)
        {
            m_dispatcher    = Dispatcher.CurrentDispatcher;
            Color           = team.Color;
            MovingDirection = team.MoveDirection;
            Name            = team.Name;
            m_timer         = new Timer();
            setTimer();
        }

        public void StartTimer()
        {
            m_lastElapsedTime = DateTime.Now;
            m_timer.Start();
        }

        public void StopTimer()
        {
            m_timer.Stop();
        }

        public void SetTeamTurn(bool isTeamTurn)
        {
            IsTeamTurn = isTeamTurn;
        }

        private void setTimer()
        {
            m_timer.Interval =  s_elapsedTimerIntervalMs;
            m_timer.Elapsed  += onElapsed;

        }

        private void onElapsed(object? sender, ElapsedEventArgs e)
        {
            m_dispatcher.Invoke(
                                () =>
                                {
                                    DateTime currentTime = DateTime.Now;
                                    TimeSpan diff        = currentTime - m_lastElapsedTime;
                                    m_lastElapsedTime =  currentTime;
                                    Timer             -= diff;
                                });
        }


    }
}
