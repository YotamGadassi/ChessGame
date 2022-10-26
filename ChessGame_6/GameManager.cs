using System.ComponentModel;
using System.Windows.Media;
using Board;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using Tools;

namespace ChessGame
{
    public abstract class BaseGameManager : IGameManager
    {
        protected ChessBoard m_gameBoard;

        // public event EventHandler<ChessBoardEventArgs> CheckEvent;
        public event EventHandler<CheckmateEventArgs> CheckmateEvent;
        public event EventHandler<EventArgs>          EndGameEvent;
        public event EventHandler<EventArgs>          StartGameEvent;
        public event EventHandler<ToolMovedEventArgs> ToolMovedEvent;
        public event EventHandler<KillingEventArgs>   ToolKilledEvent;
        public event PromotionEventHandler            PromotionEvent;
        public event EventHandler<Color>              TeamSwitchEvent;

        public Color                                  CurrentColorTurn => m_teams[m_currentTeamIndex];

        protected                 Color[]? m_teams = { Colors.White, Colors.Black };
        protected                 int      m_currentTeamIndex;
        protected static readonly int      s_teamsAmount = 2;

        public bool IsGameRunning { get; private set; }

        protected BaseGameManager()
        {
            IsGameRunning = false;
            m_gameBoard     = new ChessBoard();
        }
        
        public MoveResult Move(BoardPosition start, BoardPosition end)
        {
            MoveResult result = m_gameBoard.Move(start, end);
            switch (result.Result)
            {
                case MoveResultEnum.NoChangeOccurred:
                {
                    break;
                }
                case MoveResultEnum.ToolKilled:
                {
                    ITool toolKilled  = result.ToolAtEnd;
                    bool  isCheckmate = toolKilled is King;
                    if (isCheckmate)
                    {
                        OnCheckmateEvent(new CheckmateEventArgs(toolKilled.Color, result.EndPosition, result.InitialPosition));
                        break;
                    }

                    toolKilledHandler(new KillingEventArgs(result.ToolAtInitial, start, end, result.ToolAtEnd));
                    if (isPromotion(result))
                    {
                        OnPromotionEvent(new PromotionEventArgs(result.ToolAtInitial, result.EndPosition));
                    }
                    switchCurrentTeam();
                    break;
                }
                case MoveResultEnum.ToolMoved:
                {
                    toolMovedHandler(new ToolMovedEventArgs(result.ToolAtInitial, start, end));
                    if (isPromotion(result))
                    {
                        OnPromotionEvent(new PromotionEventArgs(result.ToolAtInitial, result.EndPosition));
                    }
                    switchCurrentTeam();
                    break;
                }
                default:
                {
                    throw new InvalidEnumArgumentException($"The enum value {result.Result} is not known.");
                }
            }

            return result;
        }

        private bool isPromotion(MoveResult result)
        {
            ITool         toolMoved   = result.ToolAtInitial;
            BoardPosition endPosition = result.EndPosition;
            return toolMoved is Pawn && isLastRow(toolMoved, endPosition);
        }

        private bool isLastRow(ITool         movedTool
                             , BoardPosition endPosition)
        {
            return movedTool.Color == Colors.White && endPosition.Row == 8
                || movedTool.Color == Colors.Black && endPosition.Row == 1;
        }

        public void EndGame()
        {
            IsGameRunning = false;
            m_gameBoard.ClearBoard();
            m_teams            = null;
            m_currentTeamIndex = 0;
            EndGameEvent?.Invoke(this, EventArgs.Empty);
        }

        public void StartGame()
        {
            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = getInitialBoardArrangement(GameDirection.North, Colors.White);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = getInitialBoardArrangement(GameDirection.South, Colors.Black);

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

        protected virtual void OnCheckmateEvent(CheckmateEventArgs args)
        {
            CheckmateEvent?.Invoke(this, args);
        }
        
        protected async void OnPromotionEvent(PromotionEventArgs e)
        {
            ITool newTool = await PromotionEvent?.Invoke(this, e);
            m_gameBoard.Remove(e.ToolPosition);
            m_gameBoard.Add(e.ToolPosition, newTool);
        }
    }

}
