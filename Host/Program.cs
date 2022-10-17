using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Host
{
    public class Program
    {
        private static ILog s_log;
        
        [STAThread]
        private static void Main(string[] args)
        {
            setUpLog();
            setUpUnhandledExceptions();
            s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            s_log.Info("App started");
            MainWindowControl   mainWinControl = new MainWindowControl();
            MainWindowViewModel mainWinVm      = new MainWindowViewModel();
            mainWinControl.DataContext = mainWinVm;
            mainWinControl.ShowDialog();
        }

        private static void setUpUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
        }

        private static void onUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex  = (e.ExceptionObject as Exception);
            string    log = $"{ex.Message}\n {ex.StackTrace}";
            s_log.Error(log);
            throw ex;
        }

        private static void setUpLog()
        {
            Stream? stream = Assembly.GetCallingAssembly().GetManifestResourceStream("Host.LogConfiguration.xml");
            XmlConfigurator.Configure(stream);
        }
    }
}