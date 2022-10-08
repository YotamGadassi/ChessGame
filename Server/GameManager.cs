using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ChessBoard;
using ChessBoard.ChessBoardEventArgs;
using Common;
using Tools;

namespace ChessGame
{
    public class GameManager
    {
        private ChessBoard.ChessBoard          m_gameBoard;

        // public event EventHandler<ChessBoardEventArgs> CheckEvent;
        public event EventHandler<EventArgs>          CheckmateEvent;
        public event EventHandler<EventArgs>          EndGameEvent;
        public event EventHandler<EventArgs>          StartGameEvent;
        public event EventHandler<ToolMovedEventArgs> ToolMovedEvent;
        public event EventHandler<KillingEventArgs>   ToolKilledEvent;

        public event Func<object, ToolMovedEventArgs, ITool> PromotionEvent; 
        private                 Team[]                       m_teams;
        private                 int                          m_currentTeamIndex;
        private static readonly int                          s_teamsAmount = 2;
        public                  Team                         CurrentTeamTurn => m_teams[m_currentTeamIndex];


        public GameManager()
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
        
        private void switchCurrentTeam()
        {
            m_currentTeamIndex = (m_currentTeamIndex + 1) % s_teamsAmount;
        }

        private void toolKilledHandler(object sender, KillingEventArgs e)
        {
            // need to handle:
            //1. king killed - CheckMate
            ITool killedTool = e.KilledTool;
            if (killedTool is King)
            {
                CheckmateEvent?.Invoke(this, e);
                return;
            }
            //2. promotion
            ITool movedTool = e.MovedTool;
            if (movedTool is Pawn)
            {
                if ((movedTool.Color == Colors.White && e.EndPosition.Row == 7)
                 || (movedTool.Color == Colors.Black && e.EndPosition.Row == 0))
                {
                    ITool chosenToolAfterPromotion = PromotionEvent?.Invoke(this, e);
                    //TODO: handle the swap
                    return;
                }
            }

            ToolKilledEvent?.Invoke(this, e);
        }

        private void toolMovedHandler(object sender, ToolMovedEventArgs e)
        {
            switchCurrentTeam();
            // need to handle:
            // handle promotion
            ITool movedTool = e.MovedTool;
            if (movedTool is Pawn)
            {
                if ((movedTool.Color == Colors.White && e.EndPosition.Row == 7)
                    || (movedTool.Color == Colors.Black && e.EndPosition.Row == 0))
                {
                    ITool chosenToolAfterPromotion = PromotionEvent?.Invoke(this, e);
                    //TODO: handle the swap
                    return;
                }
            }

            //TODO: handle check for check
            ToolMovedEvent?.Invoke(this, e);
        }

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

    }

}
