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

        protected Team[] m_teams;
        protected                 int    m_currentTeamIndex;
        protected static readonly int    s_teamsAmount = 2;
        public                    Team   CurrentTeamTurn => m_teams[m_currentTeamIndex];

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

        public void StartGame(Team firstTeam, Team secondTeam)
        {
            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = getInitialBoardArrangement(firstTeam);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = getInitialBoardArrangement(secondTeam);

            foreach (var pair in whiteGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }

            foreach (var pair in blackGroupBoardArrangement)
            {
                m_gameBoard.Add(pair.Key, pair.Value);
            }

            m_teams            = new[] { firstTeam, secondTeam };
            m_currentTeamIndex = 0;
        }
        
        public bool TryGetTool(BoardPosition position, out ITool tool)
        {
            return m_gameBoard.TryGetTool(position, out tool);
        }
        
        protected void switchCurrentTeam()
        {
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
        }

        protected abstract void toolKilledHandler(object sender, KillingEventArgs e);

        protected abstract void toolMovedHandler(object sender, ToolMovedEventArgs e);

        private KeyValuePair<BoardPosition, ITool>[] getInitialBoardArrangement(Team team)
        {
            IList<KeyValuePair<BoardPosition, ITool>> pawnList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> rookList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> bishopList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> knightList = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> queenKingList = new List<KeyValuePair<BoardPosition, ITool>>();

            pawnList = GameInitHelper.GeneratePawns(team);
            rookList = GameInitHelper.GenerateRooks(team);
            bishopList = GameInitHelper.GenerateBishops(team);
            knightList = GameInitHelper.GenerateKnights(team);
            queenKingList = GameInitHelper.GenerateQueenKing(team);

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
