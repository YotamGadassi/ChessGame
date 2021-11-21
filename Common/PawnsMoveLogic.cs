using Common;
using System.Windows;

namespace Server
{
    public static class PawnsMoveLogic
    {
        public static bool IsMoveLegal(Move Move, MoveState MoveState)
        {
            Vector diff = Move.Diff;

            if (MoveState.MoveDirection == GameDirection.Forward)
            {
                return logicForMoveForward(diff, MoveState.IsFirstMove, MoveState.IsMoveToKill);
            }
            else
            {
                return logicForMoveBackward(diff, MoveState.IsFirstMove, MoveState.IsMoveToKill);
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

            return false;
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

            return false;
        }
    }
}
