using ChessBoard;
using System.Collections.Generic;
using Common;
using System.Windows.Media;
using System;

namespace Game
{
    public class GameMoveHelper
    {
        private delegate bool IsMoveLegalDelegate(BoardPosition Start, BoardPosition End);

        private Dictionary<string, IsMoveLegalDelegate> m_delegatesDict;
        private HashSet<ITool> toolsThatMoved = new HashSet<ITool>();
        private Dictionary<Color, GameDirection> m_colorToDirection;
        private IBoardQueryService m_board;

        public void ReportMovingTool(ITool tool)
        {
            toolsThatMoved.Add(tool);
        }

        public GameMoveHelper(IBoardQueryService boardQueryService, IEnumerable<Team> Teams)
        {
            m_board = boardQueryService;

            initMoveLegalDelegates();
            initColorToDirection(Teams);
        }

        private void initColorToDirection(IEnumerable<Team> teams)
        {
            m_colorToDirection = new Dictionary<Color, GameDirection>();
            foreach(Team team in teams)
            {
                m_colorToDirection.Add(team.Color, team.MoveDirection);
            }
        }

        private void initMoveLegalDelegates()
        {
            m_delegatesDict = new Dictionary<string, IsMoveLegalDelegate>();
            m_delegatesDict.Add("Pawn", pawnCheckMove);
            m_delegatesDict.Add("Rook", rookCheckMove);
            m_delegatesDict.Add("Bishop", bishopCheckMove);
            m_delegatesDict.Add("Knight", knightCheckMove);
            m_delegatesDict.Add("Queen", queenCheckMove);
            m_delegatesDict.Add("King", kingCheckMove);
        }

        public bool IsMoveLegal(BoardPosition Start, BoardPosition End)
        {
            ITool ToolToMove = m_board.GetTool(Start);
            IsMoveLegalDelegate func = m_delegatesDict[ToolToMove.Type];

            return func(Start, End);
        }

        private bool pawnCheckMove(BoardPosition Start, BoardPosition End)
        {
            ITool ToolToMove = m_board.GetTool(Start);
            ITool ToolAtEnd = m_board.GetTool(End);

            GameDirection toolGameDirection = m_colorToDirection[ToolToMove.Color];
            bool isKilling = false;
            if (ToolAtEnd != null)
            {
                if (isSameTeam(ToolToMove, ToolAtEnd))
                {
                    return false;
                }

                isKilling = true;
            }

            BoardPosition convertedStart = ConvertDirection(Start, toolGameDirection);
            BoardPosition convertedEnd = ConvertDirection(End, toolGameDirection);

            if (isKilling)
            {
                bool isColumnOkForKilling = (convertedEnd.Column == convertedStart.Column + 1) || (convertedEnd.Column == convertedStart.Column - 1);
                bool isRowOkForKilling = (convertedEnd.Row == convertedStart.Row + 1);

                if (!isColumnOkForKilling || !isRowOkForKilling)
                {
                    return false;
                }
                //Add En passant rule

                return true;
            }

            bool isColumnOk = convertedEnd.Column == convertedStart.Column;
            if (!isColumnOk)
            {
                return false;
            }

            bool isFirstMove = !toolsThatMoved.Contains(ToolToMove);
            if (isFirstMove)
            {
                bool isMoveTwoSquares = convertedEnd.Row == convertedStart.Row + 2;

                if (isMoveTwoSquares)
                {
                    
                    int row = Start.Row + 1;
                    if (toolGameDirection == GameDirection.Backward)
                    {
                        row = End.Row - Start.Row;
                    }

                    BoardPosition inBetweenSquare = new BoardPosition(Start.Column, row);
                    bool isSquareBetweenOccupied = m_board.GetTool(inBetweenSquare) != null;
                    if (isSquareBetweenOccupied)
                    {
                        return false;
                    }

                    return true;
                }
            }
            
            bool isMoveOneSquare = convertedEnd.Row == convertedStart.Row + 1;
            if (!isMoveOneSquare)
            {
                return false;
            }

            return true;

        }

        private bool rookCheckMove(BoardPosition Start, BoardPosition End)
        {
            bool isSameColumn = Start.Column == End.Column;
            bool isSameRow = Start.Row == End.Row;

            ITool ToolToMove = m_board.GetTool(Start);
            ITool ToolAtEnd = m_board.GetTool(End);

            if (ToolAtEnd != null && isSameTeam(ToolToMove, ToolAtEnd))
            {
                return false;
            }

            if (isSameColumn)
            {
                int moveColumm = Start.Column;
                int rowOne = Start.Row;
                int rowTwo = End.Row;
                bool isToolsBetweenRows = checkToolsBetweenColumns(moveColumm, rowOne, rowTwo);
                if (isToolsBetweenRows)
                {
                    return false;
                }
                return true;
            }
            else if (isSameRow)
            {
                int moveRow = Start.Row;
                int columnsOne = Start.Column;
                int columnTwo = End.Column;
                bool isToolsBetweenColumns = checkToolsBetweenRows(moveRow, columnsOne, columnTwo);
                if (isToolsBetweenColumns)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool bishopCheckMove(BoardPosition Start, BoardPosition End)
        {

            ITool ToolToMove = m_board.GetTool(Start);
            ITool ToolAtEnd = m_board.GetTool(End);

            if (ToolAtEnd != null && isSameTeam(ToolToMove, ToolAtEnd))
            {
                return false;
            }

            int columnDiff = Math.Abs(End.Column - Start.Column);
            int rowDiff = Math.Abs(End.Row - Start.Row);
            if (columnDiff != rowDiff)
            {
                return false;
            }

            bool isToolsBetweenSquares = checkToolsBetweenDiagonlMove(Start, End);
            if (isToolsBetweenSquares)
            {
                return false;
            }

            return true;
        }

        private bool knightCheckMove(BoardPosition Start, BoardPosition End)
        {
            ITool ToolToMove = m_board.GetTool(Start);
            ITool ToolAtEnd = m_board.GetTool(End);

            if (ToolAtEnd != null && isSameTeam(ToolToMove, ToolAtEnd))
            {
                return false;
            }

            bool isMoveTwoColumns = Math.Abs(End.Column - Start.Column) == 2;
            if (isMoveTwoColumns)
            {
                int rowsMoved = Math.Abs(End.Row - Start.Row);
                return rowsMoved == 1;
            }

            bool isMoveTwoRows = Math.Abs(End.Row - Start.Row) == 2;
            if (isMoveTwoRows)
            {
                int columnsMoved = Math.Abs(End.Column - Start.Column);
                return columnsMoved == 1;
            }

            return false;
        }

        private bool queenCheckMove(BoardPosition Start, BoardPosition End)
        {
            bool checkQueenMove = bishopCheckMove(Start, End) || rookCheckMove(Start, End);
            return checkQueenMove;
        }

        private bool kingCheckMove(BoardPosition Start, BoardPosition End)
        {
            ITool toolToMove = m_board.GetTool(Start);
            ITool toolAtEnd = m_board.GetTool(End);

            if (null != toolAtEnd && isSameTeam(toolToMove, toolAtEnd))
            {
                return false;
            }

            int rowMoved = Math.Abs(End.Row - Start.Row);
            int columnMoved = Math.Abs(End.Column - Start.Column);

            return rowMoved <= 1 && columnMoved <= 1;
        }

        private bool checkToolsBetweenDiagonlMove(BoardPosition start, BoardPosition end)
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
                ITool tool = m_board.GetTool(position);
                if (null != tool)
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkToolsBetweenColumns(int column, int rowOne, int rowTwo)
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
                ITool tool = m_board.GetTool(position);
                if (null != tool)
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkToolsBetweenRows(int row, int columnOne, int columnTwo)
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
                ITool tool = m_board.GetTool(position);
                if (null != tool)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool isSameTeam(ITool ToolToMove, ITool ToolAtEnd)
        {
            return ToolAtEnd.Color == ToolToMove.Color;
        }

        private BoardPosition ConvertDirection(BoardPosition position, GameDirection moveDirection)
        {
            if (moveDirection == GameDirection.Backward)
            {
                int Column = 7 - position.Column;
                int Row = 7 - position.Row;
                
                BoardPosition convertedPosition = new BoardPosition(Column, Row);
                
                return convertedPosition;
            }

            return position;
        }
    }
}
