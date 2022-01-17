using ClientWebServerCommon;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Client
{
    public enum ConnectionState
    {
        Connected,
        Disconnected
    }

    public class ConnectionManager
    {
        private Timer m_invitationCancelScheduler;
        
        private HubConnection m_hubConnection;
        private string m_userName;
        private string m_serverURL;
        public ConnectionState State { get; private set; }

        private Action<User> m_addUser;
        private Action<User> m_removeUser;
        private Action<User> m_addInvitation;
        private Action<User> m_removeInvitation;
        private Action m_disconnection;

        public event EventHandler<EventArgs> ConnectionEvent;
        public event EventHandler<EventArgs> DisconnectionEvent;

        public ConnectionManager(string serverURL, Action<User> addUser, Action<User> removeUser, Action<User> addInvitation, Action<User> removeInvitation)
        {
            State = ConnectionState.Disconnected;
            m_serverURL = serverURL;
            m_addUser = addUser;
            m_removeUser = removeUser;
            m_addInvitation = addInvitation;
            m_removeInvitation = removeInvitation;
            m_invitationCancelScheduler = new Timer();
            m_invitationCancelScheduler.AutoReset = false;
            m_invitationCancelScheduler.Elapsed += cancelInvitation;
        }

        private void cancelInvitation(object sender, ElapsedEventArgs e)
        {
            // TODO: create invitation manager
            Task.Run(() => m_hubConnection.InvokeAsync<Guid>("Server_CancelInvitaiton", guest));
        }

        public async void SendInvitation(User guest)
        {
            Task<Guid> result = Task.Run(() => m_hubConnection.InvokeAsync<Guid>("Server_SendInvitaiton", guest));
            setInvitationModeOn();
            m_invitationCancelScheduler.Enabled = true;
            m_invitationCancelScheduler.Interval = 30000;

        }

        private void setInvitationModeOn()
        {
            throw new NotImplementedException();
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

            raiseConnectionEvent();
        }

        private void raiseConnectionEvent()
        {
            EventArgs args = new EventArgs();
            ConnectionEvent?.Invoke(this, args);
        }

        private void addAllMethodsToConnection()
        {
            m_hubConnection.On<User>("Client_AddNewUser", m_addUser);
            m_hubConnection.On<User[]>("Client_AddUsersList",addUsersList);
            m_hubConnection.On<User>("Client_RemoveUser", m_removeUser);
            m_hubConnection.On<User>("Client_GetInvitation", m_addInvitation);
            m_hubConnection.On<User>("Client_CancelInvitation", m_addInvitation);
            m_hubConnection.On<User>("RemoveInvititaionFromInvitaionsList", m_removeInvitation);

        }

        private void uiInjectMethod(Action<User> method)
        {
            Application currentApp = App.Current;

            if (null == currentApp)
            {
                return;
            }

            currentApp.Dispatcher.Invoke(method);
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
            m_hubConnection.Remove("Client_AddNewUser");
            m_hubConnection.Remove("Client_RemoveUser");
            m_hubConnection.Remove("Client_GetInvitation");
            m_hubConnection.Remove("Client_CancelInvitation");
            m_hubConnection.Remove("RemoveInvititaionFromInvitaionsList");

            try
            {
                await m_hubConnection.StopAsync();
                raiseDisconnectionEvent();
                State = ConnectionState.Disconnected;
            }
            catch (Exception e)
            {

            }
        }

        private void raiseDisconnectionEvent()
        {
            EventArgs args = new EventArgs();
            DisconnectionEvent.Invoke(this, args);
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
