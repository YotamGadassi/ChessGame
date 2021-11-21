using Common;

namespace Server
{
    public class MovementController : IMovementController
    {
        private IPlayer player;
        private ChessBoard board;
        private ChessGameServer server;

        public MovementController(IPlayer player_, ChessBoard board_, ChessGameServer server_)
        {
            player = player_;
            board = board_;
            server = server_;
        }

        public bool MoveTool(BoardPosition start, BoardPosition end)
        {
            GameStatus gameStatus = server.CurrentStatus;
            bool isGameActive = gameStatus != GameStatus.Active;
            if (!isGameActive)
                return false;
            
            ITool toolToMove = board.GetToolSafe(start);
            bool isStartPositionContainTool = toolToMove != null
            if (!isStartPositionContainTool)
                return false;

            Team toolToMoveTeam = server.toolToTeam[toolToMove];
            bool isToolBelongsToPlayer = toolToMoveTeam == server.playerToTeam.GetValue(player);
            if (!isToolBelongsToPlayer)
                return false;

            ITool toolAtEndPosition = board.GetToolSafe(end);
            Team toolAtEndPositionTeam = server.toolToTeam[toolAtEndPosition];

            bool isEndPositionValid = toolAtEndPosition == null || toolAtEndPositionTeam != toolToMoveTeam;
            if (!isEndPositionValid)
                return false;

            bool isKillingRivalTeam = toolAtEndPosition != null;
            GameDirection direction = server.TeamToGameDirection.GetValue(toolToMoveTeam);
            MoveState moveState = new MoveState(direction, toolToMove.IsToolFirstMove, isKillingRivalTeam);
            
            return board.MoveTool(start, end, moveState);
        }
    
    
    }
}
