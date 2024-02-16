using System;

namespace Common;

public class TeamId
{
    public Guid Id { get; }

    public static TeamId NewTeamId()
    {
        return new TeamId(Guid.NewGuid());
    }

    private TeamId(Guid id)
    {
        Id = id;
    }

    protected bool Equals(TeamId other)
    {
        return Id.Equals(other.Id);
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

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}";
    }
}