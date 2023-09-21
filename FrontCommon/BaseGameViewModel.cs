using System.Windows;
using System.Windows.Input;
using Common;

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

        private IGameStateController m_gameController;

        protected BaseGameViewModel(IGameStateController gameController)
        {
            m_gameController              =  gameController;
            StartResume                   =  new WpfCommand(onStartResumeExecute, onStartResumeCanExecute);
            Pause                         =  new WpfCommand(onPauseExecute,       onPauseCanExecute);
            EndGame                       =  new WpfCommand(onEndGameExecute,     onEndGameCanExecute);
            GameState                     =  gameController.State;
            m_gameController.StateChanged += onStateChanged;
        }

        protected virtual void onStartResumeExecute(object state)
        {
            m_gameController.StartResumeGame();
        }

        protected virtual bool onStartResumeCanExecute(object state)
        {
            return isGameNotEnded()
                && m_gameController.State != GameState.Running;
        }

        protected virtual void onPauseExecute(object state)
        {
            m_gameController.PauseGame();
        }

        protected virtual bool onPauseCanExecute(object state)
        {
            return m_gameController.State == GameState.Running;
        }

        protected virtual void onEndGameExecute(object state)
        {
            m_gameController.EndGame();
        }

        protected virtual bool onEndGameCanExecute(object state)
        {
            return m_gameController.State != GameState.Ended;
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