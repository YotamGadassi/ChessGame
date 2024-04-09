using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using Board;
using log4net;
using Tools;

namespace Common.Chess
{
    public class GameMoveHelper
    {
        private readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private delegate bool IsMoveLegalDelegate(BoardPosition start
                                                , BoardPosition end
                                                , ITool         toolToMove
                                                , bool          isKilling);

        private Dictionary<string, IsMoveLegalDelegate> m_isLegalDelegatesDict;

        private Dictionary<Color, GameDirection> m_colorToDirection;

        private readonly ChessBoard m_chessBoard;

        public static bool ValidatePositionOnBoard(BoardPosition position)
        {
            int upLeftBoundry    = 1;
            int downRightBoundry = 8;
            if (position.Column > downRightBoundry
             || position.Column < upLeftBoundry
             || position.Row    < upLeftBoundry
             || position.Row    > downRightBoundry)
            {
                return false;
            }

            return true;
        }

        public GameMoveHelper(ChessBoard chessBoard)
        {
            m_chessBoard = chessBoard;
            initMoveLegalDelegates();
            initColorToDirection();
        }

        public bool IsMoveLegal(BoardPosition start
                              , BoardPosition end)
        {
            if (false == m_chessBoard.TryGetTool(start, out ITool toolToMove))
            {
                s_log.Info($"No tool to move from position {start}. So no move occurred.");
                return false;
            }

            bool isToolAtEnd = m_chessBoard.TryGetTool(end, out ITool toolAtEnd);
            if (isToolAtEnd && isSameTeam(toolToMove, toolAtEnd))
            {
                s_log.Warn($"{toolToMove.Type} cannot move to a position which contains tool from same team!");
                return false;
            }

            IsMoveLegalDelegate func = m_isLegalDelegatesDict[toolToMove.Type];

            return func(start, end, toolToMove, isToolAtEnd);
        }

        private void initColorToDirection()
        {
            m_colorToDirection = new Dictionary<Color, GameDirection>();
            m_colorToDirection.Add(Colors.White, GameDirection.North);
            m_colorToDirection.Add(Colors.Black, GameDirection.South);
        }

        private void initMoveLegalDelegates()
        {
            m_isLegalDelegatesDict = new Dictionary<string, IsMoveLegalDelegate>();
            m_isLegalDelegatesDict.Add("Pawn",   pawnCheckMove);
            m_isLegalDelegatesDict.Add("Rook",   rookCheckMove);
            m_isLegalDelegatesDict.Add("Bishop", bishopCheckMove);
            m_isLegalDelegatesDict.Add("Knight", knightCheckMove);
            m_isLegalDelegatesDict.Add("Queen",  queenCheckMove);
            m_isLegalDelegatesDict.Add("King",   kingCheckMove);
        }

        private bool pawnCheckMove(BoardPosition start
                                 , BoardPosition end
                                 , ITool         toolToMove
                                 , bool          isKilling)
        {
            GameDirection toolGameDirection = m_colorToDirection[toolToMove.Color];

            BoardPosition convertedStart = ConvertDirection(start, toolGameDirection);
            BoardPosition convertedEnd   = ConvertDirection(end,   toolGameDirection);

            if (isKilling)
            {
                bool isColumnOkForKilling = convertedEnd.Column == convertedStart.Column + 1 ||
                                            convertedEnd.Column == convertedStart.Column - 1;
                bool isRowOkForKilling = convertedEnd.Row == convertedStart.Row + 1;

                if (!isColumnOkForKilling || !isRowOkForKilling)
                {
                    s_log.Info($"Pawn cannot kill from position {start} to {end}!");
                    return false;
                }
                //TODO: Add En passant rule

                s_log.Info($"Pawn killed from position {start} to {end}.");
                return true;
            }

            bool isColumnOk = convertedEnd.Column == convertedStart.Column;
            if (!isColumnOk)
            {
                s_log.Info($"Pawn cannot move to different column:[{start}->{end}]");
                return false;
            }

            bool isMoveTwoSquares = convertedEnd.Row == convertedStart.Row + 2;
            if (isMoveTwoSquares)
            {
                bool isFirstMove = convertedStart.Row == 2;
                if (false == isFirstMove)
                {
                    s_log.Info($"Pawn cannot move two squares if it's not it first move!");
                    return false;
                }

                bool isSquareBetweenOccupied = checkToolsBetweenColumns(start.Column, start.Row, end.Row);
                if (isSquareBetweenOccupied)
                {
                    s_log.Warn($"Pawn cannot skip other tool. Squares between {start}-{end} is occupied");
                    return false;
                }

                return true;
            }

            bool isMoveOneSquare = convertedEnd.Row == convertedStart.Row + 1;
            if (!isMoveOneSquare)
            {
                s_log.Warn($"Pawn cannot move from {start} to {end}");
                return false;
            }

            return true;
        }

        private bool rookCheckMove(BoardPosition start
                                 , BoardPosition end
                                 , ITool         toolToMove
                                 , bool          isKilling)
        {
            bool isSameColumn = start.Column == end.Column;
            bool isSameRow    = start.Row    == end.Row;

            if (isSameColumn)
            {
                int  moveColumm         = start.Column;
                int  rowOne             = start.Row;
                int  rowTwo             = end.Row;
                bool isToolsBetweenRows = checkToolsBetweenColumns(moveColumm, rowOne, rowTwo);
                if (isToolsBetweenRows)
                {
                    return false;
                }

                return true;
            }

            if (isSameRow)
            {
                int  moveRow               = start.Row;
                int  columnsOne            = start.Column;
                int  columnTwo             = end.Column;
                bool isToolsBetweenColumns = checkToolsBetweenRows(moveRow, columnsOne, columnTwo);
                if (isToolsBetweenColumns)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool bishopCheckMove(BoardPosition start
                                   , BoardPosition end
                                   , ITool         toolToMove
                                   , bool          isKilling)
        {
            int columnDiff = Math.Abs(end.Column - start.Column);
            int rowDiff    = Math.Abs(end.Row    - start.Row);
            if (columnDiff != rowDiff)
            {
                return false;
            }

            bool isToolsBetweenSquares = checkToolsBetweenDiagonalMove(start, end);
            if (isToolsBetweenSquares)
            {
                return false;
            }

            return true;
        }

        private bool knightCheckMove(BoardPosition start
                                   , BoardPosition end
                                   , ITool         toolToMove
                                   , bool          isKilling)
        {
            bool isMoveTwoColumns = Math.Abs(end.Column - start.Column) == 2;
            if (isMoveTwoColumns)
            {
                int rowsMoved = Math.Abs(end.Row - start.Row);
                return rowsMoved == 1;
            }

            bool isMoveTwoRows = Math.Abs(end.Row - start.Row) == 2;
            if (isMoveTwoRows)
            {
                int columnsMoved = Math.Abs(end.Column - start.Column);
                return columnsMoved == 1;
            }

            return false;
        }

        private bool queenCheckMove(BoardPosition start
                                  , BoardPosition end
                                  , ITool         toolToMove
                                  , bool          isKilling)
        {
            bool checkQueenMove = bishopCheckMove(start, end, toolToMove, isKilling) ||
                                  rookCheckMove(start, end, toolToMove, isKilling);
            return checkQueenMove;
        }

        private bool kingCheckMove(BoardPosition start
                                 , BoardPosition end
                                 , ITool         toolToMove
                                 , bool          isKilling)
        {
            int rowMoved    = Math.Abs(end.Row    - start.Row);
            int columnMoved = Math.Abs(end.Column - start.Column);

            return rowMoved <= 1 && columnMoved <= 1;
        }

        private bool checkToolsBetweenDiagonalMove(BoardPosition start
                                                 , BoardPosition end)
        {
            int startRow    = start.Row;
            int startColumn = start.Column;

            int endRow    = end.Row;
            int endColumn = end.Column;

            bool isRowMovingAscending   = endRow    > startRow;
            bool isColumMovingAscending = endColumn > startColumn;


            int valueAddToColumn = isColumMovingAscending ? 1 : -1;
            int valueAddToRow    = isRowMovingAscending ? 1 : -1;

            int column = startColumn + valueAddToColumn;
            int row    = startRow    + valueAddToRow;

            for (; row != endRow && column != endColumn; row += valueAddToRow, column += valueAddToColumn)
            {
                BoardPosition position = new BoardPosition(column, row);
                if (m_chessBoard.TryGetTool(position, out _))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkToolsBetweenColumns(int column
                                            , int rowOne
                                            , int rowTwo)
        {
            bool isMovingForward = rowTwo > rowOne;
            if (!isMovingForward)
            {
                int switchInt = rowOne;
                rowOne = rowTwo;
                rowTwo = switchInt;
            }

            for (int row = rowOne + 1; row < rowTwo; ++row)
            {
                BoardPosition position = new BoardPosition(column, row);
                if (m_chessBoard.TryGetTool(position, out _))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkToolsBetweenRows(int row
                                         , int columnOne
                                         , int columnTwo)
        {
            bool isMovingForward = columnTwo > columnOne;
            if (!isMovingForward)
            {
                int switchInt = columnOne;
                columnOne = columnTwo;
                columnTwo = switchInt;
            }

            for (int column = columnOne + 1; column < columnTwo; ++column)
            {
                BoardPosition position = new BoardPosition(column, row);
                if (m_chessBoard.TryGetTool(position, out _))
                {
                    return true;
                }
            }

            return false;
        }

        private bool isSameTeam(ITool toolToMove
                              , ITool toolAtEnd)
        {
            return toolAtEnd.Color == toolToMove.Color;
        }

        private BoardPosition ConvertDirection(BoardPosition position
                                             , GameDirection moveDirection)
        {
            if (moveDirection == GameDirection.South)
            {
                int column = 9 - position.Column;
                int row    = 9 - position.Row;

                BoardPosition convertedPosition = new(column, row);

                return convertedPosition;
            }

            return position;
        }
    }
}