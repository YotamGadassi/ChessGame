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
        private static Dictionary<string, User> pendingUsers = new Dictionary<string, User>();
        
        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();

            string userName = httpContext.Request.Headers["UserName"];
            Guid newToken = Guid.NewGuid();
            User newUser = new User(userName, newToken);

            User[] usersCollection = pendingUsers.Values.ToArray();
            await Clients.Caller.SendAsync("Client_AddUsersList", usersCollection);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_AddNewUser", newUser);

            pendingUsers.Add(Context.ConnectionId, newUser);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            User disconnectedUser = pendingUsers[Context.ConnectionId];
            pendingUsers.Remove(Context.ConnectionId);

            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_RemoveUser", disconnectedUser);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
