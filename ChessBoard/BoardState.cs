using System;
using System.Collections.Generic;
using Common;

namespace ChessBoard
{
    public class BoardState
    {
        private Dictionary<BoardPosition, ITool> m_positionToTool = new Dictionary<BoardPosition, ITool>();
        private Dictionary<ITool, BoardPosition> m_toolToPosition = new Dictionary<ITool, BoardPosition>();

        public bool Add(BoardPosition position, ITool tool)
        {
            if (m_positionToTool.ContainsKey(position)
                ||m_toolToPosition.ContainsKey((tool)))
            {
                throw new ArgumentException($"Board already contains tool {tool}!");
            }
            
            m_positionToTool[position] = tool;
            m_toolToPosition[tool] = position;
            return true;
        }

        public bool Remove(BoardPosition position)
        {
            bool isPositionContainsTool = m_positionToTool.TryGetValue(position, out ITool tool);
            if (isPositionContainsTool)
            {
                m_positionToTool.Remove(position);
                m_toolToPosition.Remove(tool);
            }

            return isPositionContainsTool;
        }

        public bool TryGetTool(BoardPosition position, out ITool tool)
        {
            return m_positionToTool.TryGetValue(position, out tool);
        }

        public bool TryGetPosition(ITool tool, out BoardPosition position)
        {
            return m_toolToPosition.TryGetValue(tool, out position);
        }

        public void Clear()
        {
            m_toolToPosition.Clear();
            m_positionToTool.Clear();
        }

    }
}
