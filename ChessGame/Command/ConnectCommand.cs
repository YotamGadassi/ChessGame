using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Command
{
    public class ConnectCommand : BaseCommandHandler
    {
        private ConnectionManager m_connectionManager;
    
        public ConnectCommand(ConnectionManager connectionManager)
        {
            m_connectionManager = connectionManager;
        }

        public override bool CanExecute(object parameter)
        {
            if(m_connectionManager.State == ConnectionState.Disconnected)
            {
                return true;
            }

            return false;
        }

        public override void Execute(object parameter)
        {
            string userName = (string)parameter;

            User user = new User(userName);

            m_connectionManager.Connect(user);
        }
    }
}
