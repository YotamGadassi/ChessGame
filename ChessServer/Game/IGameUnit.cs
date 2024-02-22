using Board;
using ChessServer.ChessPlayer;
using Common;
using Common.Chess;
using OnlineChess.Common;
using Tools;

namespace ChessServer.Game
{
    public interface IGameUnit
    {
        public GameId Id { get; }

        public TeamId CurrentTeamId { get; }

        public PromotionResult Promote(BoardPosition position
                                                , ITool         tool);

        public MoveResult Move(BoardPosition start
                                        , BoardPosition end);

        public void EndGame(PlayerId playerId, EndGameReason reason);
    }
}
