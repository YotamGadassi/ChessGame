using Common;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utils;

namespace Server
{
    /// <summary>
    /// A singleton of the server engine.
    /// Both clients should get its instance and Register the Host client should Init and start the game when the server is ready play.
    /// </summary>
    public class ChessGameServer
    {
        internal class itoolTeamDictionary
        {
            private Dictionary<ITool, Team> toolTeamDict;

            public itoolTeamDictionary(Dictionary<ITool, Team> dict)
            {
                toolTeamDict = dict;
            }

            public itoolTeamDictionary()
            {
                toolTeamDict = new Dictionary<ITool, Team>();
            }

            public IList<ITool> this[Team team]
            {
                get
                {
                    List<ITool> tools = new List<ITool>();
                    foreach(KeyValuePair<ITool, Team> pair in toolTeamDict)
                    {
                        if (pair.Value == team)
                            tools.Add(pair.Key);
                    }
                    return tools;
                }
                set
                {
                    IList<ITool> tools = value;

                    foreach (KeyValuePair<ITool, Team> pair in toolTeamDict)
                    {
                        if (pair.Value == team)
                            toolTeamDict.Remove(pair.Key);
                    }

                    foreach (ITool tool in tools)
                    {
                        toolTeamDict[team] = tool;
                    }
                }
            }

            public Team this[ITool tool]
            {
                get
                {
                   return toolTeamDict[tool];
                }
                set
                {
                    toolTeamDict[tool] = value;
                }
            }

    }

        public static ChessGameServer instance;
        public bool isInitilized = false;


        private ChessBoard gameBoard = new ChessBoard();

        private Dictionary<string, IPlayer> playerNameToPlayer = new Dictionary<string, IPlayer>();
        internal BiDirectionalDictionary<IPlayer, Team> playerToTeam = new BiDirectionalDictionary<IPlayer, Team>();
        internal BiDirectionalDictionary<Team, GameDirection> TeamToGameDirection = new BiDirectionalDictionary<Team, GameDirection>();
        internal itoolTeamDictionary toolToTeam = new itoolTeamDictionary();

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

        static ChessGameServer()
        {
            instance = new ChessGameServer();
        }

        public void Init()
        {
            KeyValuePair<BoardPosition, ITool>[] whiteGroupBoardArrangement = getInitialBoardArrangement(GameDirection.Forward, Team.White);
            KeyValuePair<BoardPosition, ITool>[] blackGroupBoardArrangement = getInitialBoardArrangement(GameDirection.Backward, Team.Black);

            List<KeyValuePair<BoardPosition, ITool>[]> boardArrangement = new List<KeyValuePair<BoardPosition, ITool>[]>();
            boardArrangement.Add(whiteGroupBoardArrangement);
            boardArrangement.Add(blackGroupBoardArrangement);

            gameBoard.Init(boardArrangement);

            TeamToGameDirection.Add(Team.White, GameDirection.Forward);
            TeamToGameDirection.Add(Team.Black, GameDirection.Backward);

            CurrentStatus = GameStatus.NonActive;
            isInitilized = true;
        }

        private void Start(GameMode _Mode)
        {
            bool gameCanBegin = IsServerReadyToPlay();
            if (!gameCanBegin)
                return;

            Mode = _Mode;

            startTimer();
            CurrentPlayer = chooseFirstPlayer();
            CurrentStatus = GameStatus.Active;
        }

        private IPlayer chooseFirstPlayer()
        {
            return playerToTeam.GetValue(Team.White);
        }

        private void startTimer()
        {
            if (timer == null)
                timer = new Timer(updateTimeElpased, null, 0, 1000);
            else
                timer.Change(0, 1000);
        }

        public bool IsServerReadyToPlay()
        {
            if (isInitilized && playerNameToPlayer.Count == minimumPlayers)
            {
                return true;
            }

            return false;
        }

        public void Stop()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            CurrentStatus = GameStatus.NonActive;
            isInitilized = false;
            gameBoard.ClearBoard();
        }           

        public void Pause()
        {
            if (CurrentStatus != GameStatus.Active)
                return;

            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            CurrentStatus = GameStatus.Paused;
        }

        public void Play()
        {
            if (CurrentStatus != GameStatus.Paused)
                return;

            CurrentStatus = GameStatus.Active;
            startTimer();
        }

        private void updateTimeElpased(object state)
        {
            TimeElpased += TimeSpan.FromSeconds(1);
        }

        public bool RegisterPlayer(IPlayer Player, Team team)
        {
            bool isRegistrationValid = validatePlayerRegistration(Player, team);
            if (!isRegistrationValid)
                return false;

            playerNameToPlayer.Add(Player.Name, Player);
            playerToTeam.Add(Player, team);

            return true;
        }

        public IMovementController GetController(Player player)
        {
            if (false == isInitilized)
                return null;
            
            IMovementController controller = new MovementController(player, gameBoard, this);

            return controller;
        }

        private bool validatePlayerRegistration(IPlayer player, Team team)
        {
            bool isReachPlayersLimit = playerToTeam.Count == playersLimit;
            if (isReachPlayersLimit)
                return false;

            bool isPlayerAlreadyRegisterd = playerToTeam.Contains(player);
            if (isPlayerAlreadyRegisterd)
                return false;

            bool isTeamOccupied = playerToTeam.Contains(team);
            if (isTeamOccupied)
                return false;

            return true;
        }

        //TODO: Implement
        private KeyValuePair<BoardPosition, ITool>[] getInitialBoardArrangement(GameDirection direction, Team team)
        {
            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            list.Concat(GeneratePawns(direction, team));

            return list.ToArray();
        }

        private IList<KeyValuePair<BoardPosition, ITool>> GeneratePawns(GameDirection direction, Team team)
        {
            int pawnsAmount = 8;

            int yAxis = 1;
            
            if (direction == GameDirection.Backward)
                yAxis = 6;

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            for (int i = 0; i < pawnsAmount; ++i)
            {
                ToolPawn newTool = new ToolPawn();
                Point newPoint = new Point(i, yAxis);
                BoardPosition newPosition = new BoardPosition(newPoint);
                KeyValuePair<BoardPosition, ITool> newPair = new KeyValuePair<BoardPosition, ITool>(newPosition, newTool);
                list.Add(newPair);
                toolToTeam[newTool] = team;
            }

            return list;
        }

    }


}
