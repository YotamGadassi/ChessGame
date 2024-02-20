namespace OnlineChess.Common;

[Serializable]
public class GameConfig
{
    public TeamConfig[] TeamConfigs { get; }

    public GameConfig(TeamConfig[] teamConfigs)
    {
        TeamConfigs = teamConfigs;
    }
}