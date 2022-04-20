using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    class Member : User
    {
        private string username;
        private string password;

        private List<Role> roles;

        public Member(string username, string password)
        {
            this.username = username;
            this.password = password;
            roles = new List<Role>();
        }

        internal string GetPassword()
        {
            return password;
        }

        internal string GetUsername()
        {
            return username;
        }
        public Boolean IsAuthorized(int storeID, Action action)
        {
            foreach (Role role in roles) 
            {
                if(role.IsAuthorized(storeID, action))
                    return true;
            }
            return false;
        }
    }
}
