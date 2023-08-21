using System.Windows.Input;

namespace FrontCommon
{
    public class WpfCommand : ICommand
    {
        private readonly Func<object, bool>? m_canExecute;
        private readonly Action<object>     m_execute;

        public WpfCommand(Action<object> executeHandler, Func<object, bool>? canExecuteHandler)
        {
            m_canExecute = canExecuteHandler;
            m_execute    = executeHandler;
        }

        public WpfCommand( Action<object> executeHandler) : this(executeHandler, null){}

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return m_canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            m_execute?.Invoke(parameter);
        }
    }
}
