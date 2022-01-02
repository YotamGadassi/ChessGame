using Client.Command;
using Client.Models;
using System.Collections.ObjectModel;

namespace Client.Frameworks
{
    public class OnlineFramework
    {
        private readonly string s_serverURL = "";
    
        public OnlineModeControl OnlineModeControl { get; }

        private ConnectCommand m_connectCommand;
        private DisconnectCommand m_disconnectCommand;
        private InviteCommand m_inviteCommand;
        private AcceptInvitationCommand m_acceptInvitationCommand;

        private ObservableCollection<User> m_usersList;
        private ObservableCollection<User> m_invitationsList;

        private User m_user;
        private ConnectionManager m_connectionManager;

        public OnlineFramework()
        {
            m_connectionManager = new ConnectionManager(s_serverURL, AddUserToUsersList, RemoveUserFromUsersList, AddInvitationToInvitationsList, RemoveInvitationFromInvitationsList);
            OnlineModeControl = new OnlineModeControl();
            OnlineModeControl.DataContext = this;
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
