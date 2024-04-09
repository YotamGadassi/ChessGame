using System.Windows.Input;
using FrontCommon.GamePanel;

namespace FrontCommon
{
    public interface IGameButtonViewModel
    {
        ICommand ButtonCommand { get; }
        string   PanelGameName { get; }
    }

    public abstract class BaseGameButtonViewModel : IGameButtonViewModel
    {
        public          ICommand ButtonCommand { get; }
        public abstract string   PanelGameName { get; }

        public abstract string CommandName { get; }

        protected readonly IGamePanelManager PanelManager;

        protected BaseGameButtonViewModel(IGamePanelManager panelManager)
        {
            PanelManager  = panelManager;
            ButtonCommand = new WpfCommand(playCommandExecute, playCommandCanExecute);
        }

        protected virtual IGamePanel getPanel()
        {
            bool isExists = PanelManager.TryGetPanel(PanelGameName, out IGamePanel panel);
            if (false == isExists)
            {
                throw new KeyNotFoundException($"Panel: {PanelGameName} does not exist");
            }

            return panel;
        }

        protected virtual void playCommandExecute(object parameter)
        {
            IGamePanel gamePanel = getPanel();
            gamePanel.Init();
            PanelManager.Show(gamePanel);
        }

        private bool playCommandCanExecute(object parameter)
        {
            IGamePanel? currPanel = PanelManager.CurrentPanel;
            return null == currPanel;
        }
    }
}