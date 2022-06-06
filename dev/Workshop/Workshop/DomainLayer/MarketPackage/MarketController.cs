using System;
using System.Collections.Generic;
using System.Linq;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.Loggers;
using System.Threading;
using System.Collections.Concurrent;
using Workshop.DomainLayer.UserPackage.Notifications;
using Workshop.ServiceLayer;
using User = Workshop.DomainLayer.UserPackage.User;

namespace Workshop.DomainLayer.MarketPackage
{
    public class MarketController : IMarketController
    {
        private IUserController userController;
        private OrderHandler<int> orderHandler;
        private ConcurrentDictionary<int, Store> stores;
        private ConcurrentDictionary<int, ReaderWriterLock> storesLocks;
        private IExternalSystem ExternalSystem;
        private int STORE_COUNT = 0;
        private int PRODUCT_COUNT = 1;
        public MarketController(IUserController userController, IExternalSystem externalSystem)
        {
            this.userController = userController;
            this.orderHandler = new OrderHandler<int>();
            this.ExternalSystem = externalSystem;
            this.stores = new ConcurrentDictionary<int, Store>();
            this.storesLocks = new ConcurrentDictionary<int, ReaderWriterLock>();
            STORE_COUNT = 0;
            PRODUCT_COUNT = 1;
        }

        public void InitializeSystem()
        {

            Logger.Instance.LogEvent("Started initializing the system - Market Controller");
            Logger.Instance.LogEvent("Finished initializing the system - Market Controller");

        }

        private bool IsAuthorized(int userId, string username, int storeId, Action action)
        {
            userController.AssertCurrentUser(userId, username);
            return userController.IsAuthorized(username, storeId, action);
        }

        public StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"{nominatorUsername} is trying to nominate {nominatedUsername} to be a store owner of store {storeId}");
            userController.AssertCurrentUser(userId, nominatorUsername);
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            StoreOwner storeOwner = userController.NominateStoreOwner(userId, nominatorUsername, nominatedUsername, storeId);

            storesLocks[storeId].ReleaseReaderLock();
            return storeOwner;
        }

        public StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"{nominatorUsername} is trying to nominate {nominatedUsername} to be a store manager of store {storeId}.");
            userController.AssertCurrentUser(userId, nominatorUsername);
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            StoreManager storeManager = userController.NominateStoreManager(userId, nominatorUsername, nominatedUsername, storeId);
            storesLocks[storeId].ReleaseReaderLock();
            return storeManager;
        }

        public Member RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatorMembername} is trying to remove store owner nomination from {nominatedMembername} in store {storeId}.");
            userController.AssertCurrentUser(userId, nominatorMembername);
            Member nominatorMember = userController.GetMember(nominatorMembername);
            Member nominatedMember = userController.GetMember(nominatedMembername);
            foreach (StoreRole nominatedRole in nominatedMember.GetStoreRoles(storeId))
            {
                foreach (StoreRole nominatorRole in nominatorMember.GetStoreRoles(storeId))
                {
                    if (nominatedRole is StoreOwner && (nominatorRole is StoreManager || nominatorRole is StoreOwner) && nominatorRole.ContainsNominee(nominatedRole))
                    {
                        nominatedMember.RemoveRole(nominatedRole);
                        nominatorRole.RemoveNominee(nominatedRole);
                        userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("SaleInStore" + storeId, "", "MarketController"));
                        userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("OpenStore" + storeId, "", "MarketController"));
                        userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("CloseStore" + storeId, "", "MarketController"));
                        userController.notify(new Event("RemoveStoreOwnerNominationFrom" + nominatedMembername, "Removed store owner nomination from user " + nominatedMembername, "MarketController"));
                        Logger.Instance.LogEvent($"User {userId} with member {nominatorMembername} successfuly removed store owner nomination from {nominatedMembername} in store {storeId}.");

                        foreach (StoreRole newNominated in nominatedRole.nominees)
                        {
                            //RemoveStoreOwnerNominationHelper(nominatedMember,newNominated.)
                        }
                        return nominatedMember;
                    }
                }
            }
            throw new ArgumentException($"User {userId} with member {nominatorMembername} FAILED to remove store owner nomination from {nominatedMembername} in store {storeId}.");
        }
        public void AddActionToManager(int userId, string owner, string manager, int storeId, string action)
        {
            Logger.Instance.LogEvent($"{owner} is trying to add action {action} to manager {manager} in store {storeId}.");
            userController.AssertCurrentUser(userId, owner);
            if (userController.GetMember(owner).GetStoreRoles(storeId).All(x => !(x is StoreOwner)))
            {
                throw new ArgumentException($"{owner} is not an owner and can not add action to a manager");
            }
            if (!userController.GetMember(manager).GetStoreRoles(storeId).Any(x => x is StoreManager))
            {
                throw new ArgumentException($"{manager} is not a store manager so an action can not be added to him");
            }
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            Action actualAction = ParseAction(action);
            foreach (StoreRole storeRole in userController.GetMember(manager).GetStoreRoles(storeId))
            {
                if (storeRole is StoreManager)
                {
                    if (!storeRole.IsAuthorized(actualAction))
                    {
                        storeRole.AddAction(actualAction);
                    }
                }
            }
            Logger.Instance.LogEvent($"{owner} successfuly added action {action} to manager {manager} in store {storeId}.");
            storesLocks[storeId].ReleaseReaderLock();
        }

        private Action ParseAction(string action)
        {
            switch (action)
            {
                case "AddProduct":
                    return Action.AddProduct;
                case "RemoveProduct":
                    return Action.RemoveProduct;
                case "ChangeProductName":
                    return Action.ChangeProductName;
                case "ChangeProductPrice":
                    return Action.ChangeProductPrice;
                case "ChangeProductQuantity":
                    return Action.ChangeProductQuantity;
                case "ChangeProductDescription":
                    return Action.ChangeProductDescription;
                case "NominateStoreOwner":
                    return Action.NominateStoreOwner;
                case "NominateStoreManager":
                    return Action.NominateStoreManager;
                case "GetWorkersInformation":
                    return Action.GetWorkersInformation;
                case "OpenStore":
                    return Action.OpenStore;
                case "CloseStore":
                    return Action.CloseStore;
                case "AddPermissionToStoreManager":
                    return Action.AddPermissionToStoreManager;
                case "RemovePermissionFromStoreManager":
                    return Action.RemovePermissionFromStoreManager;
                case "GetStoreOrdersList":
                    return Action.GetStoreOrdersList;
                case "ViewClosedStore":
                    return Action.ViewClosedStore;
                case "AddDiscount":
                    return Action.AddDiscount;
                case "GetMarketStatistics":
                    return Action.GetMarketStatistics;
                case "CancelMember":
                    return Action.CancelMember;
                case "GetMembersOnlineStats":
                    return Action.GetMembersOnlineStats;
                case "AddPurchaseTerm":
                    return Action.AddPurchaseTerm;
                default:
                    throw new ArgumentException($"Unrecognized action: {action}");
            }
        }

        private void ValidateStoreExists(int ID)
        {
            if (!stores.ContainsKey(ID))
                throw new ArgumentException($"Store ID {ID} does not exist");
        }

        public Product AddProductToStore(int userId, string username, int storeId, string name, string description, double price, int quantity, string category)
        {
            Logger.Instance.LogEvent($"{username} is trying to add product {name} to store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            Product product = null;
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException($"Store ID {storeId} does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.AddProduct))
                throw new MemberAccessException("This user is not authorized for adding products to the specified store.");
            product = stores[storeId].AddProduct(name, PRODUCT_COUNT++, description, price, quantity, category);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly added product {name} to store {storeId}.");
            return product;
        }

        public void RemoveProductFromStore(int userId, string username, int storeId, int productID)
        {
            Logger.Instance.LogEvent($"{username} is trying to remove product {productID} to store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.RemoveProduct))
                throw new MemberAccessException("This user is not authorized for removing products from the specified store.");
            stores[storeId].RemoveProduct(productID);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly removed product {productID} from store {storeId}.");
        }

        public void ChangeProductDescription(int userId, string username, int storeId, int productID, string description)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the description of product {productID} in store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductDescription))
                throw new MemberAccessException("This user is not authorized for changing products descriptions in the specified store.");
            stores[storeId].ChangeProductDescription(productID, description);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the description of product {productID} in store {storeId}.");
        }

        public void ChangeProductName(int userId, string username, int storeId, int productID, string name)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the name of product {productID} in store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products names in the specified store.");
            stores[storeId].ChangeProductName(productID, name);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the name of product {productID} in store {storeId}.");
        }

        public void ChangeProductPrice(int userId, string username, int storeId, int productID, double price)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the price of product {productID} in store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductPrice))
                throw new MemberAccessException("This user is not authorized for changing products prices in the specified store.");
            stores[storeId].ChangeProductPrice(productID, price);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the price of product {productID} in store {storeId}.");
        }

        public void ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the quantity of product {productID} in store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            stores[storeId].ChangeProductQuantity(productID, quantity);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the quantity of product {productID} in store {storeId}.");
        }

        public void ChangeProductCategory(int userId, string username, int storeId, int productID, string category)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the quantity of product {productID} in store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            stores[storeId].ChangeProductCategory(productID, category);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the quantity of product {productID} in store {storeId}.");
        }

        public List<OrderDTO> GetStoreOrdersList(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            List<OrderDTO> orders = null;
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            IsAuthorized(userId, username, storeId, Action.GetStoreOrdersList);

            orders = this.orderHandler.GetOrders(storeId);
            if (orders == null)
                throw new ArgumentException($"Store {storeId} does not exist or it does not have previous orders.");
            storesLocks[storeId].ReleaseReaderLock();
            return orders;
        }

        /// <summary>
        /// Get information about the workers of the store
        /// </summary>
        /// <param name="username">The name of the user requesting the information</param>
        /// <param name="storeId">The ID of the store</param>
        /// <returns>Information about the workers of the store</returns>
        public List<Member> GetWorkersInformation(int userId, string username, int storeId)
        {
            Logger.Instance.LogEvent($"{username} is requesting information about the workers of store {storeId}.");
            userController.AssertCurrentUser(userId, username);
            List<Member> workers = null;
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            ViewStorePermission(userId, username, storeId);
            if (!userController.IsAuthorized(username, storeId, Action.GetWorkersInformation))
                throw new MemberAccessException($"User {username} is not allowed to request information about the workers of store #{storeId}.");
            workers = userController.GetWorkers(storeId);
            storesLocks[storeId].ReleaseReaderLock();
            Logger.Instance.LogEvent($"{username} has received information about the workers of store {storeId}.");
            return workers;
        }
        public void OpenStore(int userId, string membername, int storeId)
        {
            Logger.Instance.LogEvent($"{membername} is trying to open store {storeId}.");
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            if (!IsAuthorized(userId, membername, storeId, Action.CloseStore))
                throw new MemberAccessException("This user is not authorized to open this specified store.");
            if (!IsStoreOpen(userId, membername, storeId))
            {
                stores[storeId].OpenStore();
                userController.notify(new Event("OpenStore" + storeId, "store " + storeId + "is open" + "by user " + membername, "marketController"));
            }
            else
            {
                throw new ArgumentException($"Store {storeId} is already opened.");
            }
            storesLocks[storeId].ReleaseWriterLock();
            //userController.RegisterToEvent(userId, new Event("SaleInStore" + storeId, "", "MarketController"));
            //userController.RegisterToEvent(userId, new Event("OpenStore" + storeId, "", "MarketController"));
            //userController.RegisterToEvent(userId, new Event("CloseStore" + storeId, "", "MarketController"));
            Logger.Instance.LogEvent($"{membername} successfuly opened store {storeId}.");
        }

        public void CloseStore(int userId, string username, int storeId)
        {
            Logger.Instance.LogEvent($"{username} is trying to close store {storeId}.");
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            if (!IsAuthorized(userId, username, storeId, Action.CloseStore))
                throw new MemberAccessException("This user is not authorized to close this specified store.");
            if (IsStoreOpen(userId, username, storeId))
            {
                userController.notify(new Event("CloseStore" + storeId, "store " + storeId + "is close by user" + username, "marketController"));
                stores[storeId].CloseStore();
            }
            else
            {
                throw new ArgumentException($"Store {storeId} is already closed.");
            }
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly closed store {storeId}.");
        }

        public void ViewStorePermission(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            ValidateStoreExists(storeId);
            if (!IsStoreOpen(userId, username, storeId) && !userController.IsAuthorized(username, storeId, Action.ViewClosedStore))
            {
                throw new Exception($"User {username} is not permitted to view closed store {storeId}.");
            }
        }

        public Store CreateNewStore(int userId, string creator, string storeName)
        {
            Logger.Instance.LogEvent($"{creator} is trying to create a new store: \"{storeName}\".");
            userController.AssertCurrentUser(userId, creator);
            if (String.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException($"User {creator} requestted to create a store with an empty name.");
            }
            int storeId = STORE_COUNT;
            userController.AddStoreFounder(creator, storeId);
            ReaderWriterLock rwl = new ReaderWriterLock();
            rwl.AcquireWriterLock(Timeout.Infinite);
            storesLocks[storeId] = rwl;
            Store store = new Store(storeId, storeName);
            stores[storeId] = store;
            STORE_COUNT++;
            rwl.ReleaseWriterLock();

            userController.RegisterToEvent(creator, new Event("SaleInStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("OpenStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("CloseStore" + storeId, "", "MarketController"));
            Logger.Instance.LogEvent($"{creator} successfuly created store \"{storeName}\", and received a new store ID: {storeId}.");
            return store;
        }

        public bool IsStoreOpen(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            ValidateStoreExists(storeId);
            return stores[storeId].IsOpen();
        }

        public ProductDTO getProductInfo(int userId, string username, int productId)
        {
            userController.AssertCurrentUser(userId, username);
            Product product = getProduct(productId);
            return product.GetProductDTO();
        }

        public StoreDTO getStoreInfo(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            Store store = stores[storeId];
            StoreDTO storeDTO = store.GetStoreDTO();
            storesLocks[storeId].ReleaseWriterLock();
            return storeDTO;
        }

        private Product getProduct(int productId)
        {
            foreach (Store store in stores.Values)
            {
                try
                {
                    return store.GetProduct(productId);
                }
                catch (ArgumentException)
                {
                }
            }
            throw new ArgumentException($"Product with ID {productId} does not exist in the market.");
        }
        public List<ProductDTO> SearchProduct(int userId, string keyWords, string category, double minPrice, double maxPrice, double productReview)
        {
            userController.AssertUserEnteredMarket(userId);
            IEnumerable<IEnumerable<Product>> allProducts = stores.Values.Select(s => s.GetProducts().Values);
            IEnumerable<Product> products = allProducts.SelectMany(lp => lp);
            if (keyWords != "")
            {
                products = products.Where(p => p.Name.Contains(keyWords));
            }
            if (category != "")
            {
                products = products.Where(p => p.Category == category);
            }
            if (minPrice != -1)
            {
                products = products.Where(p => p.Price >= minPrice);
            }

            if (maxPrice != -1)
            {
                products = products.Where(p => p.Price <= maxPrice);
            }

            if (productReview != -1)
            {
                products = products.Where(p => userController.GetProductRating(p.Id) >= productReview);
            }
            return products.Select(p => p.GetProductDTO()).ToList();
        }

        public ShoppingBagProduct getProductForSale(int productId, int storeId, int quantity)
        {
            ShoppingBagProduct porduct = null;
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            if (quantity <= 0)
            {
                storesLocks[storeId].ReleaseWriterLock();
                throw new ArgumentException($"Can't add {quantity} of an item to the shopping cart");
            }
            if (stores[storeId].GetProduct(productId).Quantity >= quantity)
            {
                try
                {
                    porduct = stores[storeId].GetProductForSale(productId, quantity).GetShoppingBagProduct(quantity);
                    storesLocks[storeId].ReleaseWriterLock();
                }
                catch (Exception e)
                {
                    storesLocks[storeId].ReleaseWriterLock();
                    throw e;
                }

            }
            else
            {
                storesLocks[storeId].ReleaseWriterLock();
                throw new ArgumentException("Store doesn't has enough from the product");
            }

            return porduct;
        }
        public double BuyCart(int userId, CreditCard cc, SupplyAddress address)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to buy his cart.");
            ShoppingCartDTO shoppingCart = userController.viewCart(userId);
            if (shoppingCart.IsEmpty())
            {
                throw new InvalidOperationException("Can't buy an empty shopping cart");
            }
            Dictionary<int, List<ProductDTO>> productsSoFar = new Dictionary<int, List<ProductDTO>>();
            List<Event> events = new List<Event>();
            foreach (int storeId in shoppingCart.shoppingBags.Keys)
            {
                try
                {
                    storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
                }
                catch
                {
                    throw new ArgumentException($"Store {storeId} does not exist");
                }
                try
                {
                    int age = userController.GetAge(userId);
                    stores[storeId].CheckPurchasePolicy(shoppingCart.shoppingBags[storeId], age);
                    //ShoppingBagDTO bag = stores[storeId].validateBagInStockAndGet(shoppingCart.shoppingBags[storeId]);
                    ShoppingBagDTO bag = new ShoppingBagDTO(storeId, shoppingCart.shoppingBags[storeId].products);
                    string products = "";
                    foreach (ProductDTO product in bag.products)
                    {
                        products = product.Name + " ";
                    }
                    User currentUser = userController.GetUser(userId);
                    string username = currentUser is Member ? ((Member)currentUser).Username : "A guest";
                    events.Add(new Event("SaleInStore" + storeId, $"Prouducts with the name {products} were bought from store {storeId} by {username}", "marketController"));
                    productsSoFar.Add(storeId, shoppingCart.shoppingBags[storeId].products);
                    OrderDTO order = orderHandler.CreateOrder(username, address, stores[storeId].GetStoreName(), shoppingCart.shoppingBags[storeId].products);
                    orderHandler.addOrder(order, storeId);
                    userController.AddOrder(userId, order, username);
                    storesLocks[storeId].ReleaseWriterLock();
                }
                catch (Exception e)
                {
                    //todo add restore prod who tf added this :D
                    storesLocks[storeId].ReleaseWriterLock();
                    throw e;
                }

            }
            int pay_trans_id = ExternalSystem.Pay(cc.Card_number, cc.Month, cc.Year, cc.Holder, cc.Ccv, cc.Id);
            if (pay_trans_id != -1)
            {
                int supply_trans_id = ExternalSystem.Supply(address.Name, address.Address, address.City, address.Country, address.Zip);
                if (supply_trans_id != -1)
                {
                    double cartPrice = GetCartPrice(shoppingCart);
                    foreach (Event eventt in events)
                    {
                        userController.notify(eventt);
                    }
                    userController.ClearUserCart(userId);
                    Logger.Instance.LogEvent($"User {userId} successfuly paid {cartPrice} and purchased his cart.");
                    return cartPrice;
                }
                else
                {
                    ExternalSystem.Cancel_Pay(pay_trans_id);
                }
            }
            throw new ArgumentException("Buying cart failed");
        }
    

    public double GetCartPrice(ShoppingCartDTO shoppingCart)
    {
        double price = 0;
        foreach (ShoppingBagDTO shoppingBag in shoppingCart.shoppingBags.Values)
        {
            price += stores[shoppingBag.storeId].CalaculatePrice(shoppingBag);
        }
        return price;
    }

    public ShoppingBagProduct addToBag(int userId, int productId, int storeId, int quantity)
    {
        return userController.addToCart(userId, getProductForSale(productId, storeId, quantity), storeId);
    }

    public void AddProductDiscount(int userId, string user, int storeId, string jsonDiscount, int productId)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddDiscount))
            throw new MemberAccessException("User " + user + " is not allowed to add discounts in store " + storeId);
        stores[storeId].AddProductDiscount(jsonDiscount, productId);
        storesLocks[storeId].ReleaseWriterLock();
    }
    public void AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddDiscount))
            throw new MemberAccessException("User " + user + " is not allowed to add discounts in store " + storeId);
        stores[storeId].AddCategoryDiscount(jsonDiscount, categoryName);
        storesLocks[storeId].ReleaseWriterLock();

    }
    public void AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddDiscount))
            throw new MemberAccessException("User " + user + " is not allowed to add discounts in store " + storeId);
        stores[storeId].AddStoreDiscount(jsonDiscount);
        storesLocks[storeId].ReleaseWriterLock();

    }

    public void AddProductPurchaseTerm(int userId, string user, int storeId, string json_term, int product_id)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddPurchaseTerm))
            throw new MemberAccessException("User " + user + " is not allowed to add purchase terms in store " + storeId);
        stores[storeId].AddProductTerm(json_term, product_id);
        storesLocks[storeId].ReleaseWriterLock();

    }

    public void AddCategoryPurchaseTerm(int userId, string user, int storeId, string json_term, string category_name)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddPurchaseTerm))
            throw new MemberAccessException("User " + user + " is not allowed to add purchase terms in store " + storeId);
        stores[storeId].AddCategoryTerm(json_term, category_name);
        storesLocks[storeId].ReleaseWriterLock();

    }

    public void AddStorePurchaseTerm(int userId, string user, int storeId, string json_term)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddPurchaseTerm))
            throw new MemberAccessException("User " + user + " is not allowed to add purchase terms in store " + storeId);
        stores[storeId].AddStoreTerm(json_term);
        storesLocks[storeId].ReleaseWriterLock();

    }

    public void AddUserPurchaseTerm(int userId, string user, int storeId, string json_term)
    {
        userController.AssertCurrentUser(userId, user);
        try
        {
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
        }
        catch
        {
            throw new ArgumentException("Store ID does not exist");
        }
        if (!IsAuthorized(userId, user, storeId, Action.AddPurchaseTerm))
            throw new MemberAccessException("User " + user + " is not allowed to add purchase terms in store " + storeId);
        stores[storeId].AddUserTerm(json_term);
        storesLocks[storeId].ReleaseWriterLock();

    }

    public List<Store> GetAllStores(int userId)
    {
        if (!userController.IsConnected(userId))
        {
            throw new ArgumentException($"{userId} is not connected");
        }
        return new List<Store>(stores.Values);
    }
}
}
