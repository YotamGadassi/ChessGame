using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Common;

namespace Client.Game
{
    public class TeamStatusViewModel : DependencyObject, IDisposable
    {
        private static readonly DependencyProperty IsTeamTurnProperty = DependencyProperty.Register("IsTeamTurn", typeof(bool), typeof(TeamStatusViewModel), new FrameworkPropertyMetadata(false));
        private static readonly DependencyProperty TimeLeftProperty = DependencyProperty.Register("TimeLeft", typeof(TimeSpan), typeof(TeamStatusViewModel), new PropertyMetadata(TimeSpan.FromMinutes(10)));

        private readonly Dispatcher m_dispatcher;
        private          Team       m_team;
        private          ITeamTimer m_teamTimer;

        public bool IsTeamTurn
        {
            get=> (bool)GetValue(IsTeamTurnProperty);
            private set=> SetValue(IsTeamTurnProperty, value);
        }

        public Color Color => m_team.Color;

        public GameDirection MovingDirection => m_team.MoveDirection;

        public string Name => m_team.Name;

        public TimeSpan TimeLeft
        {
            get=> (TimeSpan)GetValue(TimeLeftProperty);
            private set=> SetValue(TimeLeftProperty, value);
        }

        public TeamStatusViewModel(Team team, ITeamTimer teamTimer)
        {
            m_dispatcher    = Dispatcher.CurrentDispatcher;
            m_team = team;
            m_teamTimer = teamTimer;
            TimeLeft = m_teamTimer.TimeLeft;
            registerToEvents();
        }

        public void Dispose()
        {
            unRegisterFromEvents();
        }

        private void registerToEvents()
        {
            m_teamTimer.TimeLeftChange    += onTimeLeftChange;
            m_teamTimer.TimerStateChanged += onTimerStateChanged;
        }

        private void unRegisterFromEvents()
        {
            m_teamTimer.TimeLeftChange    -= onTimeLeftChange;
            m_teamTimer.TimerStateChanged -= onTimerStateChanged;
        }


        private void onTimerStateChanged(bool isTimerOn)
        {
            if (isTimerOn)
                IsTeamTurn = true;
            else
                IsTeamTurn= false;
        }

        private void onTimeLeftChange(TimeSpan timeLeft)
        {
            m_dispatcher.InvokeAsync(() => TimeLeft = timeLeft);
        }
    }
}
