using System.Windows.Media;
using System.Windows.Threading;
using OnlineFramework.MainOnlineWindow;

namespace Host
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            MainWindowControl   mainWinControl = new MainWindowControl();
            MainWindowViewModel mainWinVm      = new MainWindowViewModel();
            mainWinControl.DataContext = mainWinVm;
            OnlineFramework.OnlineFramework
                onlineFramework = new OnlineFramework.OnlineFramework(mainWinControl, mainWinVm);
            onlineFramework.Init();
            onlineFramework.RequestGameFromServer();
            //Dispatcher.CurrentDispatcher.InvokeAsync(() => onlineFramework.DummyStartGame(Colors.Black));
            mainWinControl.ShowDialog();
        }
    }
}