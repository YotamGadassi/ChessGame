using System;

namespace Client
{
    public class ClientHost
    {
        private static MainClientWindows m_mainClientWin;

        [STAThread]
        public static void Startup()
        {
            createControls();
            m_mainClientWin.ShowDialog();
        }

        private static void createControls()
        {
            m_mainClientWin = new MainClientWindows();
        }
    }
}
