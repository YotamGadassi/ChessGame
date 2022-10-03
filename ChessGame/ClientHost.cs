using Client.Frameworks;
using System;

namespace Client
{
    public class ClientHost
    {
        private MainClientWindows m_mainClientWin;
        private OnlineFramework m_onlineFramework;

        [STAThread]
        public void Startup(string serverURL)
        {
            createControls();
            m_onlineFramework = new OnlineFramework(serverURL);
            m_mainClientWin.OnlineModeControl.DataContext = m_onlineFramework;
            try
            {
                m_mainClientWin.ShowDialog();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.Message);
                m_mainClientWin.Close();
                m_mainClientWin.ShowDialog();
            }
        }

        private void createControls()
        {
            m_mainClientWin = new MainClientWindows();
        }
    }
}
