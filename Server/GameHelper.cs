using ChessBoard;
using System.Collections.Generic;
using Common;

namespace Game
{
    public class GameHelper
    {
        private delegate bool IsMoveLegalDelegate(BoardPosition Start, BoardPosition End);

        private Dictionary<string, IsMoveLegalDelegate> m_delegatesDict = new Dictionary<string, IsMoveLegalDelegate>();
        private HashSet<ITool> toolsThatMoved = new HashSet<ITool>();

        private IBoardQueryService m_board;

        public void ReportMovingTool(ITool tool)
        {
            toolsThatMoved.Add(tool);
        }

        public GameHelper(IBoardQueryService boardQueryService)
        {
            m_board = boardQueryService;
            m_delegatesDict.Add("Pawn", pawnCheckMove);
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

            bool isKilling = false;
            if (ToolAtEnd != null)
            {
                if(ToolAtEnd.Color == ToolToMove.Color)
                {
                    return false;
                }
                isKilling = true;
            }

            BoardPosition convertedStart = ConvertDirection(Start, ToolToMove.MovingDirection);
            BoardPosition convertedEnd = ConvertDirection(End, ToolToMove.MovingDirection);

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
                    if (ToolToMove.MovingDirection == GameDirection.Backward)
                    {
                        row = Start.Row - 1;
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
