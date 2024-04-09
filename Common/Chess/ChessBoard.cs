using System;
using System.Reflection;
using System.Windows.Media;
using Board;
using log4net;
using Tools;

namespace Common.Chess
{
    public class ChessBoard : IBoard, IBoardQuery, IBoardEvents
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event Action<ITool, BoardPosition>? ToolAddEvent;
        public event Action<BoardPosition>?        ToolRemoved;

        private readonly BasicBoard     m_board;
        private readonly GameMoveHelper m_gameMoveHelper;

        public ChessBoard()
        {
            m_board          = new BasicBoard();
            m_gameMoveHelper = new GameMoveHelper(this);
        }

        /// <summary>
        /// Adds a tool to the chess board
        /// </summary>
        /// <exception cref="ArgumentException">If position is out of board boundaries</exception>
        /// <exception cref="position">board already contains tool, of if position is occupied</exception>
        /// <param name="tool">The position to which the tool should be added</param>
        /// <param name="tool">The tool to add</param>
        public void Add(BoardPosition position
                      , ITool         tool)
        {
            if (false == GameMoveHelper.ValidatePositionOnBoard(position))
            {
                throw new ArgumentOutOfRangeException($"The position {position} is out of range!");
            }
            addTool(position, tool);
        }

        public bool Remove(BoardPosition position)
        {
            if (false == GameMoveHelper.ValidatePositionOnBoard(position))
            {
                throw new ArgumentOutOfRangeException($"The position {position} is out of range!");
            }
            return removeTool(position);
        }

        /// <summary>
        /// Moves a tool from start to end.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">if position is out of board boundaries</exception>
        /// <param name="start">The position from which to move a tool</param>
        /// <param name="end">The position to move a tool to it</param>
        /// <returns>true if tool has moved, O.W. false</returns>
        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            if (false == (GameMoveHelper.ValidatePositionOnBoard(start)
                       && GameMoveHelper.ValidatePositionOnBoard(end)))
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

            MoveResultEnum moveResultEnum = MoveResultEnum.ToolMoved;
            if (isPromotion(toolToMove, end))
            {
                moveResultEnum |= MoveResultEnum.NeedPromotion;
            }

            bool isThereToolToKill = m_board.TryGetTool(end, out ITool toolOnEndPosition);
            if (isThereToolToKill)
            {
                if (isOnSameTeam(toolToMove, toolOnEndPosition))
                {
                    s_log.Info($"Cannot move tool {toolToMove} from {start} to {end}, because tool {toolOnEndPosition} is on the same team and is at end position");
                    return MoveResult.NoChangeOccurredResult;
                }

                moveResultEnum |= MoveResultEnum.ToolKilled;

                if (toolOnEndPosition is King)
                {
                    moveResultEnum |= MoveResultEnum.CheckMate;
                }

                removeTool(end);
                s_log.Info($"Killing event has occurred: tool at start: {toolToMove}, start: {start}, end: {end}, tool at end: {toolOnEndPosition}");
            }
            else
            {
                s_log.Info($"Tool Moved Event: tool:{toolToMove}, start:{start}, end:{end}");
            }

            removeTool(start);
            addTool(end, toolToMove);

            return new MoveResult(moveResultEnum, start, end, toolToMove, toolOnEndPosition);
        }

        public PromotionResult Promote(BoardPosition position
                                     , ITool         newTool)
        {
            if (false == (GameMoveHelper.ValidatePositionOnBoard(position)))
            {
                throw new
                    ArgumentOutOfRangeException($"The position is not valid: Position{position}");
            }

            bool isToolExist = TryGetTool(position, out ITool toolToPromote);
            if (false == isToolExist)
            {
                return new PromotionResult(null, null, position, PromotionResultEnum.PositionIsEmpty);
            }

            bool isToolValidForPromotion = isPromotion(toolToPromote, position);
            if (false == isToolValidForPromotion)
            {
                return new PromotionResult(toolToPromote, newTool, position, PromotionResultEnum.ToolIsNotValidForPromotion);
            }

            removeTool(position);
            addTool(position, newTool);

            return new PromotionResult(toolToPromote, newTool, position, PromotionResultEnum.PromotionSucceeded);
        }

        public bool TryGetTool(BoardPosition position
                             , out ITool     tool)
        {
            return m_board.TryGetTool(position, out tool);
        }

        public bool TryGetPosition(ITool             tool
                                 , out BoardPosition position)
        {
            return m_board.TryGetPosition(tool, out position);
        }

        public BoardState GetBoardState()
        {
            return m_board.GetBoardState();
        }

        public void Clear()
        {
            m_board.Clear();
        }

        private void addTool(BoardPosition position
                           , ITool         tool)
        {
            m_board.Add(position, tool);
            ToolAddEvent?.Invoke(tool, position);
        }

        private bool removeTool(BoardPosition position)
        {
            bool isRemoved = m_board.Remove(position);
            ToolRemoved?.Invoke(position);
            return isRemoved;
        }

        private bool isOnSameTeam(ITool toolA
                                , ITool toolB)
        {
            return toolA.Color.Equals(toolB.Color);
        }

        private bool isPromotion(ITool         toolMoved
                               , BoardPosition position)
        {
            return toolMoved is Pawn && isLastRow(toolMoved, position);
        }

        private bool isLastRow(ITool         movedTool
                             , BoardPosition position)
        {
            return movedTool.Color == Colors.White && position.Row == 8
                || movedTool.Color == Colors.Black && position.Row == 1;
        }
    }

}