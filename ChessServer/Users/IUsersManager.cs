namespace ChessServer.Users
{
    public interface IUsersManager<in T>
    {
        public Task AddNewUserAsync(T userIdentifier, UserData userData);

        public Task<UserData?> RemoveUserAsync(T userIdentifier);

        public Task<UserData> GetUserDataAsync(T userIdentifier);
    }
}
