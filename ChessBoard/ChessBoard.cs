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

        public void Add(BoardPosition Position, ITool Tool)
        {
            m_board.Add(Position, Tool);

            ChessBoardEventArgs args = new ChessBoardEventArgs(EventType.Add, Position, Tool);

            StateChangeEvent?.Invoke(this, args);
        }

        public void Add(IList<KeyValuePair<BoardPosition, ITool>> InitialState)
        {
            foreach (KeyValuePair<BoardPosition, ITool> pair in InitialState)
            {
                BoardPosition position = pair.Key;
                ITool tool = pair.Value;

                m_board.Add(position, tool);
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

        public void Move(BoardPosition Start, BoardPosition End)
        {
            m_board.Move(Start, End);

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

        public void ClearBoard()
        {
            m_board.Clear();
        }

        public BoardState GetStateCopy()
        {
            BoardState boardCopy = new BoardState();
            foreach (KeyValuePair<BoardPosition, ITool> pair in m_board)
            {
                BoardPosition position = pair.Key;
                ITool toolCopy = pair.Value.GetCopy();

                boardCopy.Add(position, toolCopy);
            }

            return boardCopy;
        }
    }

}
