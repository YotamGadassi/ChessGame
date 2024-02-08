using Board;
using Common.Chess;
using OnlineChess.ConnectionManager;
using Tools;

namespace OnlineChess
{
    public delegate void StartGameHandler(OnlineChessGameConfiguration gameConfiguration);

    public delegate void EndGameHandler(OnlineEndGameReason reason);

    public delegate void BoardCommandsHandler(BoardCommand[] commands);

    public delegate Task<ITool> PromotionHandler(BoardPosition positionToPromote);

    public delegate void TimeReceivedHandler(Guid teamId
                                           , TimeSpan timeLeft);

    public delegate void SwitchTeamHandler(Guid currentTeamId);

    public interface IChessServerAPI
    {
        Task<bool> SubmitGameRequest(string userName);

        Task WithdrawGame();

        Task<MoveResult> MoveTool(BoardPosition start
                                   , BoardPosition end);

        Task<PromotionResult> PromoteTool(BoardPosition         positionToPromote
                                           , IToolWrapperForServer tool);

        Task<bool> IsMyTurn();

        Task SendMessage(string msg);
    }

    public interface IChessServerAgent : IChessServerAPI
    {
        event StartGameHandler StartGameEvent;
        event EndGameHandler EndGameEvent;
        event BoardCommandsHandler BoardCommandsEvent;
        event PromotionHandler PromotionEvent;
        event TimeReceivedHandler TimeReceivedEvent;
        event SwitchTeamHandler SwitchTeamEvent;
    }
}