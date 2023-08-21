using System.Windows;
using System.Windows.Input;
using Common;

namespace FrontCommon
{
    public abstract class BaseGameViewModel : DependencyObject
    {
        private static readonly DependencyProperty gameStateProperty = DependencyProperty.Register("GameState", typeof(GameState), typeof(BaseGameViewModel));

        public GameState GameState
        {
            get => (GameState)GetValue(gameStateProperty);
            set => SetValue(gameStateProperty, value);
        }

        public ICommand Play { get; }

        public ICommand Stop { get; }

        public ICommand Pause { get; }

        private IGameManager m_gameManager;

        protected BaseGameViewModel(IGameManager gameManager)
        {
            m_gameManager              =  gameManager;
            Play                       =  new WpfCommand(onPlayExecute,  onPlayCanExecute);
            Pause                      =  new WpfCommand(onPauseExecute, onPauseCanExecute);
            Stop                       =  new WpfCommand(onStopExecute,  onStopCanExecute);
            m_gameManager.StateChanged += onStateChanged;
        }

        protected virtual void onPlayExecute(object state)
        {
            m_gameManager.StartGame();
        }

        protected virtual bool onPlayCanExecute(object state)
        {
            return m_gameManager.State != GameState.Play;
        }

        protected virtual void onPauseExecute(object state)
        {
            m_gameManager.PauseGame();
        }

        protected virtual bool onPauseCanExecute(object state)
        {
            return m_gameManager.State != GameState.Pause && m_gameManager.State != GameState.Stop;
        }

        protected virtual void onStopExecute(object state)
        {
            m_gameManager.EndGame();
        }

        protected virtual bool onStopCanExecute(object state)
        {
            return m_gameManager.State != GameState.Stop;
        }
        
        private void onStateChanged(object?   sender
                                  , GameState e)
        {
            GameState = e;
        }
    }
}