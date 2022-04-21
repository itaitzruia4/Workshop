using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;

namespace Workshop.DomainLayer.MarketPackage
{
    interface IMarketController
    {
        StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);
        
        StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId);

        Product AddProductToStore(string username, int storeId, int productID, string name, string description, double price, int quantity);

        void RemoveProductFromStore(string username, int storeId, int productID);

        void ChangeProductName(string username, int storeId, int productID, string name);

        void ChangeProductPrice(string username, int storeId, int productID, int price);

        void ChangeProductQuantity(string username, int storeId, int productID, int quantity);
    }
}
