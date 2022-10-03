using System;

namespace ClientWebServerCommon
{
    public class User
    {
        public string Name { get; set; }

        public Guid Token { get; set; }

        // for serialization
        public User() { }
        public User(string name, Guid token)
        {
            Name = name;
            Token = token;
        }
    }
}
