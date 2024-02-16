namespace Utils;

public abstract class BaseId
{
    private readonly Guid m_id;
    protected internal BaseId(Guid id)
    {
        m_id = id;
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

    protected bool Equals(BaseId other)
    {
        return m_id.Equals(other.m_id);
    }
}