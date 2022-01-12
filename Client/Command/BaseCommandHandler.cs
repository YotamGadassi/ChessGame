using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Command
{
    public abstract class BaseCommandHandler : ICommand
    {
        private EventHandler m_canExecuteHandler;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                m_canExecuteHandler += value;
                CommandManager.RequerySuggested += value;
            }
            remove 
            {
                CommandManager.RequerySuggested -= value;
                m_canExecuteHandler -= value;
            }
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}
