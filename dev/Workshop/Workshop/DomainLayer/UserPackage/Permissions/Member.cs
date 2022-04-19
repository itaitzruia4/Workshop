using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    class Member : User
    {
        private string _username;
        public string Username { get; }

        private string _password;
        public string Password { get; }

        public Member(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
