using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Command
{
    public class DisconnectCommand : BaseCommandHandler
    {
        private readonly ConnectionManager m_connectionManager;

        public event EventHandler CanExecuteChanged;

        public DisconnectCommand(ConnectionManager connectionManager)
        {
            m_connectionManager = connectionManager;
        }

        public override bool CanExecute(object parameter)
        {
            return m_connectionManager.State == ConnectionState.Connected;
        }

        public override void Execute(object parameter)
        {
            m_connectionManager.Disconnect();
        }
    }
}
