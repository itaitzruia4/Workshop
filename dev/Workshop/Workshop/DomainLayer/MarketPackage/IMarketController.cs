using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage
{
    public interface IMarketController
    {
        void InitializeSystem();
        StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date);
        
        StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date);

        Member RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId);

        void AddActionToManager(int userId, string owner, string manager, int storeId, string action);

        Product AddProductToStore(int userId, string username, int storeId, string name, string description, double price, int quantity, string category);

        void RemoveProductFromStore(int userId, string username, int storeId, int productID);

        void ChangeProductName(int userId, string username, int storeId, int productID, string name);

        void ChangeProductPrice(int userId, string username, int storeId, int productID, double price);

        void ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity);

        void ChangeProductCategory(int userId, string username, int storeId, int productID, string category);

        List<Member> GetWorkersInformation(int userId, string username, int storeId);

        void CloseStore(int userId, string username, int storeId);

        void OpenStore(int userId, string username, int storeId);
        
        Store CreateNewStore(int userId, string creator, string storeName, DateTime date);

        bool IsStoreOpen(int userId, string username, int storeId);

        void ViewStorePermission(int userId, string username, int storeId);

        ProductDTO getProductInfo(int userId, string user, int productId);

        StoreDTO getStoreInfo(int userId, string user, int storeId);

        List<ProductDTO> SearchProduct(int userId, string keyWords, string catagory, double minPrice, double maxPrice, double productReview);

        double BuyCart(int userId, CreditCard cc, SupplyAddress address, DateTime buyTime);
        ShoppingCartDTO EditCart(int userId, int productId, int newQuantity);

        ShoppingBagProduct AddToCart(int userId, int productId, int storeId, int quantity);

        void AddProductDiscount(int userId, string user, int storeId ,string jsonDiscount, int productId);

        void AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName);

        void AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount);

        void AddProductPurchaseTerm(int userId, string user, int storeId, string json_term, int product_id);

        void AddCategoryPurchaseTerm(int userId, string user, int storeId, string json_term, string category_name);

        void AddStorePurchaseTerm(int userId, string user, int storeId, string json_term);

        void AddUserPurchaseTerm(int userId, string user, int storeId, string json_term);

        List<Store> GetAllStores(int userId);
        double GetDailyIncomeStoreOwner(int userId, string username, int storeId);
        double GetDailyIncomeMarketManager(int userId, string username);
        double GetCartPrice(ShoppingCartDTO shoppingCart);
        void RejectStoreOwnerNomination(int userId, string nominatorUsername, string nominatedUsername, int storeId);
        List<OrderDTO> GetStorePurchaseHistory(int userId, string membername, int storeId);
    }
}
