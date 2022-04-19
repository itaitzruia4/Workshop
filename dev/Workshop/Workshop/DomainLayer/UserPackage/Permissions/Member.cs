using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class Member : User
    {
        private string _username;
        internal string Username { get; }

        private string _password;
        internal string Password { get; }

        public Member(string username, string password)
        {
            this._username = username;
            this._password = password;
        }
    }
}
