using System.Windows;

namespace Client.Messages
{
    /// <summary>
    /// Interaction logic for UserMessage.xaml
    /// </summary>
    public partial class UserMessage : Window
    {

        private static readonly DependencyProperty ButtonPressedProperty =
            DependencyProperty.Register("ButtonPressed", typeof(ButtonPressed), typeof(UserMessage), new PropertyMetadata(ButtonPressed.UnDefined));

        public ButtonPressed ButtonPressed
        {
            get=> (ButtonPressed)GetValue(ButtonPressedProperty);
            set => SetValue(ButtonPressedProperty, value);
        }

        public UserMessage(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }
    }
}
