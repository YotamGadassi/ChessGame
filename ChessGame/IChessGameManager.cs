using Board;
using Common;

namespace ChessGame;

public interface IChessGameManager
{
    IGameEvents       GameEvents   { get; }
    IBoardEvents      BoardEvents  { get; }
    IChessTeamManager TeamsManager { get; }
    IBoardQuery       BoardQuery   { get; }

}