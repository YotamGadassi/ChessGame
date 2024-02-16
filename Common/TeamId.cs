using System;
using Utils;

namespace Common;

public class TeamId : BaseId
{
    public static TeamId NewTeamId()
    {
        return new TeamId(Guid.NewGuid());
    }

    private TeamId(Guid id) : base(id)
    {
    }
    
    protected bool Equals(TeamId other)
    {
        return base.Equals(other);
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

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((TeamId)obj);
    }
}