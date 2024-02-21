using System.Windows.Media;
using Common;

namespace OnlineChess.Common;

[Serializable]
public class TeamConfig
{
    public bool IsFirst { get; }

    public bool          IsLocal       { get; }
    public GameDirection MoveDirection { get; }

    public string Name { get; }

    public Color Color { get; }

    public TeamId Id { get; }

    public TimeSpan TotalGameTime { get; }

    public TeamConfig(bool          isFirst
                    , bool          isLocal
                    , GameDirection moveDirection
                    , string        name
                    , Color         color
                    , TeamId        id
                    , TimeSpan      totalGameTime)
    {
        IsFirst       = isFirst;
        IsLocal       = isLocal;
        MoveDirection = moveDirection;
        Name          = name;
        Color         = color;
        Id            = id;
        TotalGameTime = totalGameTime;
    }

    protected bool Equals(TeamConfig other)
    {
        return IsFirst == other.IsFirst && IsLocal == other.IsLocal  && MoveDirection == other.MoveDirection &&
               Name    == other.Name    && Color.Equals(other.Color) && Id.Equals(other.Id)                  &&
               TotalGameTime.Equals(other.TotalGameTime);
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

        return Equals((TeamConfig)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsFirst, IsLocal, (int)MoveDirection, Name, Color, Id, TotalGameTime);
    }

    public override string ToString()
    {
        return $"{nameof(IsFirst)}: {IsFirst}, {nameof(IsLocal)}: {IsLocal}, {nameof(MoveDirection)}: {MoveDirection}, {nameof(Name)}: {Name}, {nameof(Color)}: {Color}, {nameof(Id)}: {Id}, {nameof(TotalGameTime)}: {TotalGameTime}";
    }
}