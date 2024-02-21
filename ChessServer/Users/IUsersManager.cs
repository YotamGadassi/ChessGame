namespace ChessServer.Users
{
    public interface IUsersManager<in T>
    {
        public void AddNewUser(T userIdentifier, UserData userData);

        public UserData RemoveUser(T userIdentifier);

        public UserData GetUserData(T userIdentifier);
    }
}
