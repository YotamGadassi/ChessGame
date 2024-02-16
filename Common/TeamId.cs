using System;

namespace Common;

public abstract class BaseId
{
    protected BaseId(Guid id)
    {
        m_id = id;
    }

    private readonly Guid m_id;

    protected bool Equals(BaseId other)
    {
        return m_id.Equals(other.m_id);
    }

    public abstract override bool Equals(object? obj);

    public override int GetHashCode()
    {
        return m_id.GetHashCode();
    }

    public override string ToString()
    {
        return $"Id: {m_id}";
    }
}

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