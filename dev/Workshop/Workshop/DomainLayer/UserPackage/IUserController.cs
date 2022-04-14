using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;

namespace Workshop.DomainLayer.UserPackage
{
    interface IUserController
    {
        void Register(string username, string password);

        Member Login(string username, string password);

        User EnterMarket();
    }
}
