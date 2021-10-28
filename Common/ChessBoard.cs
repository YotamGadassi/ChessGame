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

    public class ChessBoard
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

        public bool MoveTool(BoardPosition Start, BoardPosition End, bool IsMoveForward)
        {
            ITool toolToMove = getToolSafe(Start);

            bool isFirstMove = CheckFirstMove(toolToMove);

            ITool toolAtEndPoint = getToolSafe(End);
            bool isKilling = toolAtEndPoint != null;
            if (isKilling)
            {
                bool isKillingFromSameTeam = toolAtEndPoint.GroupNumber == toolToMove.GroupNumber;
                if (isKillingFromSameTeam)
                    return false;
            }

            bool isMoveLogicLegal = toolToMove.IsMovingLegal(Start, End, IsMoveForward, isFirstMove, isKilling);
            if (!isMoveLogicLegal)
                return false;

            toolAtEndPoint = RemoveTool(End);
            toolToMove = RemoveTool(Start);
            AddTool(End, toolToMove);

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

        internal ITool getToolSafe(BoardPosition position)
        {
            bool isValueExists = BoardState.TryGetValue(position, out ITool tool);

            if (!isValueExists)
                return null;

            return tool;

        }

        public IList<ITool> GetToolsForGroup(int GroupNumber)
        {
            IList<ITool> list = new List<ITool>();

            foreach (ITool tool in BoardState.Values)
            {
                if (tool.GroupNumber == GroupNumber)
                    list.Add(tool);
            }

            return list;

        }

        private Dictionary<BoardPosition, ITool> BoardState = new Dictionary<BoardPosition, ITool>();
    }

    public class ChessBoardProxy
    {
        ChessBoard board;
        HashSet<ITool> avaialableTools;
        bool isMoveForward;

        public ChessBoardProxy(ChessBoard Board, ITool[] tools, bool IsMoveForward)
        {
            board = Board;
            avaialableTools = new HashSet<ITool>(tools);
            isMoveForward = IsMoveForward;
        }

        public bool MakeMove(BoardPosition start, BoardPosition end)
        {
            ITool tool = board.getToolSafe(start);
            bool isMoveAuthorized = checkAuthorization(tool);

            if (!isMoveAuthorized)
                return false;

            Vector diff = new Vector(start.Position.X - end.Position.X, start.Position.Y - end.Position.Y);
            bool isMoveInRightDirection = checkMoveDirection(diff);
            if (!isMoveInRightDirection)
                return false;

            board.MoveTool(start, end, isMoveForward);

            return true;
        }

        private bool checkAuthorization(ITool tool)
        {
            return avaialableTools.Contains(tool);
        }

        private bool checkMoveDirection(Vector diff)
        {
            if (isMoveForward && diff.Y > 0)
                return true;
            
            if (!isMoveForward && diff.Y < 0)
                return true;

            return false;
        }
    }
}
