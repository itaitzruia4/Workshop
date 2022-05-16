using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class MemberDTO
    {
        public string Username { get; }
        public string Password { get; }

        public List<Role> roles { get; }

        public MemberDTO(string username, string password, List<Role> Roles)
        {
            Username = username;
            Password = password;
            roles = Roles;
        }
    }
}
