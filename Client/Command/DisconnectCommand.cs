using System;

namespace Client.Command
{
    public class DisconnectCommand : BaseCommandHandler
    {
        private readonly ConnectionManager m_connectionManager;

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
