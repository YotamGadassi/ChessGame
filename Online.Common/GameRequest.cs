using Utils;

namespace OnlineChess.Common;

public class GameRequest
{
    public string UserName { get; }

    public GameRequest(string userName)
    {
        UserName = userName;
    }

    public override string ToString()
    {
        return $"{nameof(UserName)}: {UserName}";
    }
}

public class GameRequestId : BaseId
{
    public static GameRequestId NewGameRequestId()
    {
        return new GameRequestId(Guid.NewGuid());
    }

    private GameRequestId(Guid id) : base(id) { }

    protected bool Equals(GameRequestId other)
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

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((GameRequestId)obj);
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

    public override string ToString()
    {
        return $"{nameof(IsError)}: {IsError}, {nameof(GameRequestId)}: {GameRequestId}";
    }
}