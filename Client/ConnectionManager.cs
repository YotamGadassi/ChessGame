using ClientWebServerCommon;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Client
{
    public enum ConnectionState
    {
        Connected,
        Disconnected
    }

    public class ConnectionManager
    {
        private HubConnection m_hubConnection;
        private string m_userName;
        private string m_serverURL;
        public ConnectionState State { get; private set; }

        private Action<User> m_addUser;
        private Action<User> m_removeUser;
        private Action<User> m_addInvitation;
        private Action<User> m_removeInvitation;

        public ConnectionManager(string serverURL, Action<User> addUser, Action<User> removeUser, Action<User> addInvitation, Action<User> removeInvitation)
        {
            State = ConnectionState.Disconnected;
            m_serverURL = serverURL;
            m_addUser = addUser;
            m_removeUser = removeUser;
            m_addInvitation = addInvitation;
            m_removeInvitation = removeInvitation;
        }

        private void setHeaders(HttpConnectionOptions options)
        {
            options.Headers.Add("UserName", m_userName);
        }

        public void Connect(User user)
        {
            m_userName = user.Name;
            m_hubConnection = new HubConnectionBuilder().WithUrl(m_serverURL, setHeaders).Build();
            m_hubConnection.Closed += onConnectionClosed;

            addAllMethodsToConnection();

            internalConnect();
        }

        private void addAllMethodsToConnection()
        {
            m_hubConnection.On<User>("Client_AddNewUser", m_addUser);
            m_hubConnection.On<User[]>("Client_AddUsersList", addUsersList);
            m_hubConnection.On<User>("Client_RemoveUser", m_removeUser);
            m_hubConnection.On<User>("Client_GetInvitation", m_addInvitation);
            m_hubConnection.On<User>("Client_CancelInvitation", m_addInvitation);
            m_hubConnection.On<User>("RemoveInvititaionFromInvitaionsList", m_removeInvitation);

        }

        private void addUsersList(User[] users)
        {
            foreach (User user in users)
            {
                m_addUser(user);
            }
        }

        public async void Disconnect()
        {
            m_userName = string.Empty;
            m_hubConnection.Closed -= onConnectionClosed;
            m_hubConnection.Remove("AddUserToUsersList");
            m_hubConnection.Remove("RemoveUserFromUsersList");
            m_hubConnection.Remove("AddInvititaionToInvitaionsList");
            m_hubConnection.Remove("RemoveInvititaionFromInvitaionsList");

            try
            {
                await m_hubConnection.StopAsync();
                State = ConnectionState.Disconnected;
            }
            catch (Exception e)
            {
                
            }
        }

        private async Task onConnectionClosed(Exception e)
        {
            State = ConnectionState.Disconnected;
            await Task.Delay(new Random().Next(0, 5) * 1000);
            internalConnect();
        }

        private async void internalConnect()
        {
            try
            {
                await m_hubConnection.StartAsync();
                State = ConnectionState.Connected;
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
