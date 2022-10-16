using System.Windows;
using System.Windows.Media;
using ChessGame;
using Common_6;

namespace Client.Game
{
    public class StatusBarViewModel : DependencyObject
    {
        private static readonly DependencyProperty CurrentTeamColorProperty = DependencyProperty.Register("CurrentTeamColor",typeof(Color), typeof(StatusBarViewModel), new FrameworkPropertyMetadata(Colors.White));

        public Color CurrentTeamColor
        {
            get => (Color)GetValue(CurrentTeamColorProperty);
            set => SetValue(CurrentTeamColorProperty, value);
        }

        private BaseGameManager m_gameManager;

        public StatusBarViewModel(BaseGameManager gameManager, Team localTeam, Team remoteTeam)
        {
            m_gameManager                 =  gameManager;
            m_gameManager.TeamSwitchEvent += onTeamSwitchEvent;
        }

        private void onTeamSwitchEvent(object? sender, Color e)
        {
            CurrentTeamColor = e;
        }
    }
}
