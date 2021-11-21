using System;
using Common;

namespace Server
{
    public class Player : IPlayer
    {
        public Player(string _Name)
        {
            Name = _Name;
            Token = new PlayerToken();
        }

        public string Name { get; } 

        public int Rank { get; private set; }

        public PlayerToken Token { get; }
    }
}
