using System.Reflection;
using Common;
using Common.ChessBoardEventArgs;
using log4net;

namespace Board
{
    public class ChessBoard
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private BoardState     m_board;
        private GameMoveHelper m_gameMoveHelper;

        public ChessBoard()
        {
            m_board          = new BoardState();
            m_gameMoveHelper = new GameMoveHelper(this);
        }

        /// <summary>
        /// Adds a tool to the chess board
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If position is out of board boundaries</exception>
        /// <exception cref="ArgumentException">board already contains tool, of if position is occupied</exception>
        /// <param name="position">The position to which the tool should be added</param>
        /// <param name="tool">The tool to add</param>
        public void Add(BoardPosition position, ITool tool)
        {
            if (false == GameMoveHelper.ValidatePositionOnBoard(position))
            {
                throw new ArgumentOutOfRangeException($"The position {position} is out of range!");
            }
            m_board.Add(position, tool);
        }

        public bool Remove(BoardPosition position)
        {
            return m_board.Remove(position);
        }

        /// <summary>
        /// Moves a tool from start to end.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">if position is out of board boundaries</exception>
        /// <param name="start">The position from which to move a tool</param>
        /// <param name="end">The position to move a tool to it</param>
        /// <returns>true if tool has moved, O.W. false</returns>
        public MoveResult Move(BoardPosition start, BoardPosition end)
        {
            if (false == (GameMoveHelper.ValidatePositionOnBoard(start) && GameMoveHelper.ValidatePositionOnBoard(end)))
            {
                throw new
                    ArgumentOutOfRangeException($"The start or end position are not valid: Start:{start}, End:{end}");
            }

            if (false == m_gameMoveHelper.IsMoveLegal(start, end))
            {
                s_log.Info($"Move from {start} to {end} is not legal!");
                return MoveResult.NoChangeOccurredResult;
            }

            bool isThereToolToMove = m_board.TryGetTool(start, out ITool toolToMove);
            if (false == isThereToolToMove)
            {
                s_log.Error($"Position {start} doesn't contain any tool. So no move occurred");
                return MoveResult.NoChangeOccurredResult;
            }

            bool isThereToolToKill = m_board.TryGetTool(end, out ITool toolOnEndPosition);
            if (isThereToolToKill)
            {
                if (isOnSameTeam(toolToMove, toolOnEndPosition))
                {
                    s_log.Info($"Cannot move tool {toolToMove} from {start} to {end}, because tool {toolOnEndPosition} is on the same team and is at end position");
                    return MoveResult.NoChangeOccurredResult;
                }

                m_board.Remove(end);
                m_board.Remove(start);
                m_board.Add(end, toolToMove);

                s_log.Info($"Killing event has occurred: tool at start: {toolToMove}, start: {start}, end: {end}, tool at end: {toolOnEndPosition}");

                // KillingEventArgs eventArgs = new KillingEventArgs(toolToMove, start, end, toolOnEndPosition);
                // KillingEvent?.Invoke(this, eventArgs);
                return new MoveResult(MoveResultEnum.ToolKilled, start, end, toolToMove, toolOnEndPosition);
            }

            m_board.Remove(start);
            m_board.Add(end, toolToMove);

            s_log.Info($"Tool Moved Event: tool:{toolToMove}, start:{start}, end:{end}");
            // ToolMovedEventArgs evengArgs = new ToolMovedEventArgs(toolToMove, start, end);
            // ToolMovedEvent?.Invoke(this, evengArgs);

            return new MoveResult(MoveResultEnum.ToolMoved, start, end, toolToMove, toolOnEndPosition);
        }

        private bool isOnSameTeam(ITool toolA, ITool toolB)
        {
            return toolA.Color.Equals(toolB.Color);
        }

        public bool TryGetTool(BoardPosition position, out ITool tool)
        {
            return m_board.TryGetTool(position, out tool);
        }

        public bool TryGetPosition(ITool tool, out BoardPosition position)
        {
            return m_board.TryGetPosition(tool, out position);
        }

        public void ClearBoard()
        {
            m_board.Clear();
        }
    }
}