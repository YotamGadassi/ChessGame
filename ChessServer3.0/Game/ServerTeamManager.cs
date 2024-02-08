using ChessServer3._0.ClientInterface;
using Common;
using Common.Chess;

namespace ChessServer3._0.Game;

public class ServerTeamManager : OfflineTeamsManager
{
    private Dictionary<Guid, IChessPlayer> m_teamIdToPlayer;

    public ServerTeamManager(IChessPlayer[] chesssPlayerArr) : base(chesssPlayerArr.Select((chessPlayer) => chessPlayer.Team).ToArray())
    {
        ChessTeam[] chessTeams = new ChessTeam[chesssPlayerArr.Length];

        for (int i = 0; i < chesssPlayerArr.Length; i++)
        {
            ChessTeam team = chesssPlayerArr[i].Team;
            chessTeams[i]             = team;
            m_teamIdToPlayer[team.Id] = chesssPlayerArr[i];
        }

        registerToEvents();
    }

    private void registerToEvents()
    {
        foreach (ChessTeam chessTeam in m_teams)
        {
            chessTeam.TeamTimer.TimeLeftChange += onTimeLeftChange;
        }
    }

    private void onTimeLeftChange(TimeSpan obj)
    {
        
    }

    private void sendTimeLeftChangedToPlayers(TimeSpan timeLeft)
    {
        for
    }
}