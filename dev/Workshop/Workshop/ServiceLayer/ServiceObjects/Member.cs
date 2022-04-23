using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainMember = Workshop.DomainLayer.UserPackage.Permissions.Member;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Member : User
    {
        public readonly string Username;
        public IReadOnlyCollection<Role> roles;

        public Member(DomainMember DomainMember): base(DomainMember)
        {
            Username = DomainMember.Username;
        }
    }
}
