using Board;
using Common;
using Common.Chess;
using OnlineChess.ConnectionManager;
using Tools;

namespace OnlineChess
{
    public delegate void BoardCommandsHandler(BoardCommand[] commands);

    public delegate void StartGameHandler(OnlineChessGameConfiguration gameConfiguration);

    public delegate void EndGameHandler(EndGameReason reason);

    public delegate Task<ITool> PromotionHandler(BoardPosition positionToPromote);

    public delegate void UpdateTimerHandler(TeamId   teamId
                                          , TimeSpan timeLeft);

    public delegate void UpdatePlayingTeamHandler(TeamId currentTeamId);

    public interface IChessServerAgent
    {
        event StartGameHandler         StartGameEvent;
        event EndGameHandler           EndGameEvent;
        event BoardCommandsHandler     BoardCommandsEvent;
        event PromotionHandler         PromotionEvent;
        event UpdateTimerHandler       UpdateTimeEvent;
        event UpdatePlayingTeamHandler UpdatePlayingTeamEvent;

        Task<GameRequestResult> SubmitGameRequest(GameRequest gameRequest);

        Task CancelGameRequest(GameRequestId gameRequestId);

        Task SubmitGameWithdraw();

        Task<MoveResult> SubmitMove(BoardPosition start
                                  , BoardPosition end);

        Task<PromotionResult> SubmitPromote(BoardPosition         positionToPromote
                                          , IToolWrapperForServer tool);

        Task<bool> IsMyTurn();

        Task SendMessage(string msg);
    }
}