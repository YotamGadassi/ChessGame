using Utils;

namespace ChessServer.Users
{
    public class UserUniqueId : BaseId
    {
        public static UserUniqueId NewUniqueId() => new(Guid.NewGuid());

        private UserUniqueId(Guid          id) : base(id) { }

        protected bool Equals(UserUniqueId other)
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

            return Equals((UserUniqueId)obj);
        }
    }
}
