using Client.Command;
using ClientWebServerCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        private MainClientWindows m_win;

        public OnlineFramework(MainClientWindows win)
        {
            m_usersList = new ObservableCollection<User>();
            m_connectionManager = new ConnectionManager(s_serverURL, AddUserToUsersList, RemoveUserFromUsersList, AddInvitationToInvitationsList, RemoveInvitationFromInvitationsList);
            m_connectCommand = new ConnectCommand(m_connectionManager);
            m_disconnectCommand = new DisconnectCommand(m_connectionManager);
            m_inviteCommand = new InviteCommand();
            m_win = win;

            registerToEvents();
        }

        private void registerToEvents()
        {
            m_connectionManager.DisconnectionEvent += disconnectionHandler;
        }

        private void disconnectionHandler(object sender, EventArgs args)
        {
            m_usersList?.Clear();
        }

        public void AddUserToUsersList(User user)
        {
            m_win.Dispatcher.Invoke(() =>m_usersList.Add(user));
        }

        public void RemoveUserFromUsersList(User user)
        {
            m_win.Dispatcher.Invoke(() => removeUser(user));
        }

        private bool removeUser(User user)
        {
            User foundUser = m_usersList.First((userInList) => userInList.Token == user.Token);
            bool isRemoved = m_usersList.Remove(foundUser);
            return isRemoved;
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
