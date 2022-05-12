using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Shopping;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.UserPackage
{
    public interface IUserController
    {
        void InitializeSystem();
        User EnterMarket(int userId);
        void ExitMarket(int userId);
        void Register(int userId, string username, string password);
        bool IsMember(string username);
        Member GetMember(string username);
        Member Login(int userId, string username, string password);
        void Logout(int userId, string username);
        StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId);
        StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId);
        bool IsAuthorized(string username, int storeId, Action action);
        void AssertCurrentUser(int userId, string username);
        List<Member> GetWorkers(int storeId);
        ReviewDTO ReviewProduct(int userId, string user, int productId, string review, int rating);
        ShoppingBagProduct addToCart(int userId, string user, ShoppingBagProduct shoppingBagProduct, int storeId);
        ShoppingCartDTO viewCart(int userId, string user);
        void AddStoreFounder(string username, int storeId);
        void AddOrder(int userId, OrderDTO order, string username);
        ShoppingCartDTO editCart(int userId, string user, int productId, int newQuantity);
        void ClearUserCart(int userId);
    }
}
