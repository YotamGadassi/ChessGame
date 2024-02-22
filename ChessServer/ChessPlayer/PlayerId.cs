using Utils;

namespace ChessServer.ChessPlayer
{
    public class PlayerId : BaseId
    {
        public static PlayerId NewPlayerId() => new PlayerId(Guid.NewGuid());
        private PlayerId(Guid              id) : base(id) { }

        protected bool Equals(PlayerId other)
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

            return Equals((PlayerId)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
