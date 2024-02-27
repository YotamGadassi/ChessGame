using System;
using Board;

namespace Common;

public class CheckMateData
{
    public BoardPosition Position { get; }

    public TeamId WinningTeam { get; }

    public CheckMateData(BoardPosition position
                        , TeamId       winningTeam)
    {
        Position    = position;
        WinningTeam = winningTeam;
    }

    protected bool Equals(CheckMateData other)
    {
        return Position.Equals(other.Position) && WinningTeam.Equals(other.WinningTeam);
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

        return Equals((CheckMateData)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, WinningTeam);
    }

    public override string ToString()
    {
        return $"{nameof(Position)}: {Position}, {nameof(WinningTeam)}: {WinningTeam}";
    }
}