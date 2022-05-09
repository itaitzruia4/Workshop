using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.ServiceLayer.ServiceObjects;

namespace Workshop.ServiceLayer
{
    public interface IService
    {
        Response<User> EnterMarket();

        Response ExitMarket();

        Response Register(string username, string password);

        Response<Member> Login(string username, string password);

        Response Logout(string username);

        Response<Product> AddProduct(string username, int storeId, int productId, string productName, string description, double price, int quantity, string category);

        Response<StoreManager> NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId);

        Response<StoreOwner> NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);

        Response<List<Member>> GetWorkersInformation(string username, int storeId);

        Response CloseStore(string username, int storeId);

        Response<Store> CreateNewStore(string creator, string storeName);

        Response ReviewProduct(string user, int productId, string review);

        Response<List<Product>> SearchProduct(string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview);

        Response<Product> addToCart(string user, int productId, int storeId, int quantity);

        Response<ShoppingCart> viewCart(string user);

        Response<ShoppingCart> editCart(string user, int productId, int newQuantity);

        Response BuyCart(string user, string address);

        Response AddProductDiscount(string user, int storeId, string jsonDiscount, int productId);

        Response AddCategoryDiscount(string user, int storeId, string jsonDiscount, string categoryName);

        Response AddStoreDiscount(string user, int storeId, string jsonDiscount);

        Response RemoveProductFromStore(string username, int storeId, int productID);

        Response ChangeProductName(string username, int storeId, int productID, string name);

        Response ChangeProductPrice(string username, int storeId, int productID, int price);

        Response ChangeProductQuantity(string username, int storeId, int productID, int quantity);

        Response ChangeProductCategory(string username, int storeId, int productID, string category);
    }
}
