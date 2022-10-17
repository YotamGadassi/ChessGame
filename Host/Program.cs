using System;

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
            mainWinControl.ShowDialog();
        }
    }
}