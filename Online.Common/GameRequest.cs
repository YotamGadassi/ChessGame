namespace OnlineChess.Common;

public class GameRequest
{
    public string UserName { get; }

    public GameRequest(string userName)
    {
        UserName = userName;
    }

    protected bool Equals(GameRequest other)
    {
        return UserName == other.UserName;
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

        return Equals((GameRequest)obj);
    }

    public override int GetHashCode()
    {
        return UserName.GetHashCode();
    }

    public override string ToString()
    {
        return $"{nameof(UserName)}: {UserName}";
    }
}

public class GameRequestResult
{
    public bool          IsError       { get; }
    public GameRequestId GameRequestId { get; }

    public GameRequestResult(bool           isError
                            , GameRequestId gameRequestId)
    {
        IsError       = isError;
        GameRequestId = gameRequestId;
    }

    protected bool Equals(GameRequestResult other)
    {
        return IsError == other.IsError && GameRequestId.Equals(other.GameRequestId);
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

        return Equals((GameRequestResult)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsError, GameRequestId);
    }

    public override string ToString()
    {
        return $"{nameof(IsError)}: {IsError}, {nameof(GameRequestId)}: {GameRequestId}";
    }
}