namespace Common;

public interface IChessTeamManager : ITeamsManager
{
    public ITeamTimer GetTeamTimer(Team team);
}