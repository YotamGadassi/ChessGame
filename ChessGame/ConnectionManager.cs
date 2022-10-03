using Client.Models;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public enum ConnectionState
    {
        Connected,
        Disconnected,
        Idle
    }

    public class ConnectionManager
    {
        private HubConnection m_hubConnection;
        private User m_user;
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
            options.Headers.Add("Header_UserName", m_user.Name);
        }

        public void Connect(User user)
        {
            State = ConnectionState.Idle;
            m_user = user;
            m_hubConnection = new HubConnectionBuilder().WithUrl(m_serverURL, setHeaders).Build();

            // m_hubConnection.Closed += onConnectionClosed;

            addAllMethodsToConnection();

            internalConnect(user);
        }

        private void addAllMethodsToConnection()
        {
            m_hubConnection.On<User>("Client_AddOneUser", (u) => 
                                                                { 
                                                                    m_addUser.Invoke(u); 
                                                                });

            m_hubConnection.On<List<User>>("Client_AddListOfUsers", (usersList) =>
            {
                foreach(User u in usersList)
                {
                    m_addUser.Invoke(u);
                }
            });
            m_hubConnection.On<User>("Client_RemoveUserFromUsersList", m_removeUser);
            m_hubConnection.On<User>("Client_AddInvititaionToInvitaionsList", m_addInvitation);
            m_hubConnection.On<User>("Client_RemoveInvititaionFromInvitaionsList", m_removeInvitation);

        }

        public async void Disconnect()
        {
            State = ConnectionState.Idle;
            // m_hubConnection.Closed -= onConnectionClosed;
            m_hubConnection.Remove("Client_AddOneUser");
            m_hubConnection.Remove("Client_AddListOfUsers");
            m_hubConnection.Remove("Client_RemoveUserFromUsersList");
            m_hubConnection.Remove("Client_AddInvititaionToInvitaionsList");
            m_hubConnection.Remove("Client_RemoveInvititaionFromInvitaionsList");

            //try
            //{
                Console.WriteLine("before connection");
                await m_hubConnection.StopAsync();
                Console.WriteLine("after connection");

                State = ConnectionState.Disconnected;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("exception detalis:{0}", e.Message);
            //}
        }

        private async Task onConnectionClosed(Exception e)
        {
            State = ConnectionState.Disconnected;
            await Task.Delay(new Random().Next(0, 5) * 1000);
            internalConnect(m_user);
        }

        private async void internalConnect(User user)
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
