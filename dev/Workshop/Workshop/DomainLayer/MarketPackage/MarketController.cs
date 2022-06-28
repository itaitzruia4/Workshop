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
using Workshop.DomainLayer.MarketPackage.Biding;
using DALMarketController = Workshop.DataLayer.DataObjects.Controllers.MarketController;
using DALStore = Workshop.DataLayer.DataObjects.Market.Store;
using DataHandler = Workshop.DataLayer.DataHandler;
using MemberDAL = Workshop.DataLayer.DataObjects.Members.Member;

namespace Workshop.DomainLayer.MarketPackage
{
    public class MarketController : IMarketController, IPersistentObject<DALMarketController>
    {
        private IUserController userController;
        private OrderHandler<int> orderHandler;
        private ConcurrentDictionary<int, Store> stores;
        private ConcurrentDictionary<int, ReaderWriterLock> storesLocks;
        private IExternalSystem ExternalSystem;
        private int STORE_COUNT = 0;
        private int PRODUCT_COUNT = 1;
        private DALMarketController dalMarketController;

        public MarketController(IUserController userController, IExternalSystem externalSystem)
        {
            this.userController = userController;
            this.orderHandler = new OrderHandler<int>();
            this.ExternalSystem = externalSystem;
            this.stores = new ConcurrentDictionary<int, Store>();
            this.storesLocks = new ConcurrentDictionary<int, ReaderWriterLock>();
            STORE_COUNT = 0;
            PRODUCT_COUNT = 1;
            this.dalMarketController = new DALMarketController(userController.ToDAL(), orderHandler.ToDAL(), new List<DALStore>(), STORE_COUNT, PRODUCT_COUNT);
            dalMarketController.userController = userController.ToDAL();
            DataHandler.Instance.Value.save(dalMarketController);
        }

        public MarketController(DALMarketController DALMarketController, IUserController userController, IExternalSystem externalSystem)
        {
            this.userController = userController;
            this.ExternalSystem = externalSystem;
            this.orderHandler = new OrderHandler<int>(DALMarketController.orderHandler);
            this.stores = new ConcurrentDictionary<int, Store>();
            this.storesLocks = new ConcurrentDictionary<int, ReaderWriterLock>();
            this.STORE_COUNT = DALMarketController.STORE_COUNT;
            this.PRODUCT_COUNT = DALMarketController.PRODUCT_COUNT;
            foreach (DALStore dal_store in DALMarketController.stores)
            {
                HashSet<Member> owners = new HashSet<Member>();
                foreach(MemberDAL owner in dal_store.Owners)
                {
                    owners.Add(userController.GetMember(owner.MemberName));
                }
                this.stores.TryAdd(dal_store.Id, new Store(dal_store, owners));
                ReaderWriterLock rwl = new ReaderWriterLock();
                this.storesLocks.TryAdd(dal_store.Id, rwl);
            }
            this.dalMarketController = DALMarketController;
        }

        public void InitializeSystem()
        {

            Logger.Instance.LogEvent("Started initializing the system - Market Controller");
            Logger.Instance.LogEvent("Finished initializing the system - Market Controller");

        }

        public DALMarketController ToDAL()
        {
            return this.dalMarketController;
        }

        private bool IsAuthorized(int userId, string username, int storeId, Action action)
        {
            userController.AssertCurrentUser(userId, username);
            return userController.IsAuthorized(username, storeId, action);
        }

        public StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} is trying to nominate {nominatedUsername} to be a store owner of store {storeId}");
            if (date > DateTime.Now)
            {
                throw new ArgumentException($"{date} is not a valid date: you are not from the future!");
            }
            Store store;
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
                store = stores[storeId];
            }
            catch
            {
                throw new ArgumentException($"Store ID does not exist: {storeId}");
            }

            userController.AssertCurrentUser(userId, nominatorUsername);
            Member nominator = userController.GetMember(nominatorUsername), nominated = userController.GetMember(nominatedUsername);

            if (!nominator.GetStoreRoles(storeId).Any(x => x is StoreOwner))
                throw new MemberAccessException($"Member {nominatorUsername} is not allowed to nominate owners in store #{storeId}.");

            if (nominatorUsername.Equals(nominatedUsername))
            {
                throw new InvalidOperationException($"Member {nominatorUsername} cannot nominate itself to be a store owner.");
            }
            Member original_voter;
            if ((original_voter = store.VoteForStoreOwnerNominee(nominator, nominated)) != null)
            {
                StoreOwner newRole = new StoreOwner(storeId);
                nominated.AddRole(newRole);
                store.AddOwner(nominated);
                store.RemoveVotingOnMember(nominated);
                // Add the new manager to the nominator's nominees list
                StoreRole nominatorStoreOwner = original_voter.GetStoreRoles(storeId).Last();
                nominatorStoreOwner.AddNominee(nominatedUsername, newRole);

                userController.RegisterToEvent(nominated.Username, new Event("RemoveStoreOwnerNominationFrom" + nominatedUsername, "", "MarketController"));
                userController.RegisterToEvent(nominated.Username, new Event("StoreOwnerVoting" + storeId, "", "MarketController"));
                userController.RegisterToEvent(nominated.Username, new Event("SaleInStore" + storeId, "", "MarketController"));
                userController.RegisterToEvent(nominated.Username, new Event("OpenStore" + storeId, "", "MarketController"));
                userController.RegisterToEvent(nominated.Username, new Event("CloseStore" + storeId, "", "MarketController"));
                userController.RegisterToEvent(nominated.Username, new Event("BidOfferInStore" + storeId, "", "MarketController"));
                userController.RegisterToEvent(nominated.Username, new Event("ReviewInStore" + storeId, "", "MarketController"));

                userController.UpdateUserStatistics(nominated, date);

                storesLocks[storeId].ReleaseReaderLock();
                Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} successfuly nominated member {nominatedUsername} as a store owner of store {storeId}");
                return newRole;
            }
            userController.notify(new Event("StoreOwnerVoting" + storeId, $"There is a store owner voting taking place in store {storeId} on member {nominatedUsername}", "MarketController"));
            return null;
        }

        public void RejectStoreOwnerNomination(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} is trying to reject the nomination of {nominatedUsername} to be a store owner of store {storeId}");
            Store store;
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
                store = stores[storeId];
            }
            catch
            {
                throw new ArgumentException($"Store ID does not exist: {storeId}");
            }

            userController.AssertCurrentUser(userId, nominatorUsername);
            Member nominator = userController.GetMember(nominatorUsername), nominated = userController.GetMember(nominatedUsername);

            store.RejectStoreOwnerNomination(nominator, nominated);

            storesLocks[storeId].ReleaseReaderLock();
        }

        public StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
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
            StoreManager storeManager = userController.NominateStoreManager(userId, nominatorUsername, nominatedUsername, storeId, date);
            storesLocks[storeId].ReleaseReaderLock();
            return storeManager;
        }

        public Member RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatorMembername} is trying to remove store owner nomination from {nominatedMembername} in store {storeId}.");
            userController.AssertCurrentUser(userId, nominatorMembername);
            return RemoveStoreOwnerNominationHelper(nominatorMembername, nominatedMembername, storeId);
        }

        private Member RemoveStoreOwnerNominationHelper(string nominator, string nominated, int storeId)
        {
            Logger.Instance.LogEvent($"Member {nominator} is trying to remove store owner nomination from {nominated} in store {storeId}.");
            Member nominatorMember = userController.GetMember(nominator);
            Member nominatedMember = userController.GetMember(nominated);
            StoreRole FOUND_NOMINATOR_ROLE = null;
            StoreRole FOUND_NOMINATED_ROLE = null;
            foreach (StoreRole nominatedRole in nominatedMember.GetStoreRoles(storeId))
            {
                foreach (StoreRole nominatorRole in nominatorMember.GetStoreRoles(storeId))
                {
                    if ((nominatedRole is StoreOwner || nominatedRole is StoreManager) && (nominatorRole is StoreManager || nominatorRole is StoreOwner) && nominatorRole.ContainsNominee(nominatedRole))
                    {
                        FOUND_NOMINATED_ROLE = nominatedRole;
                        FOUND_NOMINATOR_ROLE = nominatorRole;
                    }
                    break;
                }
                if (FOUND_NOMINATED_ROLE != null)
                {
                    break;
                }
            }
            if (FOUND_NOMINATED_ROLE == null)
            {
                throw new ArgumentException($"{nominator} did not nominate {nominated} to be a store owner in store {storeId}");
            }

            List<string> to_remove = FOUND_NOMINATED_ROLE.nominees.Keys.ToList();
            foreach (string k in to_remove)
            {
                RemoveStoreOwnerNominationHelper(nominated, k, storeId);
            }
            nominatedMember.RemoveRole(FOUND_NOMINATED_ROLE);
            FOUND_NOMINATOR_ROLE.RemoveNominee(FOUND_NOMINATED_ROLE);
            DataHandler.Instance.Value.remove(FOUND_NOMINATED_ROLE.ToDAL());
            stores[storeId].RemoveOwner(nominatedMember);
            userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("SaleInStore" + storeId, "", "MarketController"));
            userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("StoreOwnerVoting" + storeId, "", "MarketController"));
            userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("OpenStore" + storeId, "", "MarketController"));
            userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("CloseStore" + storeId, "", "MarketController"));
            userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("BidOfferInStore" + storeId, "", "MarketController"));
            userController.RemoveRegisterToEvent(nominatedMember.Username, new Event("ReviewInStore" + storeId, "", "MarketController"));
            userController.notify(new Event("RemoveStoreOwnerNominationFrom" + nominated, "Your store owner nomination has been removed by " + nominator, "MarketController"));
            Logger.Instance.LogEvent($"Member {nominator} successfuly removed store owner nomination from {nominated} in store {storeId}.");
            return nominatedMember;
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
                    else
                    {
                        throw new ArgumentException($"{manager} already has the permission for action {action}");
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
            Logger.Instance.LogEvent($"{username} is trying to add Product {name} to store {storeId}.");
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

            product = stores[storeId].AddProduct(name, Interlocked.Increment(ref PRODUCT_COUNT), description, price, quantity, category);
            this.dalMarketController.PRODUCT_COUNT = PRODUCT_COUNT;
            DataHandler.Instance.Value.update(this.dalMarketController);

            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly added Product {name} to store {storeId}.");
            return product;
        }

        public void RemoveProductFromStore(int userId, string username, int storeId, int productID)
        {
            Logger.Instance.LogEvent($"{username} is trying to remove Product {productID} to store {storeId}.");
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
            Logger.Instance.LogEvent($"{username} successfuly removed Product {productID} from store {storeId}.");
        }

        public void ChangeProductDescription(int userId, string username, int storeId, int productID, string description)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the description of Product {productID} in store {storeId}.");
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
            Logger.Instance.LogEvent($"{username} successfuly changed the description of Product {productID} in store {storeId}.");
        }

        public void ChangeProductName(int userId, string username, int storeId, int productID, string name)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the name of Product {productID} in store {storeId}.");
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
            Logger.Instance.LogEvent($"{username} successfuly changed the name of Product {productID} in store {storeId}.");
        }

        public void ChangeProductPrice(int userId, string username, int storeId, int productID, double price)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the OfferedPrice of Product {productID} in store {storeId}.");
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
            Logger.Instance.LogEvent($"{username} successfuly changed the OfferedPrice of Product {productID} in store {storeId}.");
        }

        public void ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the quantity of Product {productID} in store {storeId}.");
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
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductQuantity))
                throw new MemberAccessException("This user is not authorized for changing product's qunatity in the specified store.");
            stores[storeId].ChangeProductQuantity(productID, quantity);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the quantity of Product {productID} in store {storeId}.");
        }

        public void ChangeProductCategory(int userId, string username, int storeId, int productID, string category)
        {
            Logger.Instance.LogEvent($"{username} is trying to change the quantity of Product {productID} in store {storeId}.");
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
            if (!IsAuthorized(userId, username, storeId, Action.ChangeProductCategory))
                throw new MemberAccessException("This user is not authorized for changing product's category in the specified store.");
            stores[storeId].ChangeProductCategory(productID, category);
            storesLocks[storeId].ReleaseWriterLock();
            Logger.Instance.LogEvent($"{username} successfuly changed the quantity of Product {productID} in store {storeId}.");
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
                userController.notify(new Event("OpenStore" + storeId, "Store " + storeId + " was opened by user " + membername, "marketController"));
            }
            else
            {
                throw new ArgumentException($"Store {storeId} is already opened.");
            }
            storesLocks[storeId].ReleaseWriterLock();
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
                userController.notify(new Event("CloseStore" + storeId, "Store " + storeId + " was closed by user" + username, "marketController"));
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

        public Store CreateNewStore(int userId, string creator, string storeName, DateTime date)
        {
            Logger.Instance.LogEvent($"{creator} is trying to create a new store: \"{storeName}\".");
            userController.AssertCurrentUser(userId, creator);
            if (String.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException($"User {creator} requestted to create a store with an empty name.");
            }
            if (date > DateTime.Now)
            {
                throw new ArgumentException($"Entered date is not valid: {date}");
            }
            int storeId = Interlocked.Increment(ref STORE_COUNT);
            ReaderWriterLock rwl = new ReaderWriterLock();
            storesLocks[storeId] = rwl;
            rwl.AcquireWriterLock(Timeout.Infinite);
            userController.AddStoreFounder(creator, storeId, date);
            Store store = new Store(storeId, storeName, userController.GetMember(creator));
            stores[storeId] = store;

            this.dalMarketController.STORE_COUNT = STORE_COUNT;
            this.dalMarketController.stores.Add(store.ToDAL());
            DataHandler.Instance.Value.update(this.dalMarketController);

            rwl.ReleaseWriterLock();

            userController.RegisterToEvent(creator, new Event("SaleInStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("OpenStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("CloseStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("BidOfferInStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("StoreOwnerVoting" + storeId, "", "MarketController"));
            userController.RegisterToEvent(creator, new Event("ReviewInStore" + storeId, "", "MarketController"));
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

        public ShoppingCartDTO EditCart(int userId, int productId, int newQuantity)
        {
            Logger.Instance.LogEvent("User " + userId + " is trying to edit the quantity of " + productId + " in his cart");
            if (newQuantity < 0)
            {
                Logger.Instance.LogEvent("User " + userId + " failed to edit the quantity of " + productId + " in his cart");
                throw new ArgumentException($"Quantity {newQuantity} can not be a negtive number");
            }
            User user = userController.GetUser(userId);

            int storeId = user.GetStoreOfProduct(productId);
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }

            try
            {
                int userQuantity = user.GetQuantityInCart(productId);

                if (newQuantity == 0)
                {
                    stores[storeId].AddToProductQuantity(productId, userQuantity);
                    user.deleteFromCart(productId);
                }
                else if (newQuantity > userQuantity)
                {
                    stores[storeId].RemoveFromProductQuantity(productId, newQuantity - userQuantity);
                    user.changeQuantityInCart(productId, newQuantity);
                }
                else
                {
                    stores[storeId].AddToProductQuantity(productId, userQuantity);
                    user.changeQuantityInCart(productId, newQuantity);
                }
            }
            catch (Exception e)
            {
                storesLocks[storeId].ReleaseWriterLock();
                throw e;
            }
            Logger.Instance.LogEvent("User " + userId + " successfuly edited the quantity of " + productId + " in his cart");
            return user.ViewShoppingCart();
        }

        public double BuyCart(int userId, CreditCard cc, SupplyAddress address, DateTime buyTime)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to buy his cart.");
            ShoppingCartDTO shoppingCart = userController.viewCart(userId);
            if (shoppingCart.IsEmpty())
            {
                throw new InvalidOperationException("Can't buy an empty shopping cart");
            }

            if (buyTime > DateTime.Now)
            {
                throw new ArgumentException("Can not purchase from the future!");
            }


            Dictionary<int, List<ProductDTO>> productsSoFar = new Dictionary<int, List<ProductDTO>>();
            List<Event> events = new List<Event>();
            Dictionary<int, List<OrderDTO>> storeOrdersSoFar = new Dictionary<int, List<OrderDTO>>();
            Dictionary<string, List<OrderDTO>> userOrdersSoFar = new Dictionary<string, List<OrderDTO>>();
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
                    User currentUser = userController.GetUser(userId);
                    string username = currentUser is Member ? ((Member)currentUser).Username : "A guest";

                    events.Add(new Event("SaleInStore" + storeId, $"Prouducts with the name {String.Join(" ", bag.products.Select(p => p.Name).ToArray())} were bought from store {storeId} by {username}", "marketController"));
                    productsSoFar.Add(storeId, bag.products);
                    OrderDTO order = orderHandler.CreateOrder(username, address, stores[storeId].GetId(), bag.products, buyTime, stores[storeId].CalaculatePrice(bag));
                    if (storeOrdersSoFar.ContainsKey(storeId))
                    {
                        storeOrdersSoFar[storeId].Add(order);
                    }
                    else
                    {
                        storeOrdersSoFar.Add(storeId, new List<OrderDTO>() { order });
                    }
                    if (userOrdersSoFar.ContainsKey(username))
                    {
                        userOrdersSoFar[username].Add(order);
                    }
                    else
                    {
                        userOrdersSoFar.Add(username, new List<OrderDTO>() { order });
                    }

                    storesLocks[storeId].ReleaseWriterLock();
                }
                catch (Exception e)
                {
                    storesLocks[storeId].ReleaseWriterLock();
                    foreach (int sid in productsSoFar.Keys)
                    {
                        storesLocks[sid].AcquireWriterLock(Timeout.Infinite);
                        productsSoFar[sid].ForEach(p => stores[sid].restoreProduct(p));
                        storesLocks[sid].ReleaseWriterLock();
                    }
                    throw e;
                }
            }
            if (ExternalSystem.IsExternalSystemOnline())
            {
                int pay_trans_id = ExternalSystem.Pay(cc.Card_number, cc.Month, cc.Year, cc.Holder, cc.Ccv, cc.Id);
                if (pay_trans_id != -1)
                {
                    int supply_trans_id = ExternalSystem.Supply(address.Name, address.Address, address.City, address.Country, address.Zip);
                    if (supply_trans_id != -1)
                    {
                        double cartPrice = GetCartPrice(shoppingCart);
                        events.ForEach(e => userController.notify(e));
                        foreach (int storeId in storeOrdersSoFar.Keys)
                        {
                            foreach (OrderDTO order in storeOrdersSoFar[storeId])
                            {
                                DataHandler.Instance.Value.save(order.ToDAL());
                                orderHandler.addOrder(order, storeId);
                            }
                        }
                        foreach (string username in userOrdersSoFar.Keys)
                        {
                            foreach (OrderDTO order in userOrdersSoFar[username])
                            {

                                userController.AddOrder(userId, order, username);
                            }
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
            }
            foreach (int sid in productsSoFar.Keys)
            {
                storesLocks[sid].AcquireWriterLock(Timeout.Infinite);
                productsSoFar[sid].ForEach(p => stores[sid].restoreProduct(p));
                storesLocks[sid].ReleaseWriterLock();
            }
            throw new ArgumentException("Buying cart failed due to failures with the external system we're using!");
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

        public ShoppingBagProduct AddToCart(int userId, int productId, int storeId, int quantity)
        {
            ShoppingBagProduct product = null;
            if (quantity <= 0)
            {
                throw new ArgumentException($"Can't add {quantity} of an item to the shopping cart");
            }
            try
            {
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException("Store ID does not exist");
            }
            try
            {
                User u = userController.GetUser(userId);
                product = stores[storeId].GetProductForSale(productId, quantity);
                u.AddToCart(product, storeId);
                storesLocks[storeId].ReleaseWriterLock();
            }
            catch (Exception e)
            {
                storesLocks[storeId].ReleaseWriterLock();
                throw e;
            }
            return product;
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

        private double GetDailyIncome(List<Store> stores)
        {
            double count = 0;
            foreach (Store store in stores)
            {
                List<OrderDTO> orders = orderHandler.GetOrders(store.GetId());
                foreach (OrderDTO order in orders)
                {
                    if (order.date.Date == DateTime.Today)
                    {
                        count += order.price;
                    }
                }
            }
            return count;
        }

        public double GetDailyIncomeStoreOwner(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            Member member = userController.GetMember(username);
            foreach (StoreRole role in member.GetStoreRoles(storeId))
            {
                if (role.GetType() == typeof(StoreOwner) | role.GetType() == typeof(StoreFounder))
                {
                    List<Store> stores = new List<Store>();
                    stores.Add(this.stores[storeId]);
                    return GetDailyIncome(stores);
                }
            }
            throw new ArgumentException("user " + username + " is not a store owner of store " + storeId);

        }

        public double GetDailyIncomeMarketManager(int userId, string username)
        {
            userController.AssertCurrentUser(userId, username);
            Member member = userController.GetMember(username);
            foreach (Role role in member.GetAllRoles())
            {
                if (role.GetType() == typeof(MarketManager))
                {
                    List<Store> stores = this.stores.Values.ToList();
                    return GetDailyIncome(stores);
                }
            }
            throw new ArgumentException("user " + username + " is not a market manager");

        }

        private double BuyProduct(int userId, CreditCard cc, SupplyAddress address, DateTime buyTime,Product product, double price, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to buy {product.Name} in OfferedPrice of {price}.");
            
            if (buyTime > DateTime.Now)
            {
                throw new ArgumentException("Can not purchase from the future!");
            }

            List<ProductDTO> Products = new List<ProductDTO>();
            Products.Add(product.GetProductDTO());
            List<Event> events = new List<Event>();
            Dictionary<int, List<OrderDTO>> storeOrdersSoFar = new Dictionary<int, List<OrderDTO>>();
            Dictionary<string, List<OrderDTO>> userOrdersSoFar = new Dictionary<string, List<OrderDTO>>();
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
                User currentUser = userController.GetUser(userId);
                string username = currentUser is Member ? ((Member)currentUser).Username : "A guest";

                events.Add(new Event("SaleInStore" + storeId, $"Prouduct with the name {product.Name} were bought from store {storeId} by {username}", "marketController"));
                OrderDTO order = orderHandler.CreateOrder(username, address, stores[storeId].GetId(), Products, buyTime, price);
                if (storeOrdersSoFar.ContainsKey(storeId))
                {
                    storeOrdersSoFar[storeId].Add(order);
                }
                else
                {
                    storeOrdersSoFar.Add(storeId, new List<OrderDTO>() { order });
                }
                if (userOrdersSoFar.ContainsKey(username))
                {
                    userOrdersSoFar[username].Add(order);
                }
                else
                {
                    userOrdersSoFar.Add(username, new List<OrderDTO>() { order });
                }

                storesLocks[storeId].ReleaseWriterLock();
            }
            catch (Exception e)
            {
                storesLocks[storeId].ReleaseWriterLock();
                storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
                stores[storeId].restoreProduct(product.GetProductDTO());
                storesLocks[storeId].ReleaseWriterLock();
                throw e;
            }
            
            if (ExternalSystem.IsExternalSystemOnline())
            {
                int pay_trans_id = ExternalSystem.Pay(cc.Card_number, cc.Month, cc.Year, cc.Holder, cc.Ccv, cc.Id);
                if (pay_trans_id != -1)
                {
                    int supply_trans_id = ExternalSystem.Supply(address.Name, address.Address, address.City, address.Country, address.Zip);
                    if (supply_trans_id != -1)
                    {
                        double cartPrice = price;
                        events.ForEach(e => userController.notify(e));

                        
                        foreach (OrderDTO order in storeOrdersSoFar[storeId])
                        {
                            orderHandler.addOrder(order, storeId);
                        }
                        foreach (string username in userOrdersSoFar.Keys)
                        {
                            foreach (OrderDTO order in userOrdersSoFar[username])
                            {

                                userController.AddOrder(userId, order, username);
                            }
                        }

                        Logger.Instance.LogEvent($"User {userId} successfuly paid {price} and purchased his Product.");
                        return cartPrice;
                    }
                    else
                    {
                        ExternalSystem.Cancel_Pay(pay_trans_id);
                    }
                }
            }
            storesLocks[storeId].AcquireWriterLock(Timeout.Infinite);
            stores[storeId].restoreProduct(product.GetProductDTO());
            storesLocks[storeId].ReleaseWriterLock();
            throw new ArgumentException("Buying Product failed due to failures with the external system we're using!");
        }

        public Bid OfferBid(int userId, string username, int storeId, int productId, double price)
        {
            userController.AssertCurrentUser(userId, username);
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException($"No such store {storeId}");
            }

            Product product = stores[storeId].GetProduct(productId);
            int bidId = stores[storeId].OfferBid(username, storeId, product, price);

            userController.notify(new Event("BidOfferInStore"+ storeId, $"There is a new bid offer with id {bidId} in store {storeId} to product {product.Name}, for {price} shekels", "MarketController"));
            
            userController.RegisterToEvent(username, new Event("BidAccept" + bidId+ "OfStore"+storeId, "", "MarketController"));
            userController.RegisterToEvent(username, new Event("BidReject" + bidId + "OfStore" + storeId, "", "MarketController"));
            userController.RegisterToEvent(username, new Event("BidCounter" + bidId + "OfStore" + storeId, "", "MarketController"));

            storesLocks[storeId].ReleaseReaderLock();

            return stores[storeId].biding_votes[bidId];
        }

        public Bid CounterBid(int userId, string membername, int storeId, int bidId, double newPrice)
        {
            userController.AssertCurrentUser(userId, membername);
            try
            {
                storesLocks[storeId].AcquireReaderLock(Timeout.Infinite);
            }
            catch
            {
                throw new ArgumentException($"No such store {storeId}");
            }
            foreach (StoreRole role in userController.GetMember(membername).GetStoreRoles(storeId))
            {
                if (role is StoreOwner)
                {
                    stores[storeId].ChangeBidPrice(bidId, newPrice);
                    Bid oldBid = stores[storeId].biding_votes[bidId];
                    oldBid.CounterOfferred = true;
                    userController.notify(new Event("BidCounter" + bidId + "OfStore" + storeId, "The counter offer to your bid on product " + oldBid.Product.Name + " in store " + storeId + " is " + newPrice, "MarketController"));
                    storesLocks[storeId].ReleaseReaderLock();
                    return oldBid;
                }
            }
            storesLocks[storeId].ReleaseReaderLock();
            throw new ArgumentException($"{membername} is not a store owner of store {storeId} hence he can not give counter offers to bids in the store.");
        }

        public Bid VoteForBid(int userId, string username, int storeId, int bidId, bool vote)
        {
            userController.AssertCurrentUser(userId, username);
            Member member = userController.GetMember(username);
            foreach (StoreRole role in member.GetStoreRoles(storeId))
            {
                if (role is StoreOwner)
                {
                    Bid bid = stores[storeId].biding_votes[bidId];
                    if (stores[storeId].VoteForBid(member, vote, bidId)) // Bid was accepted
                    {
                        userController.notify(new Event("BidAccept" + bidId + "OfStore" + storeId, "Bid offer in store " + storeId + " to the Product " + bid.Product.Name + " in OfferedPrice of " + bid.OfferedPrice + " is accepted", "MarketController"));
                    }
                    else if (!vote)
                    {
                        stores[storeId].RemoveBid(bidId);
                        userController.notify(new Event("BidReject" + bidId + "OfStore" + storeId, "Bid offer in store " + storeId + " to the Product " + bid.Product.Name + " in OfferedPrice of " + bid.OfferedPrice + " is rejected", "MarketController"));
                    }
                    return bid;
                }
            }
            throw new ArgumentException("user " + username + " is not a store owner of store " + storeId);
        }

        public double BuyBidProduct(int userId, string username, int storeId, int bidId, CreditCard cc, SupplyAddress address, DateTime buyTime)
        {
            userController.AssertCurrentUser(userId, username);
            Store store = stores[storeId];
            if (store.CanBuyBid(username, bidId)){
                Bid bid = store.biding_votes[bidId];
                double retValue = BuyProduct(userId, cc, address, buyTime, bid.Product, bid.OfferedPrice, storeId);
                store.RemoveBid(bidId);
                return retValue;
            }
            throw new ArgumentException($"{username} can not buy bid {bidId} since it's not accepted by all store owners or he is not the one who submitted the bid.");
        }

        public List<Bid> GetBidsStatus(int userId, string username, int storeId)
        {
            userController.AssertCurrentUser(userId, username);
            Member member = userController.GetMember(username);
            foreach (StoreRole role in member.GetStoreRoles(storeId))
            {
                if (role is StoreOwner)
                {
                    return stores[storeId].biding_votes.Select(kvp => kvp.Value).ToList();
                }
            }
            throw new ArgumentException($"{username} is not a store owner of store {storeId} and can not view the status of bids in it");
        }

        public List<OrderDTO> GetStorePurchaseHistory(int userId, string membername, int storeId)
        {
            userController.AssertCurrentUser(userId, membername);
            Member member = userController.GetMember(membername);
            if (!member.GetAllRoles().Any(r => r is MarketManager) && !member.GetStoreRoles(storeId).Any(r => r.IsAuthorized(Action.ViewStorePurchaseHistory)))
            {
                throw new ArgumentException($"{membername} is not authorized to view the purchase history of store {storeId}");
            }
            return orderHandler.GetOrders(storeId);
        }

    }
}
