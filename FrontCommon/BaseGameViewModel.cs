using System.Windows;
using System.Windows.Input;
using Common.GameInterfaces;

namespace FrontCommon
{
    public abstract class BaseGameViewModel : DependencyObject
    {
        private static readonly DependencyProperty gameStateProperty =
            DependencyProperty.Register("GameState", typeof(GameState), typeof(BaseGameViewModel));

        public GameState GameState
        {
            get => (GameState)GetValue(gameStateProperty);
            set => SetValue(gameStateProperty, value);
        }

        public ICommand EndGame { get; }

        public ICommand StartResume { get; }

        public ICommand Pause { get; }

        private IGameManager m_gameManager;

        protected BaseGameViewModel(IGameManager gameManager)
        {
            m_gameManager              =  gameManager;
            StartResume                =  new WpfCommand(onStartResumeExecute, onStartResumeCanExecute);
            Pause                      =  new WpfCommand(onPauseExecute,       onPauseCanExecute);
            EndGame                    =  new WpfCommand(onEndGameExecute,     onEndGameCanExecute);
            GameState                  =  gameManager.State;
            m_gameManager.StateChanged += onStateChanged;
        }

        protected virtual void onStartResumeExecute(object state)
        {
            m_gameManager.StartResumeGame();
        }

        protected virtual bool onStartResumeCanExecute(object state)
        {
            return isGameNotEnded()
                && m_gameManager.State != GameState.Running;
        }

        protected virtual void onPauseExecute(object state)
        {
            m_gameManager.PauseGame();
        }

        protected virtual bool onPauseCanExecute(object state)
        {
            return m_gameManager.State == GameState.Running;
        }

        protected virtual void onEndGameExecute(object state)
        {
            m_gameManager.EndGame();
        }

        protected virtual bool onEndGameCanExecute(object state)
        {
            return m_gameManager.State != GameState.Ended;
        }

        private void onStateChanged(object?   sender
                                  , GameState newGameState)
        {
            GameState = newGameState;
        }

        private bool isGameNotEnded()
        {
            return GameState != GameState.Ended;
        }
    }
}