using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum GameDirection
    {
        Forward,
        Backward
    }
    
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

}
