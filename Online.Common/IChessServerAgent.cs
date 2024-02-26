using Common;

namespace OnlineChess.Common
{
    public delegate void BoardCommandsHandler(BoardCommand[] commands);

    public delegate void StartGameHandler(GameConfig gameConfiguration);

    public delegate void EndGameHandler(EndGameReason reason);

    public delegate void UpdateTimerHandler(TeamId   teamId
                                          , TimeSpan timeLeft);

    public delegate void UpdatePlayingTeamHandler(TeamId currentTeamId);

    public delegate void UpdateToolsAndTeamsHandler(ToolAndTeamPair[] pairs);

    public interface IChessServerAgent : IChessServerApi
    {
        event StartGameHandler           StartGameEvent;
        event EndGameHandler             EndGameEvent;
        event BoardCommandsHandler       BoardCommandsEvent;
        event AskPromotionEventHandler   AskPromotionEvent;
        event UpdateTimerHandler         UpdateTimeEvent;
        event UpdatePlayingTeamHandler   UpdatePlayingTeamEvent;
        event UpdateToolsAndTeamsHandler UpdateToolsAndTeamsEvent;
    }
}