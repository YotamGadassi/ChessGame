using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServerLocal
{
    public class Invitation
    {
        public string HostConnectionId { get; }
        public string GuestConnectionId { get; }

        public Guid Token { get; }

        public Invitation(string hostConnectionId, string guestConnectionId, Guid token)
        {
            HostConnectionId = hostConnectionId;
            GuestConnectionId = guestConnectionId;
            Token = token;
        }
    }
}
