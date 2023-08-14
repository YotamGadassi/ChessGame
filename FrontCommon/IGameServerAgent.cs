using Board;
using Common;
using Common.Chess;
using Tools;

namespace FrontCommon
{
    public delegate void MoveRequestHandler(BoardPosition start
                                          , BoardPosition end);

    public delegate void EndGameRequestHandler();

    public delegate void StartGameRequestHandler(Team localTeam
                                               , Team remoteTeam
                                               , Guid gameToken);

    public delegate void PromotionEventHandler(BoardPosition         positionToPromote
                                             , IToolWrapperForServer toolToPromote);

    public delegate void BoardReceivedHandler(BoardState boardState);

    public delegate void TimeReceivedHandler(Team     team
                                           , TimeSpan timeLeft);

    public interface IGameServerAgent
    {
        event StartGameRequestHandler StartGameRequestEvent;
        event EndGameRequestHandler   EndtGameRequestEvent;
        event MoveRequestHandler      MoveRequestEvent;
        event PromotionEventHandler   PromotionEvent;
        event BoardReceivedHandler    BoardReceivedEvent;
        event TimeReceivedHandler     TimeReceivedEvent;
        event MoveRequestHandler      CheckmateEvent;

        Task<bool> RequestGame();

        Task SendEndGame();

        Task<MoveResult> SendMoveRequest(BoardPosition start
                                       , BoardPosition end
                                       , Guid          gameVersion);

        Task<bool> PromoteRequest(BoardPosition         positionToPromote
                                , IToolWrapperForServer tool
                                , Guid                  gameVersion);

        Task<bool> IsMyTurn();

        Task SendMessage(string msg);
    }
}