﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.MarketPackage
{
    public class MarketController: IMarketController
    {
        private IUserController userController;
        private OrderHandler<int> orderHandler;
        private Dictionary<int, Store> stores;
        private IMarketPaymentService paymentService;
        private IMarketSupplyService supplyService;
        private static int STORE_COUNT = 0;
        public MarketController(IUserController userController, IMarketPaymentService paymentService, IMarketSupplyService supplyService)
        {
            this.userController = userController;
            this.orderHandler = new OrderHandler<int>();
            this.paymentService = paymentService;
            this.supplyService = supplyService;
            stores = new Dictionary<int, Store>();
        }

        public void InitializeSystem()
        {
            CreateNewStore("User1", "Sport store");
            CreateNewStore("User2", "Drug store");
            CreateNewStore("User3", "Supermarket");
            CreateNewStore("User4", "Electronics store");
            CreateNewStore("User5", "Convenience store");
        }

        private bool IsAuthorized(string username, int storeId, Action action)
        {
            userController.AssertCurrentUser(username);
            return userController.IsAuthorized(username, storeId, action);
        }

        public StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            userController.AssertCurrentUser(nominatorUsername);
            ValidateStoreExists(storeId);
            return userController.NominateStoreOwner(nominatorUsername, nominatedUsername, storeId);
        }

        public StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId)
        {
            userController.AssertCurrentUser(nominatorUsername);
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
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            if (!IsAuthorized(username, storeId, Action.AddProduct))
                throw new MemberAccessException("This user is not authorized for adding products to the specified store.");
            ValidateStoreExists(storeId);
            return stores[storeId].AddProduct(productID, name, description, price, quantity);
        }

        public void RemoveProductFromStore(string username, int storeId, int productID)
        {
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            if (!IsAuthorized(username, storeId, Action.RemoveProduct))
                throw new MemberAccessException("This user is not authorized for removing products from the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].RemoveProduct(productID);
        }

        public void ChangeProductDescription(string username, int storeId, int productID, string description)
        {
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            if (!IsAuthorized(username, storeId, Action.ChangeProductDescription))
                throw new MemberAccessException("This user is not authorized for changing products descriptions in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductDescription(productID, description);
        }

        public void ChangeProductName(string username, int storeId, int productID, string name)
        {
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            if (!IsAuthorized(username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products names in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductName(productID, name);
        }

        public void ChangeProductPrice(string username, int storeId, int productID, int price)
        {
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            if (!IsAuthorized(username, storeId, Action.ChangeProductPrice))
                throw new MemberAccessException("This user is not authorized for changing products prices in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductPrice(productID, price);
        }

        public void ChangeProductQuantity(string username, int storeId, int productID, int quantity)
        {
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            if (!IsAuthorized(username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].ChangeProductQuantity(productID, quantity);
        }

        public List<OrderDTO> GetStoreOrdersList(string username, int storeId)
        {
            userController.AssertCurrentUser(username);
            ViewStorePermission(username, storeId);
            IsAuthorized(username, storeId, Action.GetStoreOrdersList);

            List<OrderDTO> orders = this.orderHandler.GetOrders(storeId);
            if (orders == null)
                throw new Exception($"Store {storeId} does not exist or it does not have previous orders.");
            return orders;
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
            ViewStorePermission(username, storeId);
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
                throw new MemberAccessException("This user is not authorized to close this specified store.");
            ValidateStoreExists(storeId);
            if (IsStoreOpen(username, storeId)) { stores[storeId].closeStore(); }
            else
            {
                throw new Exception($"Store {storeId} already closed.");
            }
        }

        public void ViewStorePermission(string username, int storeId)
        {
            userController.AssertCurrentUser(username);
            ValidateStoreExists(storeId);
            if (!IsStoreOpen(username,storeId) && !userController.IsAuthorized(username, storeId, Action.ViewClosedStore))
            {
                throw new Exception($"User {username} is not permitted to view closed store {storeId}.");
            }
        }

        public int CreateNewStore(string creator, string storeName) {
            userController.AssertCurrentUser(creator);
            if (String.IsNullOrWhiteSpace(storeName)){
                throw new ArgumentException($"User {creator} requestted to create a store with an empty name.");
            }
            Member member = userController.GetMember(creator);
            int storeId = STORE_COUNT;
            Store store = new Store(storeId, storeName);
            Role storeFounderRole = new StoreFounder(storeId);
            member.AddRole(storeFounderRole);
            stores[storeId] = store;
            STORE_COUNT++;
            return storeId;
        }

        public bool IsStoreOpen(string username, int storeId)
        {
            userController.AssertCurrentUser(username);
            ValidateStoreExists(storeId);
            return stores[storeId].isOpen();
        }
    }
}
