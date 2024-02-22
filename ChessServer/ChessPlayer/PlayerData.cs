namespace ChessServer.ChessPlayer
{
    public class PlayerData
    {
        public PlayerId PlayerId { get; }

        public string PlayerName { get; }

        public PlayerData(PlayerId playerId
                         , string  playerName)
        {
            PlayerId   = playerId;
            PlayerName = playerName;
        }

        public override string ToString()
        {
            return $"{nameof(PlayerId)}: {PlayerId}, {nameof(PlayerName)}: {PlayerName}";
        }
    }
}
