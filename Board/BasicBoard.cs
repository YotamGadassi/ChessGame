using Tools;

namespace Board
{
    public class BasicBoard : IBoard, IBoardQuery
    {
        private BoardState m_boardState = new();
        private Dictionary<ITool, BoardPosition> m_toolToPosition = new();

        public void Add(BoardPosition position, ITool tool)
        {
            if (m_boardState.ContainsKey(position))
            {
                throw new ArgumentException($"Board already contains position {position}!");
            }

            if (m_toolToPosition.ContainsKey((tool)))
            {
                throw new ArgumentException($"Board already contains tool {tool}!");
            }

            m_boardState[position] = tool;
            m_toolToPosition[tool] = position;
        }

        public bool Remove(BoardPosition position)
        {
            bool isPositionContainsTool = m_boardState.TryGetValue(position, out ITool tool);
            if (isPositionContainsTool)
            {
                m_boardState.Remove(position);
                m_toolToPosition.Remove(tool);
            }

            return isPositionContainsTool;
        }

        public bool TryGetTool(BoardPosition position, out ITool tool)
        {
            return m_boardState.TryGetValue(position, out tool);
        }

        public bool TryGetPosition(ITool tool, out BoardPosition position)
        {
            return m_toolToPosition.TryGetValue(tool, out position);
        }

        public BoardState GetBoardState()
        {
            return new BoardState(m_boardState);
        }

        public void Clear()
        {
            m_toolToPosition.Clear();
            m_boardState.Clear();
        }

        public BoardState GetBoard => new BoardState(m_boardState);
    }
}
