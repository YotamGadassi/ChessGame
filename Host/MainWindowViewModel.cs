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

        public IGameButtonViewModel[] GameButtons => m_gameButtons.ToArray();

        private List<IGameButtonViewModel> m_gameButtons; 

        public MainWindowViewModel()
        {
            m_gameButtons = new List<IGameButtonViewModel>();
        }

        public void AddGameButton(BaseGameButtonViewModel gameButton)
        {
            m_gameButtons.Add(gameButton);
        }
    }
}