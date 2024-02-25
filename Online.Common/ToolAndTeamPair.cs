using Common;
using Tools;

namespace OnlineChess.Common;

public class ToolAndTeamPair
{
    public ToolId ToolId { get; }

    public TeamId TeamId { get; }

    public ToolAndTeamPair(ToolId  toolId
                          , TeamId teamId)
    {
        ToolId = toolId;
        TeamId = teamId;
    }

    protected bool Equals(ToolAndTeamPair other)
    {
        return ToolId.Equals(other.ToolId) && TeamId.Equals(other.TeamId);
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

        return Equals((ToolAndTeamPair)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ToolId, TeamId);
    }

    public override string ToString()
    {
        return $"{nameof(ToolId)}: {ToolId}, {nameof(TeamId)}: {TeamId}";
    }
}