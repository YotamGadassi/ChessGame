using Client.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace WebServer2
{
    public class ChessHub : Hub
    {
        private static Dictionary<Guid, UserConnectionIdPair> pendingUsers = new Dictionary<Guid, UserConnectionIdPair>();
        private static Dictionary<string, User> allUsers = new Dictionary<string, User>();

        public override async Task OnConnectedAsync()
        {
            string userName = Context.GetHttpContext().Request.Headers["Header_UserName"];

            Guid token = Guid.NewGuid();
            User newUser = new User(userName, token);
            UserConnectionIdPair userConnectionIdPair = new UserConnectionIdPair(newUser, Context.ConnectionId);

            List<User> usersToSend = createUsersToSend();

            await Clients.Caller.SendAsync("Client_AddListOfUsers", usersToSend);

            pendingUsers.Add(newUser.UserToken, userConnectionIdPair);
            allUsers.Add(Context.ConnectionId, newUser);
        
            await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_AddOneUser", newUser);

            await base.OnConnectedAsync();
        }

        private List<User> createUsersToSend()
        {
            List<User> useresToSend = new List<User>();
            foreach(var userConnectionIdPair in pendingUsers.Values)
            {
                useresToSend.Add(userConnectionIdPair.User);
            }

            return useresToSend;
        }

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    //string connectionId = Context.ConnectionId;

        //    //User user = allUsers[connectionId];
        //    //pendingUsers.Remove(user.UserToken);
        //    //allUsers.Remove(connectionId);

        //    //await Clients.AllExcept(Context.ConnectionId).SendAsync("Client_RemoveUserFromUsersList", user);
        //    await base.OnDisconnectedAsync(exception);
        //}

        private class UserConnectionIdPair
        {
            public string ConnectionId { get; }
            public User User { get; }

            public UserConnectionIdPair(User user, string connectionId)
            {
                ConnectionId = connectionId;
                User = user;
            }
        }

    }
}
