using System.Collections.Generic;
using System.Reflection;
using Common.MainWindow;
using FrontCommon;
using log4net;

namespace Host
{
    public class MainWindowViewModel : BaseMainWindowViewModel
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IGameButton[] GameButtons => m_gameButtons.ToArray();

        private List<IGameButton> m_gameButtons; 

        public MainWindowViewModel()
        {
            m_gameButtons = new List<IGameButton>();
        }

        public void AddGameButton(BaseGameButton gameButton)
        {
            m_gameButtons.Add(gameButton);
        }
    }
}