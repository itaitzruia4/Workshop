using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.MarketPackage
{
    class MarketController: IMarketController
    {
        private IUserController userController;
        private Dictionary<int, Store> stores;
        public MarketController(IUserController userController)
        {
            this.userController = userController;
            stores = new Dictionary<int, Store>();
        }

        private bool IsAuthorized(string username, int storeId, Action action)
        {
            return userController.IsAuthorized(username, storeId, action);
        }

        public StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            ValidateStoreExists(storeId);
            return userController.NominateStoreOwner(nominatorUsername, nominatedUsername, storeId);
        }

        public StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId)
        {
            ValidateStoreExists(storeId);
            return userController.NominateStoreManager(nominatorUsername, nominatedUsername, storeId);
        }

        private void ValidateStoreExists(int ID)
        {
            if (!stores.ContainsKey(ID))
                throw new ArgumentException("Store ID does not exist");
        }

        public Product AddProductToStore(string username, int storeId, int productID, string name, string description, double price, int quantity)
        {
            if (!IsAuthorized(username, storeId, Action.AddProduct))
                throw new MemberAccessException("This user is not authorized for adding products to the specified store.");
            ValidateStoreExists(storeId);
            return stores[storeId].AddProduct(productID, name, description, price, quantity);
        }

        public void RemoveProductFromStore(string username, int storeId, int productID)
        {
            if (!IsAuthorized(username, storeId, Action.RemoveProduct))
                throw new MemberAccessException("This user is not authorized for removing products from the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].RemoveProduct(productID);
        }

        public void ChangeProductName(string username, int storeId, int productID, string name)
        {
            if (!IsAuthorized(username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products names in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductName(productID, name);
        }

        public void ChangeProductPrice(string username, int storeId, int productID, int price)
        {
            if (!IsAuthorized(username, storeId, Action.ChangeProductPrice))
                throw new MemberAccessException("This user is not authorized for changing products prices in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductPrice(productID, price);
        }

        public void ChangeProductQuantity(string username, int storeId, int productID, int quantity)
        {
            if (!IsAuthorized(username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductQuantity(productID, quantity);
        }

        /// <summary>
        /// Get information about the workers of the store
        /// </summary>
        /// <param name="username">The name of the user requesting the information</param>
        /// <param name="storeId">The ID of the store</param>
        /// <returns>Information about the workers of the store</returns>
        public List<Member> GetWorkersInformation(string username, int storeId)
        {
            ValidateStoreExists(storeId);
            // Check that the user is the logged in member
            userController.AssertCurrentUser(username);
            // Check that the user is authorized to request this information
            if (!userController.IsAuthorized(username, storeId, Action.GetWorkersInformation))
                throw new MemberAccessException($"User {username} is not allowed to request information about the workers of store #{storeId}.");
            return userController.GetWorkers(storeId);
        }
        public void OpenStore(string username, int storeId)
        {
            /*if (!IsAuthorized(username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].openStore();*/
            throw new NotImplementedException();
        }

        public void CloseStore(string username, int storeId)
        {
            if (!IsAuthorized(username, storeId, Action.CloseStore))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].closeStore();
        }
    }
}
