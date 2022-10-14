using System;
using System.Windows.Controls;

namespace Client.Command
{
    public class InviteCommand : BaseCommandHandler
    {
        ConnectionManager m_connectionManager;
        
        public InviteCommand(ConnectionManager connectionManager)
        {
            m_connectionManager = connectionManager;
        }
        
        public override bool CanExecute(object parameter)
        {
            return null != parameter;
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
