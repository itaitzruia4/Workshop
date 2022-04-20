using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage;

namespace Workshop.DomainLayer.MarketPackage
{
    class MarketController
    {
        private UserController userController;
        private Dictionary<int, Store> stores;
        public MarketController(UserController userController)
        {
            this.userController = userController;
            stores = new Dictionary<int, Store>();
        }

        private Boolean IsAuthorized(int userId, int storeId, UserPackage.Action action)
        {
            return userController.IsAuthorized(userId, storeId, action);
        }

        private void ValidateStoreExist(int ID)
        {
            if (!stores.ContainsKey(ID))
                throw new Exception("Store ID does not exist");
        }

        public void AddProductToStore(int userId, int storeId, int productID, string name, int price, int quantity)
        {
            if (!IsAuthorized(userId, storeId, UserPackage.Action.AddProduct))
                throw new MemberAccessException("This user is not authorized for adding products to the specified store.");
            ValidateStoreExist(storeId);
            stores[storeId].AddProduct(productID, name, price, quantity);
        }

        public void RemoveProductFromStore(int userId, int storeId, int productID)
        {
            if (!IsAuthorized(userId, storeId, UserPackage.Action.RemoveProduct))
                throw new MemberAccessException("This user is not authorized for removing products from the specified store.");
            ValidateStoreExist(storeId);
            stores[storeId].RemoveProduct(productID);
        }

        public void ChangeProductName(int userId, int storeId, int productID, string name)
        {
            if (!IsAuthorized(userId, storeId, UserPackage.Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products names in the specified store.");
            ValidateStoreExist(storeId);
            stores[storeId].ChangeProductName(productID, name);
        }

        public void ChangeProductPrice(int userId, int storeId, int productID, int price)
        {
            if (!IsAuthorized(userId, storeId, UserPackage.Action.ChangeProductPrice))
                throw new MemberAccessException("This user is not authorized for changing products prices in the specified store.");
            ValidateStoreExist(storeId);
            stores[storeId].ChangeProductPrice(productID, price);
        }

        public void ChangeProductQuantity(int userId, int storeId, int productID, int quantity)
        {
            if (!IsAuthorized(userId, storeId, UserPackage.Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            ValidateStoreExist(storeId);
            stores[storeId].ChangeProductQuantity(productID, quantity);
        }
    }
}
