using Client.Frameworks;
using System;
using System.Threading;

namespace Client
{
    public class ClientHost
    {
        private static MainClientWindows m_mainClientWin;
        private static OnlineFramework onlineFramework;

        [STAThread]
        public static void Startup()
        {
            createControls();
            onlineFramework = new OnlineFramework(m_mainClientWin);
            m_mainClientWin.m_OnlineModeControl.DataContext = onlineFramework;
            m_mainClientWin.ShowDialog();
        }

        private static void createControls()
        {
            m_mainClientWin = new MainClientWindows();
        }
    }
}
