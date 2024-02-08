using System.Windows.Input;
using System.Windows.Threading;

namespace FrontCommon
{
    public interface IGameButton
    {
        ICommand ButtonCommand { get; }
        string   PanelGameName { get; }
        string CommandName { get; }
    }

    public abstract class BaseGameButton : IGameButton
    {
        public          ICommand ButtonCommand { get; }
        public abstract string   PanelGameName { get; }

        public abstract string CommandName { get; }

        protected readonly Dispatcher m_dispatcher;

        protected readonly IGamePanelManager m_panelManager;

        protected BaseGameButton(Dispatcher        dispatcer
                               , IGamePanelManager panelManager)
        {
            m_dispatcher   = dispatcer;
            m_panelManager = panelManager;
            ButtonCommand  = new WpfCommand(playCommandExecute, playCommandCanExecute);
        }

        protected virtual BaseGamePanel getPanel()
        {
            bool isExists = m_panelManager.TryGetPanel(PanelGameName, out BaseGamePanel panel);
            if (false == isExists)
            {
                throw new KeyNotFoundException($"Panel: {PanelGameName} does not exist");
            }
            return panel;
        }

        protected virtual void playCommandExecute(object parameter)
        {
            BaseGamePanel gamePanel = getPanel();
            gamePanel.Init();
            m_panelManager.Show(gamePanel);
        }

        private bool playCommandCanExecute(object parameter)
        {
            BaseGamePanel? currPanel = m_panelManager.CurrentPanel;
            return null == currPanel;
        }
    }
}