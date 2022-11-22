using System.Collections;
using System.ComponentModel;
using System.Windows.Media;
using Board;
using Common;
using Common.ChessBoardEventArgs;
using Common_6;
using Tools;

namespace ChessGame
{
    public class OfflineGameManager : IGameManager
    {
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
        
        protected async void OnPromotionEvent(PromotionEventArgs e)
        {
            ITool newTool = await PromotionEvent?.Invoke(this, e);
            m_gameBoard.Remove(e.ToolPosition);
            m_gameBoard.Add(e.ToolPosition, newTool);
            OnToolPromotedEvent(new ToolPromotedEventArgs(e.ToolToPromote, newTool, e.ToolPosition));
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
