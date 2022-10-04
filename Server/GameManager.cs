using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using ChessBoard.ChessBoardEventArgs;
using Common;
using Game;

namespace ChessGame
{
    public class GameManager : IDisposable
    {
        private ChessBoard.ChessBoard          m_gameBoard;

        // public event EventHandler<ChessBoardEventArgs> CheckEvent;
        public event EventHandler<EventArgs>          CheckmateEvent;
        public event EventHandler<EventArgs>          EndGameEvent;
        public event EventHandler<EventArgs>          StartGameEvent;
        public event EventHandler<ToolMovedEventArgs> ToolMovedEvent;
        public Team[]                                 m_teams;

        public Team                                   CurrentTeamTurn { get; private set; }

        public GameManager(Team[] teams)
        {
            m_gameBoard                =  new ChessBoard.ChessBoard();
            m_gameBoard.ToolMovedEvent += toolMovedHandler;
            m_teams                    =  teams;
        }

        private void toolMovedHandler(object sender, ToolMovedEventArgs e)
        {
            SwitchTeams();
        }

        public bool Move(BoardPosition start, BoardPosition end)
        {
            return m_gameBoard.Move(start, end);
        }

        public void EndGame()
        {
            m_gameBoard.ClearBoard();
            EndGameEvent?.Invoke(this, null);
            m_gameBoard.StateChangeEvent -= StateChanged;

            m_teamToolsDict.Clear();
            gameHelper = null;
            CurrentTeamTurn = null;
        }

        public void StartGame(Team FirstTeam, Team SecondTeam)
        {
            m_gameBoard.StateChangeEvent += StateChanged;
            
            m_teamToolsDict = new Dictionary<Team, IList<ITool>>();
            m_teamToolsDict[FirstTeam] = new List<ITool>();
            m_teamToolsDict[SecondTeam] = new List<ITool>();

            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = getInitialBoardArrangement(FirstTeam);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = getInitialBoardArrangement(SecondTeam);

            m_gameBoard.Add(whiteGroupBoardArrangement);
            m_gameBoard.Add(blackGroupBoardArrangement);

            IEnumerable<Team> teams = m_teamToolsDict.Keys;
            gameHelper = new GameMoveHelper(m_gameBoard, teams);
            CurrentTeamTurn = FirstTeam;
            StartGameEvent?.Invoke(this, null);
        }
        
        public BoardState GetBoardState()
        {
            return m_gameBoard.GetStateCopy();
        }
        
        private void switchCurrentTeam()
        {
            Team firstTeam = teams.First();
            Team secondTeam = teams.Last();
            
            if(CurrentTeamTurn == firstTeam)
            {
                CurrentTeamTurn = secondTeam;
            }
            else
            {
                CurrentTeamTurn = firstTeam;
            }
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

            foreach (KeyValuePair<BoardPosition, ITool> pair in toolsList)
            {
                m_teamToolsDict[team].Add(pair.Value);
            }

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

        public void Dispose()
        {
            m_gameBoard.StateChangeEvent -= StateChanged;
        }
    }

}
