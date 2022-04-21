using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class Member : User
    {
        internal string Username { get; }

        internal string Password { get; }

        public Member(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
