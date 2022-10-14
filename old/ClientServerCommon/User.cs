using System;

namespace Client.Models
{
    public class User
    {
        public string Name { get; set; }

        public Guid UserToken { get; set; }

        public User() { }

        public User(string name, Guid token)
        {
            Name = name;
            UserToken = token;
        }
    }
}
