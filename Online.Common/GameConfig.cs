using System.Text.Json.Serialization;

namespace OnlineChess.Common;

[Serializable]
public class GameConfig
{
    public TeamConfig[] TeamConfigs { get; }
    [JsonConstructor]
    public GameConfig(TeamConfig[] teamConfigs)
    {
        TeamConfigs = teamConfigs;
    }

    protected bool Equals(GameConfig other)
    {
        return TeamConfigs.SequenceEqual(other.TeamConfigs);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((GameConfig)obj);
    }

    public override int GetHashCode()
    {
        int hc = TeamConfigs.Length;
        foreach (TeamConfig val in TeamConfigs)
        {
            hc = unchecked(hc * 314159 + val.GetHashCode());
        }
        return hc;
    }
}