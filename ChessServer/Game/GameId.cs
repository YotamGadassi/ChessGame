using Utils;

namespace ChessServer.Game
{
    public class GameId : BaseId
    {
        public static GameId NewGameId() => new(Guid.NewGuid());
        
        private GameId(Guid                id) : base(id) { }

        protected bool Equals(GameId other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((GameId)obj);
        }
    }
}
