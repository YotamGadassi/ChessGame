using Common;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Server
{
    public enum GameMode
    {
        Offline,
        Online
    }

    public enum GameStatus
    {
        Active,
        Paused,
        NonActive,
        NonInitialiezed
    }

    public enum Team
    {
        White,
        Black
    }

    public class ChessGame
    {
        private ChessBoard gameBoard = new ChessBoard();
        private Dictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
        private Dictionary<IPlayer, Team> playersGroupNum = new Dictionary<IPlayer, Team>();

        private IPlayer[] playersArray; 
        private int playersLimit = 2;
        private int minimumPlayers = 2;
        private Timer timer;
        public TimeSpan TimeElpased = TimeSpan.FromSeconds(0);

        public GameStatus CurrentStatus = GameStatus.NonInitialiezed;

        public GameMode Mode
        {
            get; private set;
        }

        public IPlayer CurrentPlayer
        {
            get; private set;
        }

        private int teamToGroupNum(Team team)
        {
            switch(team)
            {
                case Team.White:
                    return 1;
                case Team.Black:
                    return 2;
                default:
                    return 1;
            }
        }

        private Team groupNumToTeam(int GroupNum)
        {
            switch (GroupNum)
            {
                case 1:
                    return Team.White;
                case 2:
                    return Team.Black;
                default:
                    return Team.White;
            }
        }

        public void Init()
        {
            KeyValuePair<BoardPosition, ITool>[] boardArrangement = getInitialBoardArrangement();

            gameBoard.Init(boardArrangement);
            CurrentStatus = GameStatus.NonActive;
        }

        public void Start(GameMode _Mode)
        {
            bool gameCanBegin = IsGameCanBegin();
            if (!gameCanBegin)
                return;
            SplitToolsBetweenPlayers();
            Mode = _Mode;



            startTimer();
            CurrentPlayer = chooseRandomFirstPlayer();
            CurrentStatus = GameStatus.Active;
        }

        private void SplitToolsBetweenPlayers()
        {
            
        }

        private void startTimer()
        {
            if (timer == null)
                timer = new Timer(updateTimeElpased, null, 0, 1000);
            else
                timer.Change(0, 1000);
        }

        private IPlayer chooseRandomFirstPlayer()
        {
            Random rand = new Random();
            int randonNumber = rand.Next(0, players.Count());
            return playersArray[randonNumber];
        }

        private bool IsGameCanBegin()
        {
            if (players.Count == minimumPlayers)
            {
                return true;
            }

            return false;
        }

        public void Stop()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            CurrentStatus = GameStatus.NonActive;
            gameBoard.ClearBoard();
        }           

        public void Pause()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            CurrentStatus = GameStatus.Paused;
        }

        private void updateTimeElpased(object state)
        {
            TimeElpased += TimeSpan.FromSeconds(1);
        }

        public bool RegisterPlayer(IPlayer Player)
        {
            bool canAddPlayer = players.Count() < playersLimit;
            if (!canAddPlayer)
                return false;

            players.Add(Player.Name, Player);
            return true;
        }

        public ChessBoardProxy GenerateBoardProxy(Guid PlayerToken)
        {
            IPlayer currentPlayer = null;
            foreach (IPlayer player in playersArray)
            {
                if (player.Token == PlayerToken)
                {
                    currentPlayer = player;
                }
            }

            if (currentPlayer == null)
            {
                return null;
            }

            int groupNum = playersGroupNum[currentPlayer];

            ITool[] tools = gameBoard.GetToolsForGroup(groupNum).ToArray();
            bool isMoveForward = isMoveForward()

            ChessBoardProxy proxy = new ChessBoardProxy(gameBoard, tools,)  
        }

        //TODO: Implement
        private KeyValuePair<BoardPosition, ITool>[] getInitialBoardArrangement()
        {
            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            bool isMoveForward = true;
            foreach (IPlayer player in players.Values)
            {
                list.Concat(GeneratePawns(isMoveForward));
                isMoveForward = !isMoveForward;
            }

            return list.ToArray();
        }

        private IList<KeyValuePair<BoardPosition, ITool>> GeneratePawns(bool IsMoveForward)
        {
            int pawnsAmount = 8;

            int yAxis = 1;
            int groupNumer = 1;
            
            if (!IsMoveForward)
            {
                yAxis = 6;
                groupNumer = 2;
            }

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            for (int i = 0; i < pawnsAmount; ++i)
            {
                ToolPawn newTool = new ToolPawn(groupNumer);
                Point newPoint = new Point(i, yAxis);
                BoardPosition newPosition = new BoardPosition(newPoint);
                KeyValuePair<BoardPosition, ITool> newPair = new KeyValuePair<BoardPosition, ITool>(newPosition, newTool);
                list.Add(newPair);
            }

            return list;
        }

    }


}
