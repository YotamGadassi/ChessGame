using System.Collections.Generic;

namespace Common
{
    public interface IMovementController
    {   
        bool MoveTool(BoardPosition start, BoardPosition end);

        Dictionary<BoardPosition, ToolType> GetBoardState();
    }
}
