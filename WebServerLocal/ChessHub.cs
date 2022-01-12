using ClientWebServerCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServerLocal
{
    public class ChessHub : Hub
    {
        private static Dictionary<User, string> pendingUsers = new Dictionary<User, string>();
        private static Dictionary<Guid, Invitation> invitations = new Dictionary<Guid, Invitation>();

        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();

            string userName = httpContext.Request.Headers["UserName"];
            Guid newToken = Guid.NewGuid();
            User newUser = new User(userName, newToken);

            User[] usersCollection = pendingUsers.Keys.ToArray();
            await Clients.Caller.SendAsync("Client_AddUsersList", usersCollection);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_AddNewUser", newUser);

            pendingUsers.Add(newUser, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            KeyValuePair<User, string> userConnectionIdPair = pendingUsers.Where(pair => pair.Value == Context.ConnectionId).ToArray()[0];
            User disconnectedUser = userConnectionIdPair.Key; 
            if (null == disconnectedUser)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }
            pendingUsers.Remove(disconnectedUser);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_RemoveUser", disconnectedUser);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<Guid> Server_Invite(User guest)
        {
            bool isGuestAvailable = pendingUsers.TryGetValue(guest, out string guestConnectionId);

            if(!isGuestAvailable)
            {
                return Guid.Empty;
            }

            User host = pendingUsers.Where(pair => pair.Value == Context.ConnectionId).ToArray()?[0].Key;
            if (null == host)
            {
                return Guid.Empty;
            }

            Guid token = Guid.NewGuid();

            Invitation invitation = new Invitation(host, guest, token);

            invitations.Add(invitation.InvitaionToken, invitation);

           await Clients.Client(guestConnectionId).SendAsync("Client_GetInvitation", invitation);

            return invitation.InvitaionToken;
        }
    }
}
