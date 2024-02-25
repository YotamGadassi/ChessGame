using Board;
using Common;
using Common.Chess;
using Tools;

namespace OnlineChess.Common;

public interface IChessServerApi
{
    Task<GameRequestResult> SubmitGameRequest(GameRequest gameRequest);

    Task CancelGameRequest(GameRequestId gameRequestId);

    Task SubmitGameWithdraw();

    Task Init();

    Task<MoveResult> SubmitMove(BoardPosition start
                              , BoardPosition end);

    Task<PromotionResult> SubmitPromote(BoardPosition         positionToPromote
                                      , ITool tool);

    Task<TeamId> GetCurrentTeamTurn();

    Task SendMessage(string msg);
}