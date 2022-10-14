using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ChessGame;
using Common_6;
using Tools;

namespace Client.Helpers
{
    public class AvailableMovesHelper
    {
        private delegate BoardPosition[] AvailableMovesDelegate(BoardPosition position, ITool tool);

        private Dictionary<Type, AvailableMovesDelegate> m_availableMovesDelegatesDict;

        private Dictionary<Color, GameDirection> m_colorToDirection;

        private BaseGameManager m_gameManager;

        public AvailableMovesHelper(BaseGameManager gameManager)
        {
            m_gameManager = gameManager;
            initColorToDirection();
            initAvailableMovesDelegates();
        }

        private void initColorToDirection()
        {
            m_colorToDirection = new Dictionary<Color, GameDirection>();
            m_colorToDirection.Add(Colors.White, GameDirection.North);
            m_colorToDirection.Add(Colors.Black, GameDirection.South);
        }

        private void initAvailableMovesDelegates()
        {
            m_availableMovesDelegatesDict = new Dictionary<Type, AvailableMovesDelegate>
                                            {
                                                { typeof(Pawn), pawnCalculateAvailablePositionsToMove },
                                                { typeof(Rook), rookCalculateAvailablePositionsToMove },
                                                { typeof(Bishop), bishopCalculateAvailablePositionsToMove },
                                                {typeof(Knight), knightCalculateAvailablePositionsToMove},
                                                {typeof(Queen), queenCalculateAvailablePositionsToMove},
                                                {typeof(King), kingCalculateAvailablePositionsToMove}
                                            };
        }

        public BoardPosition[] GetAvailablePositionToMove(BoardPosition position)
        {
            if (m_gameManager.TryGetTool(position, out ITool toolToMove))
            {
                return m_availableMovesDelegatesDict[toolToMove.GetType()](position, toolToMove);
            }

            return Array.Empty<BoardPosition>();
        }

        private BoardPosition[] pawnCalculateAvailablePositionsToMove(BoardPosition position, ITool pawn)
        {
            GameDirection toolGameDirection = m_colorToDirection[pawn.Color];
            int           rowIncrement;
            bool          isFirstMove;
            if (toolGameDirection == GameDirection.North)
            {
                rowIncrement = 1;
                isFirstMove  = position.Row == 2;
            }
            else
            {
                rowIncrement = -1;
                isFirstMove  = position.Row == 7;
            }

            int nextRow = position.Row + rowIncrement;
            //TODO: add en passant logic

            List<BoardPosition> positions              = new List<BoardPosition>();
            bool                isMoveForwardAvailable = false;
            BoardPosition       potentialPosition      = new BoardPosition(position.Column, nextRow);
            if (ValidatePositionOnBoard(potentialPosition) &&
                isPositionFree(potentialPosition))
            {
                positions.Add(potentialPosition);
                isMoveForwardAvailable = true;
            }

            if (isMoveForwardAvailable && isFirstMove)
            {
                potentialPosition = new BoardPosition(position.Column, nextRow + rowIncrement);
                if (ValidatePositionOnBoard(potentialPosition) &&
                    isPositionFree(potentialPosition))
                {
                    positions.Add(potentialPosition);
                }
            }

            BoardPosition westDiagonal = new BoardPosition(position.Column - 1, nextRow);
            if (ValidatePositionOnBoard(westDiagonal)
             && isKilling(westDiagonal, pawn))
            {
                positions.Add(westDiagonal);
            }

            BoardPosition eastDiagonal = new BoardPosition(position.Column + 1, nextRow);
            if (ValidatePositionOnBoard(eastDiagonal)
             && isKilling(eastDiagonal, pawn))
            {
                positions.Add(eastDiagonal);
            }

            return positions.ToArray();
        }

        private BoardPosition[] rookCalculateAvailablePositionsToMove(BoardPosition position, ITool rook)
        {
            List<BoardPosition> positions = getAllAvailablePositionToBorder(position, rook, 1,  0);
            positions.AddRange(getAllAvailablePositionToBorder(position,              rook, -1, 0));
            positions.AddRange(getAllAvailablePositionToBorder(position,              rook, 0,  1));
            positions.AddRange(getAllAvailablePositionToBorder(position,              rook, 0,  -1));

            return positions.ToArray();
        }

        private BoardPosition[] bishopCalculateAvailablePositionsToMove(BoardPosition position, ITool bishop)
        {
            List<BoardPosition> positions = getAllAvailablePositionToBorder(position, bishop, 1, 1);

            positions.AddRange(getAllAvailablePositionToBorder(position, bishop, -1, -1));
            positions.AddRange(getAllAvailablePositionToBorder(position, bishop, 1,  -1));
            positions.AddRange(getAllAvailablePositionToBorder(position, bishop, -1, 1));

            return positions.ToArray();
        }

        private BoardPosition[] knightCalculateAvailablePositionsToMove(BoardPosition position, ITool knight)
        {
            List<BoardPosition> positions = new List<BoardPosition>();

            positions.Add(getNewPosition(position, -2, 1));  // NorthWest1
            positions.Add(getNewPosition(position, -1, 2));  // NorthWest2
            positions.Add(getNewPosition(position, -2, -1)); // SouthWest1
            positions.Add(getNewPosition(position, -1, -2)); // SouthWest2
            positions.Add(getNewPosition(position, 2,  1));  // NorthEast1
            positions.Add(getNewPosition(position, 1,  2));  // NorthEast2
            positions.Add(getNewPosition(position, 2,  -1)); // SouthEast1
            positions.Add(getNewPosition(position, 1,  -2)); // SouthEast2

            List<BoardPosition> positionsToSend = new List<BoardPosition>();

            foreach (BoardPosition boardPosition in positions)
            {
                if (ValidatePositionOnBoard(boardPosition) &&
                    isPositionFreeOrKilling(boardPosition, knight))
                {
                    positionsToSend.Add(boardPosition);
                }
            }

            return positionsToSend.ToArray();
        }

        private BoardPosition[] kingCalculateAvailablePositionsToMove(BoardPosition position, ITool king)
        {
            List<BoardPosition> positions = new List<BoardPosition>();

            for (int rowDiff = -1; rowDiff <= 1; ++rowDiff)
            {
                for (int colDiff = -1; colDiff <= 1; ++colDiff)
                {
                    BoardPosition positionToCheck = getNewPosition(position, colDiff, rowDiff);
                    if (ValidatePositionOnBoard(positionToCheck) && isPositionFreeOrKilling(positionToCheck, king))
                    {
                        positions.Add(positionToCheck);
                    }
                }
            }

            return positions.ToArray();
        }

        private BoardPosition[] queenCalculateAvailablePositionsToMove(BoardPosition position, ITool queen)
        {
            List<BoardPosition> positions = bishopCalculateAvailablePositionsToMove(position, queen).ToList();
            positions.AddRange(rookCalculateAvailablePositionsToMove(position, queen));

            return positions.ToArray();
        }


        private List<BoardPosition> getAllAvailablePositionToBorder(BoardPosition startPosition,ITool tool, int colDiff, int rowDiff)
        {
            List<BoardPosition> positions = new List<BoardPosition>(); 
            
            BoardPosition positionToCheck = getNewPosition(startPosition, colDiff, rowDiff);
            while (ValidatePositionOnBoard(positionToCheck) && isPositionFree(positionToCheck))
            {
                positions.Add(positionToCheck);
                positionToCheck = getNewPosition(positionToCheck, colDiff, rowDiff);
            }

            if (isKilling(positionToCheck, tool))
            {
                positions.Add(positionToCheck);
            }

            return positions;
        }

        private  BoardPosition getNewPosition(BoardPosition tempPosition, int colDiff, int rowDiff)
        {
            return new BoardPosition(tempPosition.Column + colDiff, tempPosition.Row + rowDiff);
        }

        private bool isPositionFree(BoardPosition position)
        {
            return false == m_gameManager.TryGetTool(position, out _);
        }

        private  bool isPositionFreeOrKilling(BoardPosition positionToMove, ITool tool)
        {
            bool isPositionOccupied = m_gameManager.TryGetTool(positionToMove, out ITool toolOnPosition);
            if (isPositionOccupied && isSameTeam(toolOnPosition, tool))
            {
                return false;
            }

            return true;
        }

        private bool isKilling(BoardPosition positionToMove, ITool tool)
        {
            bool isPositionOccupied = m_gameManager.TryGetTool(positionToMove, out ITool toolOnPosition);
            if (isPositionOccupied && false == isSameTeam(toolOnPosition, tool))
            {
                return true;
            }

            return false;
        }

        public  bool ValidatePositionOnBoard(BoardPosition position)
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

        private  bool isSameTeam(ITool toolToMove, ITool toolAtEnd)
        {
            return toolAtEnd.Color == toolToMove.Color;
        }

    }
}
