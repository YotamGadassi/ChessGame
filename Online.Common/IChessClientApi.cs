﻿using Board;
using Common;
using Tools;

namespace OnlineChess.Common
{
    public interface IChessClientApi
    {
        public Task StartGame(GameConfig gameConfig);

        public Task EndGame(EndGameReason endGameReason);

        public Task ApplyBoardCommands(BoardCommand[] boardCommands);

        public Task AskPromote(BoardPosition positionToPromote, ITool toolToPromote);

        public Task UpdateTime(TeamId   teamId
                             , TimeSpan timeSpan);

        public Task UpdatePlayingTeam(TeamId teamId);

        public Task UpdateToolsAndTeams(ToolAndTeamPair[] pairs);
    }
}
