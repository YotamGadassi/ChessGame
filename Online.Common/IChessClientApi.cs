using Board;
using Common;
using Tools;

namespace OnlineChess.Common
{
    public interface IChessClientApi
    {
        public void StartGame(GameConfig gameConfig);

        public void EndGame(EndGameReason endGameReason);

        public void BoardCommands(BoardCommand[] boardCommands);

        public Task<ITool> AskPromote(BoardPosition positionToPromote);

        public void UpdateTime(TeamId   teamId
                             , TimeSpan timeSpan);

        public void UpdatePlayingTeam(TeamId teamId);

        public void UpdateToolsAndTeams(ToolAndTeamPair[] pairs);
    }
}
