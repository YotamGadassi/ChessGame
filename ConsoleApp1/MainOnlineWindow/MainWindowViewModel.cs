using System.Windows;

namespace OnlineFramework.MainOnlineWindow
{
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty CurrentViewProperty = DependencyProperty.Register("CurrentView", typeof(UIElement), typeof(MainWindowViewModel));

        public UIElement CurrentView
        {
            get => (UIElement)GetValue(CurrentViewProperty);
            set => SetValue(CurrentViewProperty, value);
        }

    }
}
