using System.Windows.Input;
using System.Windows.Threading;
using FrontCommon.GamePanel;

namespace FrontCommon
{
    public interface IGameButton
    {
        ICommand ButtonCommand { get; }
        string   PanelGameName { get; }
    }

    public abstract class BaseGameButton : IGameButton
    {
        public          ICommand ButtonCommand { get; }
        public abstract string   PanelGameName { get; }

        public abstract string CommandName { get; }

        protected readonly Dispatcher Dispatcher;

        protected readonly IGamePanelManager PanelManager;

        protected BaseGameButton(Dispatcher        dispatcher
                               , IGamePanelManager panelManager)
        {
            Dispatcher   = dispatcher;
            PanelManager = panelManager;
            ButtonCommand  = new WpfCommand(playCommandExecute, playCommandCanExecute);
        }

        protected virtual BaseGamePanel getPanel()
        {
            bool isExists = PanelManager.TryGetPanel(PanelGameName, out BaseGamePanel panel);
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
            PanelManager.Show(gamePanel);
        }

        private bool playCommandCanExecute(object parameter)
        {
            BaseGamePanel? currPanel = PanelManager.CurrentPanel;
            return null == currPanel;
        }
    }
}