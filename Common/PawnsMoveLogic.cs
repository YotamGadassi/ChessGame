using Common;
using System.Windows;

namespace Server
{
    public static class PawnsMoveLogic
    {
        public static bool IsMoveLegal(Move Move, bool MoveForward , bool IsFirstMove , int ToolGroupNum, ITool ToolAtEndPoint)
        {
            Vector diff = Move.Diff;

            bool isKilling = ToolAtEndPoint != null;
            
            if (isKilling && (ToolGroupNum == ToolAtEndPoint.GroupNumber))
            {
                return false;
            }

            if (MoveForward)
            {
                return logicForMoveForward(diff, IsFirstMove, isKilling);
            }
            else
            {
                return logicForMoveBackward(diff, IsFirstMove, isKilling);
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
