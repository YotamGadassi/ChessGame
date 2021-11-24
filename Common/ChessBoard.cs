using System;
using System.Collections.Generic;
using System.Windows;

namespace Common
{
    public struct BoardPosition
    {
        public Point Position;
        public BoardPosition(Point _Position)
        {
            Position = _Position;
        }
    }

    public class ChessBoard
    {
        public void Init(IList<KeyValuePair<BoardPosition, ITool>[]> InitialStateList)
        {
            ClearBoard();
             foreach (KeyValuePair<BoardPosition, ITool>[] initialState in  InitialStateList)
            {
                foreach (KeyValuePair<BoardPosition, ITool> pair in initialState)
                {
                    BoardPosition position = pair.Key;
                    ITool tool = pair.Value;

                    AddTool(position, tool);
                }
            }
        }

        public bool AddTool(BoardPosition position, ITool tool)
        {
            try
            {
                boardState.Add(position, tool);
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
                tool = boardState[position];
            }
            catch(KeyNotFoundException e)
            {
                //TODO: Log
            }

            boardState.Remove(position);
            return tool;
        }

        public bool MoveTool(BoardPosition Start, BoardPosition End, MoveState moveState)
        {
            ITool toolToMove = GetToolSafe(Start);

            ITool toolAtEndPoint = GetToolSafe(End);

            bool isMoveLogicLegal = toolToMove.IsMovingLegal(Start, End, moveState);
            if (!isMoveLogicLegal)
                return false;

            toolAtEndPoint = RemoveTool(End);
            toolToMove = RemoveTool(Start);
            AddTool(End, toolToMove);

            return true;
        }

        public void ClearBoard()
        {
            boardState.Clear();
        }

        public ITool GetToolSafe(BoardPosition position)
        {
            bool isValueExists = boardState.TryGetValue(position, out ITool tool);

            if (!isValueExists)
                return null;

            return tool;

        }

        public Dictionary<BoardPosition, ToolType> GetBoardStateCopy()
        {
            Dictionary<BoardPosition, ToolType> dict = new Dictionary<BoardPosition, ToolType>();
            foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
            {
                dict[pair.Key] = pair.Value.Type; 
            }

            return dict;
        }

        private Dictionary<BoardPosition, ITool> boardState = new Dictionary<BoardPosition, ITool>();
    }

}
