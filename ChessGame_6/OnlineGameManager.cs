using System.Reflection;
using System.Windows.Media;
using Board;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using log4net;

namespace ChessGame
{
    public class OnlineGameManager : IGameManager
    {
        private static readonly ILog       s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public                  Team       CurrentMachineTeam { get; }
        private                 BasicBoard m_gameBoard;

        private                 Color[]? m_teams = { Colors.White, Colors.Black };
        private                 int      m_currentTeamIndex;
        private static readonly int      s_teamsAmount = 2;

        public OnlineGameManager(Team currentMachineTeam)
        {
            m_gameBoard        = new BasicBoard();
            IsGameRunning      = false;
            CurrentMachineTeam = currentMachineTeam;
        }

        public event EventHandler<CheckmateEventArgs>?    CheckmateEvent;
        public event EventHandler<EventArgs>?             EndGameEvent;
        public event EventHandler<EventArgs>?             StartGameEvent;
        public event EventHandler<ToolMovedEventArgs>?    ToolMovedEvent;
        public event EventHandler<KillingEventArgs>?      ToolKilledEvent;
        public event PromotionEventHandler?               PromotionEvent;
        public event EventHandler<ToolPromotedEventArgs>? ToolPromotedEvent;
        public event EventHandler<Color>?                 TeamSwitchEvent;
        public Color                                      CurrentColorTurn => m_teams[m_currentTeamIndex];
        public bool                                       IsGameRunning    { get; private set; }

        public void StartGame()
        {
            m_currentTeamIndex = 0;
            IsGameRunning      = true;
            s_log.Info("Game started");
            StartGameEvent?.Invoke(this, EventArgs.Empty);
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
                    resultEnum = MoveResultEnum.ToolKilled;
                }
            }

            switch (resultEnum)
            {
                case MoveResultEnum.ToolMoved:
                {
                    s_log.Info($@"Move from {start} to {end} successfully, tool [{toolToMove}]");
                    m_gameBoard.Remove(start);
                    m_gameBoard.Add(end, toolToMove);
                        toolMovedHandler(new ToolMovedEventArgs(toolToMove, start, end));
                    break;
                }
                case MoveResultEnum.ToolKilled:
                {
                    s_log.Info($@"Move and kill from {start} to {end} successfully, tool moved: [{toolToMove}], tool killed: [{toolAtEnd}]");
                    m_gameBoard.Remove(start);
                    m_gameBoard.Remove(end);
                    m_gameBoard.Add(end, toolToMove);
                    toolKilledHandler(new KillingEventArgs(toolToMove, start, end, toolAtEnd));
                    break;
                }
            }

            switchCurrentTeam();
            return new MoveResult(resultEnum, start, end,
                                  toolToMove, toolAtEnd);
        }

        public void ForceAddTool(BoardPosition position
                          , ITool         newTool)
        {
            if (true == m_gameBoard.TryGetTool(position, out ITool toolToPromote))
            {
                m_gameBoard.Remove(position);
                s_log.Info($"Tool [{toolToPromote}] in position [{position}] has been promoted to [{newTool}]");
            }
            else
            {
                s_log.Info($"Tool [{newTool}] is forced add to position [{position}]");
            }

            m_gameBoard.Add(position, newTool);
            ToolPromotedEvent?.Invoke(this, new ToolPromotedEventArgs(toolToPromote, newTool, position));
        }

        private void toolMovedHandler(ToolMovedEventArgs e)
        {
            OnToolMovedEvent(e);
        }

        protected virtual void OnToolMovedEvent(ToolMovedEventArgs e)
        {
            ToolMovedEvent?.Invoke(this, e);
        }

        private void toolKilledHandler(KillingEventArgs e)
        {
            OnToolKilledEvent(e);
        }

        protected virtual void OnToolKilledEvent(KillingEventArgs e)
        {
            ToolKilledEvent?.Invoke(this, e);
        }

        protected void switchCurrentTeam()
        {
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
            TeamSwitchEvent?.Invoke(this, m_teams[m_currentTeamIndex]);
        }
    }
}
