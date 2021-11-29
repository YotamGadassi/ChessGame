using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoard
{
    public class BoardState : IEnumerable
    {
        
        private Dictionary<BoardPosition, ITool> m_board = new Dictionary<BoardPosition, ITool>();

        public void Add(BoardPosition Position, ITool Tool)
        {
            m_board[Position] = Tool;
        }

        public bool Remove(BoardPosition Position)
        {
            return m_board.Remove(Position);
        }

        public void Move(BoardPosition Start, BoardPosition End)
        {
            ITool toolToMove = m_board[Start];
            m_board.Remove(Start);
            m_board[End] = toolToMove;
        }

        public ITool GetTool(BoardPosition Position)
        {
            m_board.TryGetValue(Position, out ITool tool);
            return tool;
        }

        public BoardPosition GetPosition(ITool Tool)
        {
            foreach (KeyValuePair<BoardPosition, ITool> pair in m_board)
            {
                if (pair.Value == Tool)
                {
                    return pair.Key;
                }
            }

            return BoardPosition.Empty;
        }

        public void Clear()
        {
            m_board.Clear();
        }

        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            return m_board.GetEnumerator();
        }

        #endregion
    }
}
