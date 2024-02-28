namespace ChessServer.Game;

public class GameEndedEventArgs
{
    public IGameUnit GameUnit { get; }

    public GameEndedEventArgs(IGameUnit gameUnit)
    {
        GameUnit = gameUnit;
    }
}