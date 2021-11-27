using Common;

namespace ChessBoard
{
    public class MoveState
    {
        public GameDirection MoveDirection { get; }

        public bool IsFirstMove { get; }

        public bool IsMoveToKill { get; }

        public MoveState(GameDirection MoveDirection_, bool IsFirstMove_, bool IsMoveToKill_)
        {
            MoveDirection = MoveDirection_;
            IsFirstMove = IsFirstMove_;
            IsMoveToKill = IsMoveToKill_;
        }
    }
}
