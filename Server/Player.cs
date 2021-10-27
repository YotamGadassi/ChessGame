using System;
using Common;

namespace Server
{
    public class Player : IPlayer
    {
        public Player(string _Name)
        {
            Name = _Name;
            Token = Guid.NewGuid();
        }

        public string Name { get; } 

        public int Rank { get; private set; }

        public Guid Token { get; }
    }
}
