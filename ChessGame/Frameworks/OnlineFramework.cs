using Client.Command;
using Client.Models;
using System;
using System.Collections.ObjectModel;

namespace Client.Frameworks
{
    public class OnlineFramework
    {
        public string ServerURL { get; }
    
        public ConnectCommand m_connectCommand { get; }
        public DisconnectCommand m_disconnectCommand { get; }
        public InviteCommand m_inviteCommand { get; }
        public AcceptInvitationCommand m_acceptInvitationCommand { get; }

        public ObservableCollection<User> m_usersList { get; }
        public ObservableCollection<User> m_invitationsList { get; }

        private ConnectionManager m_connectionManager;

        public OnlineFramework(string serverURL)
        {
            ServerURL = serverURL;
            m_connectionManager = new ConnectionManager(ServerURL, AddUserToUsersList, RemoveUserFromUsersList, AddInvitationToInvitationsList, RemoveInvitationFromInvitationsList);
            m_usersList = new ObservableCollection<User>();
            m_invitationsList = new ObservableCollection<User>();
            m_connectCommand = new ConnectCommand(m_connectionManager);
            m_disconnectCommand = new DisconnectCommand(m_connectionManager);
            m_inviteCommand = new InviteCommand();
            m_acceptInvitationCommand = new AcceptInvitationCommand();

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
