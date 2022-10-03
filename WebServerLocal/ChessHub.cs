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
        private static Dictionary<Guid, User> s_pendingUsers = new Dictionary<Guid, User>();
        private static Dictionary <Guid, string> s_connectionIds = new Dictionary<Guid, string>();
        private static Dictionary<Guid, Invitation> s_invitations = new Dictionary<Guid, Invitation>();

        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();

            string userName = httpContext.Request.Headers["UserName"];
            string publicTokenString = httpContext.Request.Headers["publicToken"];

            Guid privateToken = Guid.NewGuid();
            Guid publicToken = new Guid(publicTokenString);
            User newUser = new User(userName, publicToken);

            User[] usersCollection = s_pendingUsers.Values.ToArray();
            await Clients.Caller.SendAsync("Client_AddUsersList", usersCollection);
            await Clients.Caller.SendAsync("Client_SetPrivateToken", privateToken);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_AddNewUser", newUser);

            s_pendingUsers.Add(privateToken, newUser);
            s_connectionIds.Add(privateToken, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            bool isGuidExists = s_connectionIds.TryGetValue(Context.ConnectionId, out Guid privateToken);
            if (!isGuidExists)
            {
                return;
            }

            User disconnectedUser = s_pendingUsers[privateToken];

            s_connectionIds.Remove(privateToken);
            s_pendingUsers.Remove(privateToken);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_RemoveUser", disconnectedUser);
            await base.OnDisconnectedAsync(exception);
        }

        public Guid Server_CreateInvitation(User guest)
        {
            KeyValuePair<Guid, User> guidUserPair = s_pendingUsers.First((pair => pair.Value.Equals(guest)));

            Guid guestPrivateToken = guidUserPair.Key;

            string guestDonnectionId = s_connectionIds[guestPrivateToken];
            string hostConnectionId = Context.ConnectionId;

            Guid invitationToken = createNewInvitation(hostConnectionId, guestDonnectionId);

            Clients.Client(guestDonnectionId).SendAsync("Client_AddInvitation", invitationToken);

            return invitationToken;
        }

        public bool Server_AcceptInvitation(Guid invitationToken)
        {
            bool isHostStillInviting = s_invitations.TryGetValue(invitationToken, out Invitation invitation);
            if (!isHostStillInviting)
            {

            }
        }

        private Guid createNewInvitation(string hostConnectionId, string guestDonnectionId)
        {
            Guid newToken = Guid.NewGuid();
            Invitation newInvitation = new Invitation(hostConnectionId, guestDonnectionId, newToken);
            s_invitations.Add(newToken, newInvitation);
            return newToken;
        }

        public async Task Server_RemoveInvitation(Guid InvitationToken)
        {

        }
    }
}
