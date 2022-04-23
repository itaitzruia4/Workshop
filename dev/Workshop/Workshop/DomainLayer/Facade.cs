using System.Collections.Generic;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;

namespace Workshop.DomainLayer
{
    public class Facade
    {
        private IUserController UserController;
        private IMarketController MarketController;

        internal Facade()
        {
            UserController = new UserController(new HashSecurityHandler(), new ReviewHandler());
            MarketController = new MarketController(UserController);
        }

        public User EnterMarket()
        {
            return UserController.EnterMarket();
        }

        public void ExitMarket()
        {
            UserController.ExitMarket();
        }

        public Member Login(string username, string password)
        {
            return UserController.Login(username, password);
        }

        public void Logout(string username)
        {
            UserController.Logout(username);
        }

        public void Register(string username, string password)
        {
            UserController.Register(username, password);
        }

        internal Product AddProduct(string username, int storeId, int productId, string productName, string description, double price, int quantity)
        {
            return MarketController.AddProductToStore(username, storeId, productId, productName, description, price, quantity);
        }

        internal StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId)
        {
            return MarketController.NominateStoreManager(nominatedUsername, nominatedUsername, storeId);
        }

        internal StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            return MarketController.NominateStoreOwner(nominatedUsername, nominatedUsername, storeId);
        }

        internal List<Member> GetWorkersInformation(string username, int storeId)
        {
            return MarketController.GetWorkersInformation(username, storeId);
        }
        internal void CloseStore(string username, int storeId)
        {
            MarketController.CloseStore(username, storeId);
        }

        internal int CreateNewStore(string creator, string storeName)
        {
            return MarketController.CreateNewStore(creator, storeName);
        }

        internal void ReviewProduct(string user, int productId, string review)
        {
            UserController.ReviewProduct(user, productId, review);
        }
    }
}