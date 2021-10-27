using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ITool
    {
        bool IsMovingLegal(BoardPosition start, BoardPosition End);

        Point[] PossibleMoves(BoardPosition Start);

        Guid Key
        {
            get;
        }
    }
}
