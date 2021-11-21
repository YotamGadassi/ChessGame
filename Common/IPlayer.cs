namespace Common
{
    public interface IPlayer
    {
        string Name { get; }
        int Rank { get; }
        PlayerToken Token { get; }
    }
}
