using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Reviews;
using Workshop.ServiceLayer.ServiceObjects;
using CreditCard = Workshop.DomainLayer.MarketPackage.CreditCard;
using SupplyAddress = Workshop.DomainLayer.MarketPackage.SupplyAddress;

namespace Workshop.ServiceLayer
{
    public interface IService
    {
        Response<User> EnterMarket(int userId, DateTime date);
        Response ExitMarket(int userId);
        Response Register(int userId, string username, string password, DateTime birthdate);
        Response<KeyValuePair<Member, List<Notification>>> Login(int userId, string username, string password, DateTime date);
        Response Logout(int userId, string username);
        Response<Product> AddProduct(int userId, string username, int storeId, string productName, string description, double price, int quantity, string category);
        Response<List<Notification>> TakeNotifications(int userId, string membername);
        int GetPort();
        Response<StoreManager> NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date);
        Response<StatisticsInformation> TodaysInformation(DateTime date);
        Response<StoreOwner> NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date);
        Response RejectStoreOwnerNomination(int userId, string nominatorUsername, string nominatedUsername, int storeId);
        Response<Member> RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId);
        Response<List<Member>> GetWorkersInformation(int userId, string username, int storeId);
        Response CloseStore(int userId, string username, int storeId);
        Response OpenStore(int userId, string username, int storeId);
        Response<Store> CreateNewStore(int userId, string creator, string storeName, DateTime date);
        Response<ReviewDTO> ReviewProduct(int userId, string user, int productId, string review, int rating);
        Response<List<Product>> SearchProduct(int userId, string keyWords, string catagory, double minPrice, double maxPrice, double productReview);
        Response<List<Store>> GetAllStores(int userId);
        Response<Product> AddToCart(int userId, int productId, int storeId, int quantity);
        Response<ShoppingCart> ViewCart(int userId);
        Response<ShoppingCart> EditCart(int userId, int productId, int newQuantity);
        Response<double> BuyCart(int userId, CreditCard cc, SupplyAddress address, DateTime buyTime);
        Response AddProductDiscount(int userId, string user, int storeId, string jsonDiscount, int productId);
        Response AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName);
        Response AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount);
        Response RemoveProductFromStore(int userId, string username, int storeId, int productID);
        Response ChangeProductName(int userId, string username, int storeId, int productID, string name);
        Response ChangeProductPrice(int userId, string username, int storeId, int productID, double price);
        Response ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity);
        Response ChangeProductCategory(int userId, string username, int storeId, int productID, string category);
        Response AddProductPurchaseTerm(int userId, string user, int storeId, string jsonTerm, int productId);
        Response AddCategoryPurchaseTerm(int userId, string user, int storeId, string jsonTerm, string category);
        Response AddStorePurchaseTerm(int userId, string user, int storeId, string jsonTerm);
        Response AddUserPurchaseTerm(int userId, string user, int storeId, string jsonTerm);
        Response AddActionToManager(int userId, string owner, string manager, int storeId, string action);
        Response CancelMember(int userId, string actingUsername, string canceledUsername);
        Response<double> GetDailyIncomeStore(int userId, string username, int storeId);
        Response<double> GetDailyIncomeMarketManager(int userId, string username);
        Response<Dictionary<Member, bool>> GetMembersOnlineStats(int userId, string actingUsername);
        Response<List<PermissionInformation>> GetMemberPermissions(int userId, string membername);
        Response<List<StatisticsInformation>> MarketManagerDailyRangeInformation(int userId, string membername, DateTime beginning, DateTime end);
        Response<List<Order>> GetStorePurchaseHistory(int userId, string membername, int storeId);
        Response<Bid> OfferBid(int userId, string username, int storeId, int productId, double price);
        Response<Bid> CounterBid(int userId, string membername, int storeId, int bidId, double newPrice);
        Response<Bid> VoteForBid(int userId, string username, int storeId, int bidId, bool vote);
        Response<double> BuyBidProduct(int userId, string username, int storeId, int bidId, CreditCard cc, SupplyAddress address, DateTime buyTime);
        Response<List<Bid>> GetBidsStatus(int userId, string username, int storeId);
    }
}
