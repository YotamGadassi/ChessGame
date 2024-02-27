using Common;

namespace OnlineChess.Common
{
    public interface IChessClientApi
    {
        public Task StartGame(GameConfig gameConfig);

        public Task EndGame(EndGameReason endGameReason);

        public Task CheckMate(CheckMateData           checkMateData);
        public Task ApplyBoardCommands(BoardCommand[] boardCommands);

        public Task AskPromote(PromotionRequest promotionRequest);

        public Task UpdateTime(TeamId   teamId
                             , TimeSpan timeSpan);

        public Task UpdatePlayingTeam(TeamId teamId);

        public Task UpdateToolsAndTeams(ToolAndTeamPair[] pairs);
    }
}
