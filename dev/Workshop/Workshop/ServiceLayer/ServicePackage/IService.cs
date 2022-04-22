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

        Response Register(string username, string password);

        Response<Member> Login(string username, string password);

        Response Logout(string username);

        Response<Product> AddProduct(string username, int storeId, int productId, string productName, string description, double price, int quantity);

        Response<StoreManager> NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId);

        Response<StoreOwner> NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);


        Response<List<Member>> GetWorkersInformation(string username, int storeId);

        Response CloseStore(string username, int storeId);

        Response<int> CreateNewStore(string creator, string storeName);

        Response ReviewProduct(string user, int productId, string review);
    }
}
