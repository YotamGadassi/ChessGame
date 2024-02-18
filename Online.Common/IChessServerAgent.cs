using Board;
using Common;
using Common.Chess;
using Tools;

namespace OnlineChess.Common
{
    public delegate void BoardCommandsHandler(BoardCommand[] commands);

    public delegate void StartGameHandler(OnlineChessGameConfiguration gameConfiguration);

    public delegate void EndGameHandler(EndGameReason reason);

    public delegate Task<ITool> PromotionHandler(BoardPosition positionToPromote);

    public delegate void UpdateTimerHandler(TeamId   teamId
                                          , TimeSpan timeLeft);

    public delegate void UpdatePlayingTeamHandler(TeamId currentTeamId);

    public interface IChessServerAgent : IChessServerApi
    {
        event StartGameHandler         StartGameEvent;
        event EndGameHandler           EndGameEvent;
        event BoardCommandsHandler     BoardCommandsEvent;
        event PromotionHandler         PromotionEvent;
        event UpdateTimerHandler       UpdateTimeEvent;
        event UpdatePlayingTeamHandler UpdatePlayingTeamEvent;
    }
}