﻿using System;
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
        StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId);
        
        StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId);

        Product AddProductToStore(string username, int storeId, int productID, string name, string description, double price, int quantity);

        void RemoveProductFromStore(string username, int storeId, int productID);

        void ChangeProductName(string username, int storeId, int productID, string name);

        void ChangeProductPrice(string username, int storeId, int productID, int price);

        void ChangeProductQuantity(string username, int storeId, int productID, int quantity);

        List<Member> GetWorkersInformation(string username, int storeId);

        void CloseStore(string username, int storeId);
        
        StoreDTO CreateNewStore(string creator, string storeName);

        bool IsStoreOpen(string username, int storeId);

        void ViewStorePermission(string username, int storeId);
        object GetProductForSale(int productId, int storeId, int quantity);
        List<ProductDTO> SearchProduct(string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview, object storeReview);
        Store GetStoreInfo(string user, int storeId);
        Product GetProductInfo(string user, int productId);
    }
}
