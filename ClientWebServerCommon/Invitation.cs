using System;

namespace ClientWebServerCommon
{
    public class Invitation
    {
        public User Host { get; set; }
        public User Guest { get; set; }
        public Guid InvitaionToken { get; set; }

        public Invitation(User host, User guest, Guid invitationToken)
        {
            Host = host;
            Guest = guest;
            InvitaionToken = invitationToken;
        }

    }
}
