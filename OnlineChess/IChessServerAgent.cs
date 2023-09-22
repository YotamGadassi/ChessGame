using Board;
using Common.Chess;
using OnlineChess.ConnectionManager;
using Tools;

namespace OnlineChess
{
    public delegate void BoardCommandsHandler(BoardCommand[] commands);

    public delegate void StartGameHandler(OnlineChessGameConfiguration gameConfiguration);

    public delegate void EndGameHandler(EndGameReason reason);

    public delegate Task<ITool> PromotionHandler(BoardPosition positionToPromote);

    public delegate void TimeReceivedHandler(Guid teamId
                                           , TimeSpan timeLeft);

    public delegate void SwitchTeamHandler(Guid currentTeamId);

    public interface IChessServerAgent
    {
        event StartGameHandler StartGameEvent;
        event EndGameHandler EndGameEvent;
        event BoardCommandsHandler BoardCommandsEvent;
        event PromotionHandler PromotionEvent;
        event TimeReceivedHandler TimeReceivedEvent;
        event SwitchTeamHandler SwitchTeamEvent;

        Task<bool> RequestGame(string userName);

        Task WithdrawGame();

        Task<MoveResult> RequestMove(BoardPosition start
                                       , BoardPosition end);

        Task<PromotionResult> RequestPromote(BoardPosition positionToPromote
                                , IToolWrapperForServer tool);

        Task<bool> IsMyTurn();

        Task SendMessage(string msg);
    }
}