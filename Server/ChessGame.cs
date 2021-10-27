using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    public class ChessGame
    {
        private Board gameBoard = new Board();
        private Dictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
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

            Mode = _Mode;
            playersArray = players.Values.ToArray();
            if (timer != null)
                timer = new Timer(updateTimeElpased, null, 0, 1000);
            else
                timer.Change(0, 1000);
            Random rand = new Random();
            int randonNumber = rand.Next(0, players.Count());

            CurrentPlayer = playersArray[randonNumber];
            CurrentStatus = GameStatus.Active;


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
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            CurrentStatus = GameStatus.NonActive;
            gameBoard.ClearBoard();
        }           

        public void Pause()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            CurrentStatus = GameStatus.Paused;
        }

        private void updateTimeElpased(object state)
        {
            TimeElpased += TimeSpan.FromSeconds(1);
        }

        public bool RegisterPlayer(IPlayer Player)
        {
            bool canAddPlayer = players.Count() <= playersLimit;
            if (!canAddPlayer)
                return false;

            players.Add(Player.Name, Player);

            return true;
        }

        public bool TurnPlay(BoardPosition start, BoardPosition end)
        {
            bool moveSuccess = gameBoard.MoveTool(start, end);

            return moveSuccess;
        }

        //TODO: Implement
        private KeyValuePair<BoardPosition, ITool>[] getInitialBoardArrangement()
        {
            throw new NotImplementedException();
        }
    }
}
