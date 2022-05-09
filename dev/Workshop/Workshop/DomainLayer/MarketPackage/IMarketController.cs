using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;

namespace Workshop.DomainLayer.MarketPackage
{
    public interface IMarketController
    {
        void InitializeSystem();
        StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId);
        
        StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId);

        Product AddProductToStore(int userId, string username, int storeId, int productID, string name, string description, double price, int quantity, string category);

        void RemoveProductFromStore(int userId, string username, int storeId, int productID);

        void ChangeProductName(int userId, string username, int storeId, int productID, string name);

        void ChangeProductPrice(int userId, string username, int storeId, int productID, int price);

        void ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity);

        void ChangeProductCategory(int userId, string username, int storeId, int productID, string category);

        List<Member> GetWorkersInformation(int userId, string username, int storeId);

        void CloseStore(int userId, string username, int storeId);
        
        Store CreateNewStore(int userId, string creator, string storeName);

        bool IsStoreOpen(int userId, string username, int storeId);

        void ViewStorePermission(int userId, string username, int storeId);

        ProductDTO getProductInfo(int userId, string user, int productId);

        StoreDTO getStoreInfo(int userId, string user, int storeId);

        List<ProductDTO> SearchProduct(int userId, string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview);

        void BuyCart(int userId, string user,string address);

        ShoppingBagProduct getProductForSale(int productId, int storeId, int quantity);

        ShoppingBagProduct addToBag(int userId, string user, int productId, int storeId, int quantity);

        void AddProductDiscount(int userId, string user, int storeId ,string jsonDiscount, int productId);

        void AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName);

        void AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount);
    }
}
