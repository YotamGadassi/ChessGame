using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Common;

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
        private static readonly DependencyProperty TimeLeftProperty = DependencyProperty.Register("TimeLeft", typeof(TimeSpan), typeof(TeamStatusViewModel), new PropertyMetadata(TimeSpan.FromMinutes(10)));


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

        public TimeSpan TimeLeft
        {
            get=> (TimeSpan)GetValue(TimeLeftProperty);
            set=> SetValue(TimeLeftProperty, value);
        }

        private readonly Timer m_timer;

        public TeamStatusViewModel(Team team)
        {
            m_dispatcher    = Dispatcher.CurrentDispatcher;
            Color           = team.Color;
            MovingDirection = team.MoveDirection;
            Name            = team.Name;
            TimeLeft = TimeSpan.FromMinutes(10);
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

    }
}
