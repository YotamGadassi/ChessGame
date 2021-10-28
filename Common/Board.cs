using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common
{
    public class BoardPosition
    {
        public Point Position;
        public BoardPosition(Point _Position)
        {
            Position = _Position;
        }
    }

    public class Board
    {
        public void Init(KeyValuePair<BoardPosition, ITool>[] InitialState)
        {
            ClearBoard();

            foreach (KeyValuePair<BoardPosition, ITool> pair in InitialState)
            {
                BoardPosition position = pair.Key;
                ITool tool = pair.Value;

                AddTool(position, tool);
            }
        }

        public bool AddTool(BoardPosition position, ITool tool)
        {
            try
            {
                BoardState.Add(position, tool);
            }
            catch (ArgumentException KeyExistsException)
            {
                // Log this.
                return false;
            }

            return true;
        }

        public ITool RemoveTool(BoardPosition position)
        {
            ITool tool = null;

            try
            {
                tool = BoardState[position];
            }
            catch(KeyNotFoundException e)
            {
                //TODO: Log
            }

            BoardState.Remove(position);
            return tool;
        }

        public bool MoveTool(BoardPosition start, BoardPosition end)
        {
            ITool toolToMove = getToolSafe(start);
            bool isToolExists = null != toolToMove;
            if (!isToolExists)
                return false;

            bool isMoveForward = CheckMoveForward(toolToMove);
            bool isFirstMove = CheckFirstMove(toolToMove);

            ITool toolAtEndPoint = getToolSafe(end);
            bool isMovingLegal = toolToMove.IsMovingLegal(start, end, isMoveForward, isFirstMove, toolAtEndPoint);
            if (!isMovingLegal)
                return false;

            toolAtEndPoint = RemoveTool(end);
            toolToMove = RemoveTool(start);
            AddTool(end, toolToMove);

            return true;
        }

        private bool CheckFirstMove(ITool toolToMove)
        {
            return true;
        }

        private bool CheckMoveForward(ITool toolToMove)
        {
            if (toolToMove.GroupNumber == 1)
            {
                return true;
            }
            return false;
        }

        public void ClearBoard()
        {
            BoardState.Clear();
        }

        private ITool getToolSafe(BoardPosition position)
        {
            bool isValueExists = BoardState.TryGetValue(position, out ITool tool);

            if (!isValueExists)
                return null;

            return tool;

        }

        private Dictionary<BoardPosition, ITool> BoardState = new Dictionary<BoardPosition, ITool>();
    }

}
