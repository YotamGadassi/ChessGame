using Common;
using System.Windows;

namespace Server
{
    public static class PawnsMoveLogic
    {
        public static bool IsMoveLegal(Move Move, bool MoveForward , bool IsFirstMove, bool IsKilling)
        {
            Vector diff = Move.Diff;

            if (MoveForward)
            {
                return logicForMoveForward(diff, IsFirstMove, IsKilling);
            }
            else
            {
                logicForMoveBackward(diff, IsFirstMove, IsKilling);
            }

        }

        private static bool logicForMoveForward(Vector diff, bool isFirstMove, bool isKilling)
        {
            if (diff.X == 0 && diff.Y == 1)
            {
                return true;
            }

            if (isKilling)
            {
                if(diff.X == 1 && diff.Y == 1)
                {
                    return true;
                }
            }

            if (isFirstMove)
            {
                if(diff.X == 0 && diff.Y == 2)
                {
                    return true;                    
                }
            }
        }

        private static bool logicForMoveBackward(Vector diff, bool isFirstMove, bool isKilling)
        {
            if (diff.X == 0 && diff.Y == -1)
            {
                return true;
            }

            if (isKilling)
            {
                if(diff.X == -1 && diff.Y == -1)
                {
                    return true;
                }
            }

            if (isFirstMove)
            {
                if(diff.X == 0 && diff.Y == -2)
                {
                    return true;                    
                }
            }
        }
    }
}
