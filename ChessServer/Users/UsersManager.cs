﻿using System.Collections.Concurrent;
using UserData = ChessServer.Users.UserData<string>;

namespace ChessServer.Users
{
    public class SignalRUsersManager : IUsersManager<string>
    {
        private readonly ConcurrentDictionary<string, UserData> m_concurrentDictionary;

        public SignalRUsersManager()
        {
            m_concurrentDictionary = new ConcurrentDictionary<string, UserData>();
        }

        public Task AddNewUserAsync(string   userIdentifier
                                  , UserData userData)
        {
            if (false == m_concurrentDictionary.TryAdd(userIdentifier, userData))
            {
                throw new ArgumentException($"UserIdentifier: [{userIdentifier}] already exist in manager");
            }

            return Task.CompletedTask;
        }

        public Task<UserData> RemoveUserAsync(string userIdentifier)
        {
            m_concurrentDictionary.TryRemove(userIdentifier, out UserData userData);
            return Task.FromResult(userData);
        }

        public Task<UserData> GetUserDataAsync(string userIdentifier)
        {
            if (false == m_concurrentDictionary.TryGetValue(userIdentifier, out UserData userData))
            {
                throw new ArgumentException($"UserIdentifier: [{userIdentifier}] does not exist in manager");
            }

            return Task.FromResult(userData);
        }
    }
}