namespace ChessServer.Users
{
    public class UserData
    {
        public UserUniqueId UserId { get; }

        public string UserName { get; }

        public UserData(UserUniqueId userId
                       , string      userName)
        {
            UserId   = userId;
            UserName = userName;
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(UserName)}: {UserName}";
        }
    }
}
