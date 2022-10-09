using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using Common;
using log4net;
using Tools;

namespace ChessBoard
{
    public class GameMoveHelper
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private delegate bool IsMoveLegalDelegate(BoardPosition start,     BoardPosition end, ITool toolToMove,
                                                  bool          isKilling, ChessBoard    chessBoard);

        private delegate BoardPosition[] AvailableMovesDelegate(BoardPosition position, ITool tool, ChessBoard chessBoard);

        private static Dictionary<string, IsMoveLegalDelegate>        m_isLegalDelegatesDict;
        private        static Dictionary<Type, AvailableMovesDelegate> m_availableMovesDelegatesDict;

        private static Dictionary<Color, GameDirection> m_colorToDirection;

        static GameMoveHelper()
        {
            initMoveLegalDelegates();
            initAvailableMovesDelegates();
            initColorToDirection();
        }

        private static void initAvailableMovesDelegates()
        {
            m_availableMovesDelegatesDict = new Dictionary<Type, AvailableMovesDelegate>();
            m_availableMovesDelegatesDict.Add(typeof(Pawn), pawnCalculateAvailablePositionsToMove);
            m_availableMovesDelegatesDict.Add(typeof(Rook), rookCalculateAvailablePositionsToMove);
        }

        public static bool IsMoveLegal(BoardPosition start, BoardPosition end, ChessBoard chessBoard)
        {
            if (false == chessBoard.TryGetTool(start, out ITool toolToMove))
            {
                s_log.Info($"No tool to move from position {start}. So no move occurred.");
                return false;
            }

            bool isToolAtEnd = chessBoard.TryGetTool(end, out ITool toolAtEnd);
            if (isToolAtEnd && isSameTeam(toolToMove, toolAtEnd))
            {
                s_log.Warn($"{toolToMove.Type} cannot move to a position which contains tool from same team!");
                return false;
            }

            IsMoveLegalDelegate func = m_isLegalDelegatesDict[toolToMove.Type];

            return func(start, end, toolToMove, isToolAtEnd, chessBoard);
        }

        private static void initColorToDirection()
        {
            m_colorToDirection = new Dictionary<Color, GameDirection>();
            m_colorToDirection.Add(Colors.White, GameDirection.North);
            m_colorToDirection.Add(Colors.Black, GameDirection.South);
        }

        private static void initMoveLegalDelegates()
        {
            m_isLegalDelegatesDict = new Dictionary<string, IsMoveLegalDelegate>();
            m_isLegalDelegatesDict.Add("Pawn", pawnCheckMove);
            m_isLegalDelegatesDict.Add("Rook", rookCheckMove);
            m_isLegalDelegatesDict.Add("Bishop", bishopCheckMove);
            m_isLegalDelegatesDict.Add("Knight", knightCheckMove);
            m_isLegalDelegatesDict.Add("Queen", queenCheckMove);
            m_isLegalDelegatesDict.Add("King", kingCheckMove);
        }

        private bool pawnCheckMove(BoardPosition start, BoardPosition end, ITool toolToMove, bool isKilling, ChessBoard chessBoard)
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
                bool isFirstMove = start.Row == 2;
                if (false == isFirstMove)
                {
                    s_log.Info($"Pawn cannot move two squares if it's not it first move!");
                    return false;
                }

                int nextRow = start.Row + 1;
                if (toolGameDirection == GameDirection.South)
                {
                    nextRow = end.Row - start.Row;
                }

                BoardPosition inBetweenSquare         = new BoardPosition(start.Column, nextRow);
                bool          isSquareBetweenOccupied = chessBoard.TryGetTool(inBetweenSquare, out ITool toolInBetween);
                if (isSquareBetweenOccupied)
                {
                    s_log.Warn($"Pawn cannot skip other tool. Position {inBetweenSquare} is occupied with tool {toolInBetween.Type}");
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

        private bool rookCheckMove(BoardPosition start, BoardPosition end, ITool toolToMove, bool isKilling, ChessBoard chessBoard)
        {
            bool isSameColumn = start.Column == end.Column;
            bool isSameRow = start.Row == end.Row;

            if (isSameColumn)
            {
                int  moveColumm         = start.Column;
                int  rowOne             = start.Row;
                int  rowTwo             = end.Row;
                bool isToolsBetweenRows = checkToolsBetweenColumns(moveColumm, rowOne, rowTwo, chessBoard);
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
                bool isToolsBetweenColumns = checkToolsBetweenRows(moveRow, columnsOne, columnTwo, chessBoard);
                if (isToolsBetweenColumns)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool bishopCheckMove(BoardPosition start, BoardPosition end, ITool toolToMove, bool isKilling, ChessBoard chessBoard)
        {
            int columnDiff = Math.Abs(end.Column - start.Column);
            int rowDiff = Math.Abs(end.Row - start.Row);
            if (columnDiff != rowDiff)
            {
                return false;
            }

            bool isToolsBetweenSquares = checkToolsBetweenDiagonlMove(start, end, chessBoard);
            if (isToolsBetweenSquares)
            {
                return false;
            }

            return true;
        }

        private bool knightCheckMove(BoardPosition start, BoardPosition end, ITool toolToMove, bool isKilling, ChessBoard chessBoard)
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

        private bool queenCheckMove(BoardPosition start, BoardPosition end, ITool toolToMove, bool isKilling, ChessBoard chessBoard)
        {
            bool checkQueenMove = bishopCheckMove(start, end, toolToMove, isKilling, chessBoard) || rookCheckMove(start, end, toolToMove, isKilling, chessBoard);
            return checkQueenMove;
        }

        private bool kingCheckMove(BoardPosition start, BoardPosition end, ITool toolToMove, bool isKilling, ChessBoard chessBoard)
        {
            int rowMoved = Math.Abs(end.Row - start.Row);
            int columnMoved = Math.Abs(end.Column - start.Column);

            return rowMoved <= 1 && columnMoved <= 1;
        }

        private bool checkToolsBetweenDiagonlMove(BoardPosition start, BoardPosition end, ChessBoard chessBoard)
        {
            int startRow = start.Row;
            int startColumn = start.Column;

            int endRow = end.Row;
            int endColumn = end.Column;

            bool isRowMovingAscending = endRow > startRow;
            bool isColumMovingAscending = endColumn > startColumn;


            int valueAddToColumn = isColumMovingAscending ? 1 : -1;
            int valueAddToRow = isRowMovingAscending ? 1 : -1;

            int column = startColumn + valueAddToColumn;
            int row = startRow + valueAddToRow;

            for (; row != endRow && column != endColumn; row += valueAddToRow, column += valueAddToColumn)
            {
                BoardPosition position = new BoardPosition(column, row);
                if (chessBoard.TryGetTool(position, out _))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkToolsBetweenColumns(int column, int rowOne, int rowTwo, ChessBoard chessBoard)
        {
            bool isMovingForward = rowTwo > rowOne;
            if (!isMovingForward)
            {
                int switchInt = rowOne;
                rowOne = rowTwo;
                rowTwo = switchInt;
            }

            for(int row = rowOne + 1; row < rowTwo; ++row)
            {
                BoardPosition position = new BoardPosition(column, row);
                if (chessBoard.TryGetTool(position, out _))
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkToolsBetweenRows(int row, int columnOne, int columnTwo, ChessBoard chessBoard)
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
                if (chessBoard.TryGetTool(position, out _))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool isSameTeam(ITool toolToMove, ITool toolAtEnd)
        {
            return toolAtEnd.Color == toolToMove.Color;
        }

        private BoardPosition ConvertDirection(BoardPosition position, GameDirection moveDirection)
        {
            if (moveDirection == GameDirection.South)
            {
                int column = 7 - position.Column;
                int row = 7 - position.Row;
                
                BoardPosition convertedPosition = new BoardPosition(column, row);
                
                return convertedPosition;
            }

            return position;
        }

        public BoardPosition[] GetAvailablePositionToMove(BoardPosition position, ChessBoard chessBoard)
        {
            if (chessBoard.TryGetTool(position, out ITool toolToMove))
            {
                return m_availableMovesDelegatesDict[toolToMove.GetType()](position, toolToMove, chessBoard);
            }

            return null;
        }

        private BoardPosition[] pawnCalculateAvailablePositionsToMove(BoardPosition position, ITool pawn, ChessBoard chessBoard)
        {
            GameDirection toolGameDirection = m_colorToDirection[pawn.Color];
            int           rowIncrement      = toolGameDirection == GameDirection.North ? 1 : -1;
            int           nextRow           = position.Row + rowIncrement;
            
            //TODO: add en passant logic
            
            List<BoardPosition> potentialPositions = new List<BoardPosition>()
                                        {
                                            new BoardPosition(position.Column,     nextRow),
                                        };

            bool isFirstMove = position.Row == 1;
            if (isFirstMove)
            {
                potentialPositions.Add(new BoardPosition(position.Column, nextRow + rowIncrement));
            }

            foreach (BoardPosition boardPosition in potentialPositions)
            {
                if (false == ValidatePositionOnBoard(boardPosition) || false == isPositionFreeOrKilling(boardPosition, pawn, chessBoard))
                {
                    potentialPositions.Remove(boardPosition);
                }
            }

            BoardPosition westDiagonal = new BoardPosition(position.Column - 1, nextRow);
            if (ValidatePositionOnBoard(westDiagonal) 
             && chessBoard.TryGetTool(westDiagonal, out ITool westDiagonalTool) 
             && false == isSameTeam(westDiagonalTool, pawn))
            {
                potentialPositions.Add(westDiagonal);
            }

            BoardPosition eastDiagonal = new BoardPosition(position.Column + 1, nextRow);
            if (ValidatePositionOnBoard(eastDiagonal)  
             && chessBoard.TryGetTool(eastDiagonal , out ITool eastDiagonalTool) 
             && false == isSameTeam(eastDiagonalTool, pawn))
            {
                potentialPositions.Add(eastDiagonal);
            }

            return potentialPositions.ToArray();
        }

        private BoardPosition[] rookCalculateAvailablePositionsToMove(BoardPosition position, ITool rook, ChessBoard chessBoard)
        {
            List<BoardPosition> positions = new List<BoardPosition>();

            int           diff         = 1;
            BoardPosition tempPosition = position;
            tempPosition = getNewPosition(tempPosition, 0, diff);
            
            while (ValidatePositionOnBoard(tempPosition) && isPositionFreeOrKilling(tempPosition, rook, chessBoard))
            {
                positions.Add(tempPosition);
                ++diff;
                tempPosition = getNewPosition(tempPosition, 0, diff);
            }

            diff         = -1;
            tempPosition = position;
            tempPosition = getNewPosition(tempPosition, 0, diff);
            while (ValidatePositionOnBoard(tempPosition) && isPositionFreeOrKilling(tempPosition, rook, chessBoard))
            {
                positions.Add(tempPosition);
                --diff;
                tempPosition = getNewPosition(tempPosition, 0, diff);
            }
            
            diff         = 1;
            tempPosition = position;
            tempPosition = getNewPosition(tempPosition, diff, 0);
            while (ValidatePositionOnBoard(tempPosition) && isPositionFreeOrKilling(tempPosition, rook, chessBoard))
            {
                positions.Add(tempPosition);
                --diff;
                tempPosition = getNewPosition(tempPosition, diff, diff);
            }
            
            diff         = -1;
            tempPosition = position;
            tempPosition = getNewPosition(tempPosition, diff, 0);
            while (ValidatePositionOnBoard(tempPosition) && isPositionFreeOrKilling(tempPosition, rook, chessBoard))
            {
                positions.Add(tempPosition);
                --diff;
                tempPosition = getNewPosition(tempPosition, diff, 0);
            }

            return positions.ToArray();
        }

        private BoardPosition getNewPosition(BoardPosition tempPosition, int colDiff, int rowDiff)
        {
            return new BoardPosition(tempPosition.Column + colDiff, tempPosition.Row + rowDiff);
        }

        private bool isPositionFreeOrKilling(BoardPosition positionToMove, ITool tool, ChessBoard chessBoard)
        {
            bool  isPositionOccupied = chessBoard.TryGetTool(positionToMove, out ITool toolOnPosition);
            if (isPositionOccupied && isSameTeam(toolOnPosition, tool))
            {
                return false;
            }

            return true;
        }

        public bool ValidatePositionOnBoard(BoardPosition position)
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
    }
}
