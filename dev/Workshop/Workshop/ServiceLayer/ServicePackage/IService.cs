using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Reviews;
using Workshop.ServiceLayer.ServiceObjects;

namespace Workshop.ServiceLayer
{
    public interface IService
    {
        Response<User> EnterMarket(int userId);

        Response ExitMarket(int userId);

        Response Register(int userId, string username, string password, DateTime birthdate);

        Response<Member> Login(int userId, string username, string password);

        Response Logout(int userId, string username);

        Response<Product> AddProduct(int userId, string username, int storeId, string productName, string description, double price, int quantity, string category);

        Response<StoreManager> NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId);

        Response<StoreOwner> NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId);

        Response<Member> RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId);

        Response<List<Member>> GetWorkersInformation(int userId, string username, int storeId);

        Response CloseStore(int userId, string username, int storeId);

        Response<Store> CreateNewStore(int userId, string creator, string storeName);

        Response<ReviewDTO> ReviewProduct(int userId, string user, int productId, string review, int rating);

        Response<List<Product>> SearchProduct(int userId, string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview);

        Response<List<Store>> GetAllStores(int userId);

        Response<Product> addToCart(int userId, string user, int productId, int storeId, int quantity);

        Response<ShoppingCart> viewCart(int userId, string user);

        Response<ShoppingCart> editCart(int userId, string user, int productId, int newQuantity);

        Response BuyCart(int userId, string user, string address);

        Response AddProductDiscount(int userId, string user, int storeId, string jsonDiscount, int productId);

        Response AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName);

        Response AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount);

        Response RemoveProductFromStore(int userId, string username, int storeId, int productID);

        Response ChangeProductName(int userId, string username, int storeId, int productID, string name);

        Response ChangeProductPrice(int userId, string username, int storeId, int productID, int price);

        Response ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity);

        Response ChangeProductCategory(int userId, string username, int storeId, int productID, string category);
    }
}
