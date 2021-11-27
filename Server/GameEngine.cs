using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using ChessBoard;

namespace Game
{
    public class GameEngine : IDisposable
    {
        private Board m_gameBoard;
        private Dictionary<Team, IList<ITool>> m_teamToolsDict;

        private GameHelper gameHelper;

        public event EventHandler<ChessBoardEventArgs> StateChangeEvent;

        public Team CurrentTeamTurn { get; private set; }

        public GameEngine(Team FirstTeam, Team SecondTeam)
        {
            m_gameBoard = new Board();
            m_gameBoard.StateChangeEvent += StateChanged;

            m_teamToolsDict = new Dictionary<Team, IList<ITool>>();
            m_teamToolsDict[FirstTeam] = new List<ITool>();
            m_teamToolsDict[SecondTeam] = new List<ITool>();

            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = getInitialBoardArrangement(FirstTeam);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = getInitialBoardArrangement(SecondTeam);

            m_gameBoard.ForceAdd(whiteGroupBoardArrangement);
            m_gameBoard.ForceAdd(blackGroupBoardArrangement);

            gameHelper = new GameHelper(m_gameBoard);

            CurrentTeamTurn = FirstTeam;
        }

        public bool Move(BoardPosition Start, BoardPosition End, Team team)
        {
            bool isTeamTurn = CurrentTeamTurn == team;
            if(!isTeamTurn)
            {
                return false;
            }

            return Move(Start, End);
        }

        public bool Move(BoardPosition Start, BoardPosition End)
        {
            bool isMoveOk = gameHelper.IsMoveLegal(Start, End);
            if (isMoveOk)
            {
                ITool tool = m_gameBoard.GetTool(Start);
                m_gameBoard.ForceMove(Start, End);
                gameHelper.ReportMovingTool(tool);
                SwitchTeams();
            }

            return isMoveOk;
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
            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            list = GeneratePawns(team);

            foreach (KeyValuePair<BoardPosition, ITool> pair in list)
            {
                m_teamToolsDict[team].Add(pair.Value);
            }

            return list.ToArray();
        }

        private IList<KeyValuePair<BoardPosition, ITool>> GeneratePawns(Team team)
        {
            int pawnsAmount = 8;

            int yAxis = default;
            switch(team.MoveDirection)
            {
                case GameDirection.Forward:
                    yAxis = 1;
                    break;
                case GameDirection.Backward:
                    yAxis = 6;
                    break;
            }

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            for (int i = 0; i < pawnsAmount; ++i)
            {
                BoardPosition newPosition = new BoardPosition(i, yAxis);
                Pawn newPawn = new Pawn(newPosition, team.Color, team.MoveDirection);
                KeyValuePair<BoardPosition, ITool> newPair = new KeyValuePair<BoardPosition, ITool>(newPosition, newPawn);
                list.Add(newPair);
            }

            return list;
        }

        public void Dispose()
        {
            m_gameBoard.StateChangeEvent -= StateChanged;
        }
    }

}
