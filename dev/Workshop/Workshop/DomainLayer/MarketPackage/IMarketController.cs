using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    interface IMarketController
    {
        void NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);

        void AddProductToStore(string username, int storeId, int productID, string name, int price, int quantity);

        void RemoveProductFromStore(string username, int storeId, int productID);

        void ChangeProductName(string username, int storeId, int productID, string name);

        void ChangeProductPrice(string username, int storeId, int productID, int price);

        void ChangeProductQuantity(string username, int storeId, int productID, int quantity);
    }
}
