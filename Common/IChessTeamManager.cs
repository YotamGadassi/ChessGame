using Common.Chess;

namespace Common;

public interface IChessTeamManager : ITeamsManager
{
    public ITeamTimer GetTeamTimer(TeamId teamId);
}