namespace ChessServer.Users
{
    public interface IUsersManager<T>
    {
        public Task AddNewUserAsync(T userIdentifier, UserData<T> userData);

        public Task<UserData<T>> RemoveUserAsync(T userIdentifier);

        public Task<UserData<T>> GetUserDataAsync(T userIdentifier);
    }
}
