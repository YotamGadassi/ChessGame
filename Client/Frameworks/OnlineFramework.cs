using Client.Command;
using ClientWebServerCommon;
using System.Collections.ObjectModel;

namespace Client.Frameworks
{
    public class OnlineFramework
    {
        private readonly string s_serverURL = "https://localhost:44340/ChessHub";
    
        public ConnectCommand m_connectCommand { get; }
        public DisconnectCommand m_disconnectCommand { get; }
        public InviteCommand m_inviteCommand { get; }
        public AcceptInvitationCommand m_acceptInvitationCommand { get; }

        public ObservableCollection<User> m_usersList { get; }
        public ObservableCollection<User> m_invitationsList { get; }

        private User m_user;
        public ConnectionManager m_connectionManager { get; }

        public OnlineFramework()
        {
            m_connectionManager = new ConnectionManager(s_serverURL, AddUserToUsersList, RemoveUserFromUsersList, AddInvitationToInvitationsList, RemoveInvitationFromInvitationsList);
            m_connectCommand = new ConnectCommand(m_connectionManager);
            m_disconnectCommand = new DisconnectCommand(m_connectionManager);
            m_usersList = new ObservableCollection<User>();
        }

        public void AddUserToUsersList(User user)
        {
            m_usersList.Add(user);
        }

        public void RemoveUserFromUsersList(User user)
        {
            m_usersList.Remove(user);
        }

        public void AddInvitationToInvitationsList(User user)
        {
            m_invitationsList.Add(user);
        }

        public void RemoveInvitationFromInvitationsList(User user)
        {
            m_invitationsList.Remove(user);
        }
    }
}
