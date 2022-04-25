using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Shopping;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.UserPackage
{
    public interface IUserController
    {
        void InitializeSystem();
        User EnterMarket();
        void ExitMarket();
        void Register(string username, string password);
        bool IsMember(string username);
        Member GetMember(string username);
        Member Login(string username, string password);
        void Logout(string username);
        StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);
        StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId);
        bool IsAuthorized(string username, int storeId, Action action);
        void AssertCurrentUser(string username);
        List<Member> GetWorkers(int storeId);
        ReviewDTO ReviewProduct(string user, int productId, string review);
        ShoppingBagProduct addToCart(string user, int productId, int storeId, int quantity);
        ShoppingCartDTO viewCart(string user);
        void editCart(string user, int productId, int newQuantity);
    }
}
