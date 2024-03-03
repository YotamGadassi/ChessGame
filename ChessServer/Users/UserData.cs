namespace ChessServer.Users
{
    public class UserData<T>
    {
        public UserUniqueId UserId { get; }

        public string UserName { get; }

        public T ConnectionId { get; }

        public UserData(T            conncetionId
                      , UserUniqueId userId
                      , string       userName)
        {
            ConnectionId = conncetionId;
            UserId       = userId;
            UserName     = userName;
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(UserName)}: {UserName}";
        }
    }
}
