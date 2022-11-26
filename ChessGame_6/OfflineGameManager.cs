using System.Reflection;
using System.Windows.Media;
using Board;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using log4net;

namespace ChessGame
{
    public class OfflineGameManager : IGameManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        protected ChessBoard m_gameBoard;

        // public event EventHandler<ChessBoardEventArgs> CheckEvent;
        public event EventHandler<CheckmateEventArgs>    CheckmateEvent;
        public event EventHandler<EventArgs>             EndGameEvent;
        public event EventHandler<EventArgs>             StartGameEvent;
        public event EventHandler<ToolMovedEventArgs>    ToolMovedEvent;
        public event EventHandler<KillingEventArgs>      ToolKilledEvent;
        public event PromotionEventHandler               PromotionEvent;
        public event EventHandler<ToolPromotedEventArgs> ToolPromotedEvent;
        public event EventHandler<Color>                 TeamSwitchEvent;

        public Color                                  CurrentColorTurn => m_teams[m_currentTeamIndex];

        protected                 Color[]? m_teams = { Colors.White, Colors.Black };
        protected                 int      m_currentTeamIndex;
        protected static readonly int      s_teamsAmount = 2;

        public bool IsGameRunning { get; private set; }

        public OfflineGameManager()
        {
            IsGameRunning = false;
            m_gameBoard     = new ChessBoard();
        }
        
        public MoveResult Move(BoardPosition start, BoardPosition end)
        {
            MoveResult     result     = m_gameBoard.Move(start, end);
            MoveResultEnum resultEnum = result.Result;

            if ((resultEnum & (MoveResultEnum.CheckMate | MoveResultEnum.NeedPromotion)) != 0)
            {
                //s_log.Info($"{resultEnum} occurred after move from {start} to {end}");
                return result;
            }

            if (resultEnum.HasFlag(MoveResultEnum.ToolMoved))
            {
                switchCurrentTeam();
            }

            return result;
        }

        public void EndGame()
        {
            IsGameRunning = false;
            m_gameBoard.Clear();
            m_teams            = null;
            m_currentTeamIndex = 0;
            EndGameEvent?.Invoke(this, EventArgs.Empty);
        }

        public void StartGame()
        {
            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = GameInitHelper.GenerateInitialArrangement(GameDirection.North, Colors.White);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = GameInitHelper.GenerateInitialArrangement(GameDirection.South, Colors.Black);

            foreach (KeyValuePair<BoardPosition, ITool> pair in whiteGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
                toolMovedHandler(new ToolMovedEventArgs(pair.Value, BoardPosition.Empty, pair.Key));
            }

            foreach (KeyValuePair<BoardPosition, ITool> pair in blackGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
                toolMovedHandler(new ToolMovedEventArgs(pair.Value, BoardPosition.Empty, pair.Key));
            }

            m_currentTeamIndex = 0;
            IsGameRunning    = true;
            StartGameEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public bool TryGetTool(BoardPosition position, out ITool tool)
        {
            return m_gameBoard.TryGetTool(position, out tool);
        }
        
        protected void switchCurrentTeam()
        {
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
            TeamSwitchEvent?.Invoke(this, m_teams[m_currentTeamIndex]);
        }

        private void toolKilledHandler(KillingEventArgs e)
        {
            OnToolKilledEvent(e);
        }

        private void toolMovedHandler(ToolMovedEventArgs e)
        {
            OnToolMovedEvent(e);
        }

        protected virtual void OnToolMovedEvent(ToolMovedEventArgs e)
        {
            ToolMovedEvent?.Invoke(this, e);
        }

        protected virtual void OnToolKilledEvent(KillingEventArgs e)
        {
            ToolKilledEvent?.Invoke(this, e);
        }

        protected virtual void OnCheckmateEvent(CheckmateEventArgs args)
        {
            CheckmateEvent?.Invoke(this, args);
        }
        
        public void Promote(BoardPosition position, ITool promotedTool)
        {
            m_gameBoard.Remove(position);
            m_gameBoard.Add(position, promotedTool);
            switchCurrentTeam();
        }

        protected void OnToolPromotedEvent(ToolPromotedEventArgs e)
        {
            ToolPromotedEvent?.Invoke(this, e);
        }

        public BoardState GetBoardState()
        {
            return m_gameBoard.GetBoard;
        }
    }

}
