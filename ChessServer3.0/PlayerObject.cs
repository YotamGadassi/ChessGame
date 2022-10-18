using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public class PlayerObject
    {
        public GameUnit GameUnit     { get; }
        public string   ConnectionId { get; private set; }

        public Team PlayersTeam { get; set; }
    }
}
