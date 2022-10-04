using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using Common;
using Game;

namespace ChessGame
{
    public class GameEngine : IDisposable
    {
        private ChessBoard.ChessBoard          m_gameBoard;

        // public event EventHandler<ChessBoardEventArgs> CheckEvent;
        public event EventHandler<EventArgs> CheckmateEvent;
        public event EventHandler<EventArgs> EndGameEvent;
        public event EventHandler<EventArgs> StartGameEvent;
        public Team CurrentTeamTurn { get; private set; }

        public GameEngine()
        {
            m_gameBoard = new ChessBoard.ChessBoard();  
        }

        public bool Move(BoardPosition start, BoardPosition end)
        {
            bool isMoveOk = gameHelper.IsMoveLegal(start, end);
            if (isMoveOk)
            {
                ITool toolToMove = m_gameBoard.GetTool(start);
                ITool toolAtEnd = m_gameBoard.GetTool(end);
                if(toolAtEnd != null && toolAtEnd.Type == "King")
                {
                    CheckmateEvent?.Invoke(this, null);
                }
                m_gameBoard.Move(start, end);
                gameHelper.ReportMovingTool(toolToMove);
                SwitchTeams();
            }

            return isMoveOk;
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
        
        private void StateChanged(object sender, ChessBoardEventArgs args)
        {
            StateChangeEvent?.Invoke(sender, args);
        }

        private void SwitchTeams()
        {
            IEnumerable<Team> teams = m_teamToolsDict.Keys;

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
