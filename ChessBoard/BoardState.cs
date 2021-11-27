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

        public bool SafeAdd(BoardPosition Position, ITool Tool)
        {
            bool keyExists = m_board.ContainsKey(Position);
            if (keyExists)
            {
                return false;
            }

            m_board.Add(Position, Tool);
            
            return true;
        }

        public void ForceAdd(BoardPosition Position, ITool Tool)
        {
            m_board[Position] = Tool;
        }

        public bool Remove(BoardPosition Position)
        {
            return m_board.Remove(Position);
        }

        public bool SafeMove(BoardPosition Start, BoardPosition End, Func<ITool, ITool, bool> PredicateForEndPosition)
        {
            m_board.TryGetValue(Start, out ITool toolToMove);
            m_board.TryGetValue(End, out ITool toolAtEnd);

            bool moveLegal = PredicateForEndPosition(toolToMove, toolAtEnd);

            if (!moveLegal)
            {
                return false;
            }

            m_board.Remove(Start);
            m_board[End] = toolToMove;

            return true;
        }

        public void ForceMove(BoardPosition Start, BoardPosition End)
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
