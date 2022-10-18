using System.Windows.Media;
using Common_6;
using Common_6.ChessBoardEventArgs;

namespace ChessGame
{
    public abstract class BaseGameManager
    {
        public delegate ITool PromotionEventHandler (object sender, PromotionEventArgs e);
        
        protected ChessBoard.ChessBoard m_gameBoard;

        // public event EventHandler<ChessBoardEventArgs> CheckEvent;
        public event EventHandler<EventArgs> CheckmateEvent;
        public event EventHandler<EventArgs> EndGameEvent;
        public event EventHandler<EventArgs> StartGameEvent;
        public event EventHandler<ToolMovedEventArgs> ToolMovedEvent;
        public event EventHandler<KillingEventArgs> ToolKilledEvent;
        public event PromotionEventHandler PromotionEvent;
        public event EventHandler<Color> TeamSwitchEvent;

        protected Color[] m_teams = {Colors.White, Colors.Black};
        protected                 int    m_currentTeamIndex;
        protected static readonly int    s_teamsAmount = 2;
        public                    Color   CurrentColorTurn => m_teams[m_currentTeamIndex];

        protected BaseGameManager()
        {
            m_gameBoard                =  new ChessBoard.ChessBoard();
            m_gameBoard.ToolMovedEvent += toolMovedHandler;
            m_gameBoard.KillingEvent   += toolKilledHandler;
        }
        
        public bool Move(BoardPosition start, BoardPosition end)
        {
            return m_gameBoard.Move(start, end);
        }

        public void EndGame()
        {
            m_gameBoard.ClearBoard();
            m_teams            = null;
            m_currentTeamIndex = 0;
        }

        public void StartGame()
        {
            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = getInitialBoardArrangement(GameDirection.North, Colors.White);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = getInitialBoardArrangement(GameDirection.South, Colors.Black);

            foreach (KeyValuePair<BoardPosition, ITool> pair in whiteGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
                toolMovedHandler(this, new ToolMovedEventArgs(pair.Value, BoardPosition.Empty, pair.Key));
            }

            foreach (KeyValuePair<BoardPosition, ITool> pair in blackGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
                toolMovedHandler(this, new ToolMovedEventArgs(pair.Value, BoardPosition.Empty, pair.Key));
            }

            m_currentTeamIndex = 0;
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

        protected abstract void toolKilledHandler(object sender, KillingEventArgs e);

        protected abstract void toolMovedHandler(object sender, ToolMovedEventArgs e);

        private KeyValuePair<BoardPosition, ITool>[] getInitialBoardArrangement(GameDirection direction, Color color)
        {
            IList<KeyValuePair<BoardPosition, ITool>> pawnList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> rookList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> bishopList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> knightList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> queenKingList = new List<KeyValuePair<BoardPosition, ITool>>();

            pawnList      = GameInitHelper.GeneratePawns(direction, color);
            rookList      = GameInitHelper.GenerateRooks(direction, color);
            bishopList    = GameInitHelper.GenerateBishops(direction, color);
            knightList    = GameInitHelper.GenerateKnights(direction, color);
            queenKingList = GameInitHelper.GenerateQueenKing(direction, color);

            IList<KeyValuePair<BoardPosition, ITool>> toolsList = conctanteLists(pawnList, rookList, bishopList, knightList, queenKingList);

            return toolsList.ToArray();
        }

        private IList<KeyValuePair<BoardPosition, ITool>> conctanteLists(params IList<KeyValuePair<BoardPosition, ITool>>[] lists)
        {
            IList<KeyValuePair<BoardPosition, ITool>> toolsList = new List<KeyValuePair<BoardPosition, ITool>>();

            foreach (IList<KeyValuePair<BoardPosition,ITool>> list in lists)
            {
                toolsList = toolsList.Concat(list).ToList();
            }

            return toolsList;
        }

        protected virtual void OnToolMovedEvent(ToolMovedEventArgs e)
        {
            ToolMovedEvent?.Invoke(this, e);
        }

        protected virtual void OnToolKilledEvent(KillingEventArgs e)
        {
            ToolKilledEvent?.Invoke(this, e);
        }

        protected virtual void OnCheckmateEvent()
        {
            CheckmateEvent?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnPromotionEvent(PromotionEventArgs e)
        {
            ITool newTool = PromotionEvent?.Invoke(this, e);
            m_gameBoard.Remove(e.ToolPosition);
            m_gameBoard.Add(e.ToolPosition, newTool);
        }
    }

}
