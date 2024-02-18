using Board;
using Common.Chess;
using Tools;

namespace OnlineChess.Common;

public interface IChessServerApi
{
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