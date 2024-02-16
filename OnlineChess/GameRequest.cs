namespace OnlineChess;

public class GameRequest
{
    public string UserName { get; }

    public GameRequest(string userName)
    {
        UserName = userName;
    }

    public override string ToString()
    {
        return $"{nameof(UserName)}: {UserName}";
    }
}