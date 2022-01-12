using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        public void Application_Startup(object sender, StartupEventArgs args)
        {
            ClientHost.Startup();
        }

    }
}
