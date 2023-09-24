using Common;

namespace ChessServer3._0.ClientInterface;

public class ServerTeamWithTimer
{
    public Team     Team     { get; }
    public TimeSpan TimeLeft { get; }

    public ServerTeamWithTimer(Team      team
                              , TimeSpan timeLeft)
    {
        Team     = team;
        TimeLeft = timeLeft;
    }
}