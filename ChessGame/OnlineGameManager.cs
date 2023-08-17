using System.Reflection;
using System.Windows.Media;
using Board;
using Common;
using Common.Chess;
using log4net;
using Tools;

namespace ChessGame
{
    public class OnlineGameManager
    {
        private static readonly ILog       s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public                  Team       CurrentMachineTeam { get; }
        public                  Color      CurrentColorTurn   => m_teams[m_currentTeamIndex];
        public                  bool       IsGameRunning      { get; private set; }

        private                 BasicBoard m_gameBoard;

        private readonly        Color[]? m_teams = { Colors.White, Colors.Black };
        private                 int      m_currentTeamIndex;
        private static readonly int      s_teamsAmount = 2;

        public OnlineGameManager(Team currentMachineTeam)
        {
            m_gameBoard        = new BasicBoard();
            IsGameRunning      = false;
            CurrentMachineTeam = currentMachineTeam;
        }

        public void StartGame()
        {
            m_currentTeamIndex = 0;
            IsGameRunning      = true;
            s_log.Info("Game started");
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTool(BoardPosition position
                             , out ITool     tool)
        {
            return m_gameBoard.TryGetTool(position, out tool);
        }

        public MoveResult Move(BoardPosition start
                             , BoardPosition end)
        {
            if (false == m_gameBoard.TryGetTool(start, out ITool toolToMove))
            {
                s_log.Warn($"Cannot move from {start} to {end}. There is no tool to move");
                return MoveResult.NoChangeOccurredResult;
            }

            MoveResultEnum resultEnum = MoveResultEnum.ToolMoved;

            if (true == m_gameBoard.TryGetTool(end, out ITool toolAtEnd))
            {
                //s_log.Warn($"Cannot move from {start} to {end}. There is a tool [{toolAtEnd}] at the end position");
                if (false == toolAtEnd.Color.Equals(toolToMove.Color))
                {
                    resultEnum |= MoveResultEnum.ToolKilled;
                    m_gameBoard.Remove(end);
                }
            }

            m_gameBoard.Remove(start);
            m_gameBoard.Add(end, toolToMove);

            switchCurrentTeam();
            return new MoveResult(resultEnum, start, end,
                                  toolToMove, toolAtEnd);
        }

        public void Promote(BoardPosition position
                          , ITool         newTool)
        {
            m_gameBoard.Remove(position);
            m_gameBoard.Add(position, newTool);
            switchCurrentTeam();
            s_log.Info($"Tool in position [{position}] has been promoted to [{newTool}]");
        }

        public void ForceAddTool(BoardPosition position
                               , ITool         newTool)
        {
            m_gameBoard.Remove(position);
            m_gameBoard.Add(position, newTool);
            s_log.Info($"Tool [{newTool}] is forced add to position [{position}]");
        }

        protected void switchCurrentTeam()
        {
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
        }
    }
}