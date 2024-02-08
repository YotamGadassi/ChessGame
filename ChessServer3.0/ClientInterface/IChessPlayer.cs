using Board;
using Common.Chess;
using OnlineChess;
using Tools;

namespace ChessServer3._0.ClientInterface;

public interface IChessPlayer
{
    ChessTeam Team { get; }

    Task<bool> StartGame(ServerChessGameConfiguration ganeConfiguration);

    Task EndGame(OnlineEndGameReason  reason);

    Task<ITool> Promote(BoardPosition position);

    Task UpdateTime(Guid     teamId
                  , TimeSpan timeLeft);

    Task SwitchTeam(Guid currentTeamId);

    Task UpdateBoard(BoardCommand[] chessBoard);
}