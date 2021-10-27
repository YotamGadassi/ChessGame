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

        public bool MoveTool(BoardPosition start, BoardPosition end, bool OverrideEndPosition = true)
        {
            ITool toolToMove = getToolSafe(start);
            bool isToolExists = null != toolToMove;
            if (!isToolExists)
                return false;

            ITool toolToRemove;
            if (!OverrideEndPosition)
            {
                toolToRemove = getToolSafe(end);
                bool isEndPositionEmpty = toolToRemove == null;
                if (!isEndPositionEmpty)
                    return false;
            }

            bool isMovingLegal = toolToMove.IsMovingLegal(start, end);
            if (!isMovingLegal)
                return false;

            toolToRemove = RemoveTool(end);
            toolToMove = RemoveTool(start);
            AddTool(end, toolToMove);

            return true;
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
