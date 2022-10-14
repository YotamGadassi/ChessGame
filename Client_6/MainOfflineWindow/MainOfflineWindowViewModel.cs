using System.Windows;

namespace Client.MainOfflineWindow
{
    public class MainOfflineWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty CurrentViewProperty = DependencyProperty.Register("CurrentView", typeof(UIElement), typeof(MainOfflineWindowViewModel));

        public UIElement CurrentView
        {
            get => (UIElement)GetValue(CurrentViewProperty);
            set => SetValue(CurrentViewProperty, value);
        }

    }
}
