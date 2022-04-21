using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.UserPackage
{
    interface IUserController
    {
        void InitializeSystem();
        User EnterMarket();
        void Register(string username, string password);
        Member Login(string username, string password);
        void Logout(string username);
        StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);
        StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId);
        bool IsAuthorized(string username, int storeId, Action action);
    }
}
