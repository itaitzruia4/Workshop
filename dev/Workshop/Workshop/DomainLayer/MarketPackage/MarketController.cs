using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;
using Workshop.DomainLayer.MarketPackage.ExternalServices;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.Loggers;
using System.Threading;
using System.Collections.Concurrent;

namespace Workshop.DomainLayer.MarketPackage
{
    public class MarketController: IMarketController
    {
        private IUserController userController;
        private OrderHandler<int> orderHandler;
        private ConcurrentDictionary<int, Store> stores;
        private ConcurrentDictionary<int, ReaderWriterLock> storesLocks;
        private IMarketPaymentService paymentService;
        private IMarketSupplyService supplyService;
        private static int STORE_COUNT = 0;
        private static int PRODUCT_COUNT = 0;
        public MarketController(IUserController userController, IMarketPaymentService paymentService, IMarketSupplyService supplyService)
        {
            this.userController = userController;
            this.orderHandler = new OrderHandler<int>();
            this.paymentService = paymentService;
            this.supplyService = supplyService;
            this.stores = new ConcurrentDictionary<int, Store>();
            this.storesLocks = new ConcurrentDictionary<int, ReaderWriterLock>();
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
            StoreOwner storeOwner = null;
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            finally {
                storeOwner = userController.NominateStoreOwner(userId, nominatorUsername, nominatedUsername, storeId);
                storesLocks[storeId].ReleaseReaderLock();
            }
            return storeOwner;
        }

        public StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"{nominatorUsername} is trying to nominate {nominatedUsername} to be a store manager of store {storeId}.");
            userController.AssertCurrentUser(userId, nominatorUsername);
            StoreManager storeManager = null;
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            finally
            {
                storeManager = userController.NominateStoreManager(userId, nominatorUsername, nominatedUsername, storeId);
                storesLocks[storeId].ReleaseReaderLock();
            }
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
                        Logger.Instance.LogEvent($"User {userId} with member {nominatorMembername} successfuly removed store owner nomination from {nominatedMembername} in store {storeId}.");
                        return nominatedMember;
                    }
                }
            }
            throw new ArgumentException($"User {userId} with member {nominatorMembername} FAILED to remove store owner nomination from {nominatedMembername} in store {storeId}.");
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.AddProduct))
                    throw new MemberAccessException("This user is not authorized for adding products to the specified store.");
                product = stores[storeId].AddProduct(name, PRODUCT_COUNT, description, price, quantity, category);
                storesLocks[storeId].ReleaseWriterLock();
                PRODUCT_COUNT++;
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.RemoveProduct))
                    throw new MemberAccessException("This user is not authorized for removing products from the specified store.");
                stores[storeId].RemoveProduct(productID);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.ChangeProductDescription))
                    throw new MemberAccessException("This user is not authorized for changing products descriptions in the specified store.");
                stores[storeId].ChangeProductDescription(productID, description);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.ChangeProductName))
                    throw new MemberAccessException("This user is not authorized for changing products names in the specified store.");
                stores[storeId].ChangeProductName(productID, name);
                storesLocks[storeId].ReleaseWriterLock();
            }
            Logger.Instance.LogEvent($"{username} successfuly changed the name of product {productID} in store {storeId}.");
        }

        public void ChangeProductPrice(int userId, string username, int storeId, int productID, int price)
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.ChangeProductPrice))
                    throw new MemberAccessException("This user is not authorized for changing products prices in the specified store.");
                stores[storeId].ChangeProductPrice(productID, price);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.ChangeProductName))
                    throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
                stores[storeId].ChangeProductQuantity(productID, quantity);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!IsAuthorized(userId, username, storeId, Action.ChangeProductName))
                    throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
                stores[storeId].ChangeProductCategory(productID, category);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                IsAuthorized(userId, username, storeId, Action.GetStoreOrdersList);

                orders = this.orderHandler.GetOrders(storeId);
                if (orders == null)
                    throw new ArgumentException($"Store {storeId} does not exist or it does not have previous orders.");
                storesLocks[storeId].ReleaseReaderLock();
            }
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
            finally
            {
                ViewStorePermission(userId, username, storeId);
                if (!userController.IsAuthorized(username, storeId, Action.GetWorkersInformation))
                    throw new MemberAccessException($"User {username} is not allowed to request information about the workers of store #{storeId}.");
                workers = userController.GetWorkers(storeId);
                storesLocks[storeId].ReleaseReaderLock();
            }
            Logger.Instance.LogEvent($"{username} has received information about the workers of store {storeId}.");
            return workers;
        }
        public void OpenStore(string username, int storeId)
        {
            Logger.Instance.LogEvent($"{username} is trying to open store {storeId}.");
            /*if (!IsAuthorized(username, storeId, Action.ChangeProductName))
                throw new MemberAccessException("This user is not authorized for changing products qunatities in the specified store.");
            ValidateStoreExists(storeId);
            stores[storeId].openStore();*/
            throw new NotImplementedException();
            // Logger.Instance.LogEvent($"{username} successfuly opened store {storeId}.");
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
            finally
            {
                if (!IsAuthorized(userId, username, storeId, Action.CloseStore))
                    throw new MemberAccessException("This user is not authorized to close this specified store.");
                if (IsStoreOpen(userId, username, storeId)) { stores[storeId].closeStore(); }
                else
                {
                    throw new ArgumentException($"Store {storeId} already closed.");
                }
                storesLocks[storeId].ReleaseWriterLock();
            }
            Logger.Instance.LogEvent($"{username} successfuly closed store {storeId}.");
        }

        public void ViewStorePermission(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            ValidateStoreExists(storeId);
            if (!IsStoreOpen(userId, username,storeId) && !userController.IsAuthorized(username, storeId, Action.ViewClosedStore))
            {
                throw new Exception($"User {username} is not permitted to view closed store {storeId}.");
            }
        }

        public Store CreateNewStore(int userId, string creator, string storeName) {
            Logger.Instance.LogEvent($"{creator} is trying to create a new store: \"{storeName}\".");
            userController.AssertCurrentUser(userId, creator);
            if (String.IsNullOrWhiteSpace(storeName)){
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
            StoreDTO storeDTO = null;
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            finally
            {
                Store store = stores[storeId];
                storeDTO = store.GetStoreDTO();
                storesLocks[storeId].ReleaseWriterLock();
            }
            return storeDTO;
        }

        private Product getProduct(int productId)
        {
            foreach(Store store in stores.Values)
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
        public List<ProductDTO> SearchProduct(int userId, string username, string keyWords, string category, double minPrice, double maxPrice, int productReview)
        {
            userController.AssertCurrentUser(userId, username);
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
            ValidateStoreExists(storeId);
            if (quantity <= 0)
            {
                throw new ArgumentException($"Can't add {quantity} of an item to the shopping cart");
            }
            if(stores[storeId].GetProduct(productId).Quantity >= quantity){
                return stores[storeId].GetProduct(productId).GetShoppingBagProduct(quantity);
            }
            throw new ArgumentException("Store doesn't has enough from the product");
        }
        public void BuyCart(int userId, string username, string address)
        {
            Logger.Instance.LogEvent($"User {username} is trying to buy his cart.");
            ShoppingCartDTO shoppingCart = userController.viewCart(userId, username);
            if (shoppingCart.IsEmpty())
            {
                throw new InvalidOperationException("Can't buy an empty shopping cart");
            }
            Dictionary<int,List<ProductDTO>> productsSoFar = new Dictionary<int, List<ProductDTO>>();
            try
            {
                foreach (int storeId in shoppingCart.shoppingBags.Keys)
                {
                    try
                    {
                        storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
                    }
                    catch
                    {
                        throw new ArgumentException($"Store {storeId} does not exist");
                    }
                    finally
                    {
                        int age = userController.GetAge(userId, username);
                        stores[storeId].CheckPurchasePolicy(shoppingCart.shoppingBags[storeId], age);
                        stores[storeId].validateBagInStockAndGet(shoppingCart.shoppingBags[storeId]);
                        productsSoFar.Add(storeId, shoppingCart.shoppingBags[storeId].products);
                        OrderDTO order = orderHandler.CreateOrder(username, address, stores[storeId].GetStoreName(), shoppingCart.shoppingBags[storeId].products);
                        orderHandler.addOrder(order, storeId);
                        userController.AddOrder(userId, order, username);
                        storesLocks[storeId].ReleaseReaderLock();
                    }
                }
                supplyService.supplyToAddress(username, address);
                paymentService.PayAmount(username,GetCartPrice(shoppingCart));
                userController.ClearUserCart(userId);
                Logger.Instance.LogEvent($"User {userId} successfuly paid {shoppingCart.getPrice()} and purchased his cart.");
            }
            catch (ArgumentException)
            {
                //restore taken products till we have a smarter way of threding
                foreach (int storeId in productsSoFar.Keys)
                {
                    foreach (ProductDTO item in productsSoFar[storeId])
                    {
                       stores[storeId].restoreProduct(item);   
                    }
                }  
            }
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

        public ShoppingBagProduct addToBag(int userId, string user, int productId, int storeId, int quantity)
        {
            return userController.addToCart(userId, user, getProductForSale(productId, storeId, quantity), storeId);
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
            finally
            {
                if (!IsAuthorized(userId, user, storeId, Action.AddDiscount))
                    throw new MemberAccessException("User " + user + " is not allowed to add discounts in store " + storeId);
                stores[storeId].AddProductDiscount(jsonDiscount, productId);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                if (!IsAuthorized(userId, user, storeId, Action.AddDiscount))
                    throw new MemberAccessException("User " + user + " is not allowed to add discounts in store " + storeId);
                stores[storeId].AddCategoryDiscount(jsonDiscount, categoryName);
                storesLocks[storeId].ReleaseWriterLock();
            }
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
            finally
            {
                if (!IsAuthorized(userId, user, storeId, Action.AddDiscount))
                    throw new MemberAccessException("User " + user + " is not allowed to add discounts in store " + storeId);
                stores[storeId].AddStoreDiscount(jsonDiscount);
                storesLocks[storeId].ReleaseWriterLock();
            }
        }

        public List<Store> GetAllStores()
        {
            return new List<Store>(stores.Values);
        }
    }
}
