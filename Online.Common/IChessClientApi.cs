using Board;
using Common;

namespace OnlineChess.Common
{
    public interface IChessClientApi
    {
        public Task StartGame(GameConfig gameConfig);

        public Task EndGame(EndGameReason endGameReason);

        public Task ApplyBoardCommands(BoardCommand[] boardCommands);

        public Task AskPromote(BoardPosition positionToPromote);

        public Task UpdateTime(TeamId   teamId
                             , TimeSpan timeSpan);

        public Task UpdatePlayingTeam(TeamId teamId);

        public Task UpdateToolsAndTeams(ToolAndTeamPair[] pairs);
    }
}
