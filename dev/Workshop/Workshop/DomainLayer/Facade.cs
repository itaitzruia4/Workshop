using System;
using System.Collections.Generic;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.MarketPackage.Biding;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.ServiceLayer;
using Notification = Workshop.DomainLayer.UserPackage.Notifications.Notification;
using SystemAdminDTO = Workshop.ServiceLayer.ServiceObjects.SystemAdminDTO;
using DataHandler = Workshop.DataLayer.DataHandler;
using DALMarketController = Workshop.DataLayer.DataObjects.Controllers.MarketController;

namespace Workshop.DomainLayer
{
    public class Facade
    {
        private IUserController UserController;
        private IMarketController MarketController;
        internal Facade(IExternalSystem externalSystem, List<SystemAdminDTO> systemAdmins)
        {
            //DALMarketController market = DataHandler.Instance.Value.find<DALMarketController>(0);
            DALMarketController market = DataHandler.Instance.Value.loadMarket(0);
            if (market == null)
            {
                UserController = new UserController(new HashSecurityHandler(), new ReviewHandler(), systemAdmins);
                MarketController = new MarketController(UserController, externalSystem);
            }
            else
            {
                UserController = new UserController(market.userController, systemAdmins);
                MarketController = new MarketController(market, UserController, externalSystem);
            }
        }

        public User EnterMarket(int userId, DateTime date)
        {
            return UserController.EnterMarket(userId, date);
        }

        public void ExitMarket(int userId)
        {
            UserController.ExitMarket(userId);
        }

        public KeyValuePair<Member, List<Notification>> Login(int userId, string membername, string password, DateTime date)
        {
            return UserController.Login(userId, membername, password, date);
        }

        public void Logout(int userId, string membername)
        {
            UserController.Logout(userId, membername);
        }

        public void Register(int userId, string membername, string password, DateTime birthdate)
        {
            UserController.Register(userId, membername, password, birthdate);
        }

        internal Product AddProduct(int userId, string membername, int storeId, string productName, string description, double price, int quantity, string category)
        {
            return MarketController.AddProductToStore(userId, membername, storeId, productName, description, price, quantity, category);
        }

        internal StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
        {
            return MarketController.NominateStoreManager(userId, nominatorUsername, nominatedUsername, storeId, date);
        }

        internal StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
        {
            return MarketController.NominateStoreOwner(userId, nominatorUsername, nominatedUsername, storeId, date);
        }

        internal Member RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId)
        {
            return MarketController.RemoveStoreOwnerNomination(userId, nominatorMembername, nominatedMembername, storeId);
        }

        internal void AddActionToManager(int userId, string owner, string manager, int storeId, string action)
        {
            MarketController.AddActionToManager(userId, owner, manager, storeId, action);
        }

        internal List<Member> GetWorkersInformation(int userId, string membername, int storeId)
        {
            return MarketController.GetWorkersInformation(userId, membername, storeId);
        }
        internal void CloseStore(int userId, string username, int storeId)
        {
            MarketController.CloseStore(userId, username, storeId);
        }

        internal void OpenStore(int userId, string membername, int storeId)
        {
            MarketController.OpenStore(userId, membername, storeId);
        }

        internal Store CreateNewStore(int userId, string creator, string storeName, DateTime date)
        {
            return MarketController.CreateNewStore(userId, creator, storeName, date);
        }

        internal ReviewDTO ReviewProduct(int userId, string user, int productId, string review, int rating)
        {
            return UserController.ReviewProduct(userId, user, productId, review, rating);
        }
        internal ProductDTO GetProductInfo(int userId, string user, int productId)
        {
            return MarketController.getProductInfo(userId, user, productId);
        }
        internal StoreDTO GetStoreInfo(int userId, string user, int storeId)
        {
            return MarketController.getStoreInfo(userId, user, storeId);
        }
        internal List<ProductDTO> SearchProduct(int userId, string keyWords, string category, double minPrice, double maxPrice, double productReview)
        {
            return MarketController.SearchProduct(userId, keyWords, category, minPrice, maxPrice, productReview);
        }
        internal ShoppingBagProduct AddToCart(int userId, int productId, int storeId,int quantity)
        {
            return MarketController.AddToCart(userId, productId, storeId, quantity);
        }
        internal ShoppingCartDTO ViewCart(int userId)
        {
            return UserController.viewCart(userId);
        }
        internal ShoppingCartDTO EditCart(int userId, int productId, int newQuantity)
        {
            return MarketController.EditCart(userId, productId, newQuantity);
        }
        internal double BuyCart(int userId, CreditCard cc, SupplyAddress address, DateTime buyTime)
        {
            return MarketController.BuyCart(userId, cc, address, buyTime);
        }

        public void AddProductDiscount(int userId, string user, int storeId, string jsonDiscount, int productId)
        {
            MarketController.AddProductDiscount(userId, user, storeId,jsonDiscount,productId);
        }

        public void AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName)
        {
            MarketController.AddCategoryDiscount(userId, user, storeId,jsonDiscount,categoryName);
        }

        public void AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount)
        {
            MarketController.AddStoreDiscount(userId, user, storeId, jsonDiscount);
        }

        public void AddProducPurchaseTerm(int userId, string user, int storeId, string json_term, int product_id)
        {
            MarketController.AddProductPurchaseTerm(userId, user, storeId,json_term,product_id);
        }

        public void AddCategoryPurchaseTerm(int userId, string user, int storeId, string json_term, string category_name)
        {
            MarketController.AddCategoryPurchaseTerm(userId, user, storeId,json_term,category_name);
        }

        public void AddStorePurchaseTerm(int userId, string user, int storeId, string json_term)
        {
            MarketController.AddStorePurchaseTerm(userId, user, storeId,json_term);
        }

        public void AddUserPurchaseTerm(int userId, string user, int storeId, string json_term)
        {
            MarketController.AddUserPurchaseTerm(userId, user, storeId,json_term);
        }

        public void RemoveProductFromStore(int userId, string username, int storeId, int productID)
        {
            MarketController.RemoveProductFromStore(userId, username, storeId, productID);
        }

        public void ChangeProductName(int userId, string username, int storeId, int productID, string name)
        {
            MarketController.ChangeProductName(userId, username, storeId, productID, name);
        }

        public void ChangeProductPrice(int userId, string username, int storeId, int productID, double price)
        {
            MarketController.ChangeProductPrice(userId, username, storeId, productID, price);
        }

        public void ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity)
        {
            MarketController.ChangeProductQuantity(userId, username, storeId,productID,quantity);
        }

        public void ChangeProductCategory(int userId, string username, int storeId, int productID, string category)
        {
            MarketController.ChangeProductCategory(userId, username, storeId, productID, category);
        }

        internal List<Store> GetAllStores(int userId)
        {
            return MarketController.GetAllStores(userId);
        }

        internal List<Notification> TakeNotifications(int userId, string membername)
        {
            return UserController.TakeNotifications(userId, membername);
        }
        internal Dictionary<Member, bool> GetMembersOnlineStats(int userId, string actingUsername)
        {
            return UserController.GetMembersOnlineStats(userId, actingUsername);
        }
        internal void CancelMember(int userId, string actingUsername, string canceledUsername)
        {
            UserController.CancelMember(userId, actingUsername, canceledUsername);
        }

        internal double GetDailyIncomeMarketManager(int userId, string username)
        {
            return MarketController.GetDailyIncomeMarketManager(userId, username);
        }

        internal double GetDailyIncomeStoreOwner(int userId, string username, int storeId)
        {
            return MarketController.GetDailyIncomeStoreOwner(userId, username, storeId);
        }

        internal List<ServiceLayer.ServiceObjects.PermissionInformation> GetMemberPermissions(int userId, string membername)
        {
            return UserController.GetMemberPermissions(userId, membername);
        }

        internal double GetCartPrice(ShoppingCartDTO shoppingCart)
        {
            return MarketController.GetCartPrice(shoppingCart);
        }

        internal void RejectStoreOwnerNomination(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            MarketController.RejectStoreOwnerNomination(userId, nominatorUsername, nominatedUsername, storeId);
        }

        internal List<UserCountInDate> MarketManagerDailyRangeInformation(int userId, string membername, DateTime beginning, DateTime end)
        {
            return UserController.MarketManagerDailyRangeInformation(userId, membername, beginning, end);
        }

        internal List<OrderDTO> GetStorePurchaseHistory(int userId, string membername, int storeId)
        {
            return MarketController.GetStorePurchaseHistory(userId, membername, storeId);
        }

        internal Bid OfferBid(int userId, string username, int storeId, int productId, double price)
        {
            return MarketController.OfferBid(userId, username, storeId, productId, price);
        }
        internal Bid CounterBid(int userId, string membername, int storeId, int bidId, double newPrice)
        {
            return MarketController.CounterBid(userId, membername, storeId, bidId, newPrice);
        }
        internal Bid VoteForBid(int userId, string username, int storeId, int bidId, bool vote)
        {
            return MarketController.VoteForBid(userId, username, storeId, bidId, vote);
        }
        internal double BuyBidProduct(int userId, string username, int storeId, int bidId, CreditCard cc, SupplyAddress address, DateTime buyTime)
        {
            return MarketController.BuyBidProduct(userId, username, storeId, bidId, cc, address, buyTime);
        }
        internal List<Bid> GetBidsStatus(int userId, string username, int storeId)
        {
            return MarketController.GetBidsStatus(userId, username, storeId);
        }

        internal UserCountInDate TodaysInformation(DateTime date)
        {
            return UserController.TodaysInformation(date);
        }
    }
}