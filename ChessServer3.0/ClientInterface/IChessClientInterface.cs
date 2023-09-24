using Board;
using OnlineChess;
using Tools;

namespace ChessServer3._0.ClientInterface;

public interface IChessClientInterface
{
    Task<bool> StartGame(ServerChessGameConfiguration ganeConfiguration);

    Task EndGame(OnlineEndGameReason  reason);

    Task Promote(BoardPosition position
               , ITool         tool);

    Task UpdateTime(Guid     teamId
                  , TimeSpan timeLeft);

    Task SwitchTeam(Guid currentTeamId);

    Task UpdateBoard(BoardCommand[] chessBoard);
}