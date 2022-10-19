using Common;

namespace ChessServer3._0
{
    public class PlayerObject
    {
        public GameUnit GameUnit     { get; set; }
        public string   ConnectionId { get; }
        public string   Name         { get; }
        public Team     PlayersTeam  { get; set; }

        public PlayerObject(string  connectionId
                          , string  name)
        {
            ConnectionId = connectionId;
            Name         = name;
        }
    }
}
