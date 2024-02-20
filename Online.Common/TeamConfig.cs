using System.Windows.Media;
using Common;

namespace OnlineChess.Common;

[Serializable]
public class TeamConfig
{
    public bool          IsFirst       { get; }

    public bool          IsLocal       { get; }
    public GameDirection MoveDirection { get; }

    public string Name { get; }

    public Color Color { get; }

    public TeamId Id { get; }

    public TimeSpan TotalGameTime { get; }

    public TeamConfig(bool           isFirst
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
}