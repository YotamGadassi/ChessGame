using Common;
using System;
using System.Collections.Generic;

namespace ChessBoard
{
   public interface IBoardQueryService
    {
        ITool GetTool(BoardPosition Position);

        BoardPosition GetPosition(ITool Tool);
        BoardState GetStateCopy();
   }

    public class Board : IBoardQueryService
    {
        private BoardState m_board = new BoardState();

        public event EventHandler<ChessBoardEventArgs> StateChangeEvent;

        public bool SafeAdd(BoardPosition Position, ITool Tool)
        {
            bool isAdded = m_board.SafeAdd(Position, Tool);
            if (!isAdded)
            {
                return false;
            }

            ChessBoardEventArgs args = new ChessBoardEventArgs(EventType.Add, Position, Tool);

            StateChangeEvent?.Invoke(this, args);
            return true;
        }

        public void ForceAdd(BoardPosition Position, ITool Tool)
        {
            m_board.ForceAdd(Position, Tool);
        }

        public void ForceAdd(IList<KeyValuePair<BoardPosition, ITool>> InitialState)
        {
            foreach (KeyValuePair<BoardPosition, ITool> pair in InitialState)
            {
                BoardPosition position = pair.Key;
                ITool tool = pair.Value;

                m_board.ForceAdd(position, tool);
            }
        }

        public bool Remove(BoardPosition Position)
        {
            bool isRemoved = m_board.Remove(Position);
            if (!isRemoved)
            {
                return false;
            }

            ChessBoardEventArgs args = new ChessBoardEventArgs(EventType.Remove, Position, null);

            StateChangeEvent?.Invoke(this, args);

            return true;
        }

        public bool SafeMove(BoardPosition Start, BoardPosition End)
        {
            ITool toolToMove = m_board.GetTool(Start);
            if (null == toolToMove)
            {
                return false;
            }

            ChessMoveChecker moveChecker = new ChessMoveChecker(End);

            bool isMoved = m_board.SafeMove(Start, End, moveChecker.CheckMove);
            if (!isMoved)
            {
                return false;
            }

            toolToMove.Move(End);
            ChessBoardEventArgs args = new ChessBoardEventArgs(EventType.Move, Start, End, null, null);

            StateChangeEvent?.Invoke(this, args);

            return true;
        }

        public void ForceMove(BoardPosition Start, BoardPosition End)
        {
            m_board.ForceMove(Start, End);

            ChessBoardEventArgs args = new ChessBoardEventArgs(EventType.Move, Start, End, null, null);

            StateChangeEvent?.Invoke(this, args);
        }

        public ITool GetTool(BoardPosition Position)
        {
            return m_board.GetTool(Position);
        }

        public BoardPosition GetPosition(ITool Tool)
        {
            return m_board.GetPosition(Tool);
        }

        public BoardState GetStateCopy()
        {
            BoardState boardCopy = new BoardState();
            foreach (KeyValuePair<BoardPosition, ITool> pair in m_board)
            {
                BoardPosition position = pair.Key;
                ITool toolCopy = pair.Value.GetCopy();

                boardCopy.ForceAdd(position, toolCopy);
            }

            return boardCopy;
        }
    }

    internal class ChessMoveChecker
    {
        BoardPosition EndPosition;

        public ChessMoveChecker(BoardPosition End)
        {
            EndPosition = End;
        }

        public bool CheckMove(ITool ToolToMove, ITool ToolAtEnd)
        {
            return ToolToMove.IsMovingLegal(EndPosition, ToolAtEnd);
        }
    }
}
