using System;
using System.Collections.Generic;
using System.Linq;
using Workshop.DomainLayer;
using Context = Workshop.DataLayer.Context;
using Workshop.ServiceLayer.ServiceObjects;
using DomainUser = Workshop.DomainLayer.UserPackage.User;
using DomainMember = Workshop.DomainLayer.UserPackage.Permissions.Member;
using DomainProduct = Workshop.DomainLayer.MarketPackage.Product;
using DomainProductDTO = Workshop.DomainLayer.MarketPackage.ProductDTO;
using DomainStoreManager = Workshop.DomainLayer.UserPackage.Permissions.StoreManager;
using DomainStoreOwner = Workshop.DomainLayer.UserPackage.Permissions.StoreOwner;
using DomainStore = Workshop.DomainLayer.MarketPackage.Store;
using DomainNotification = Workshop.DomainLayer.UserPackage.Notifications.Notification;
using CreditCard = Workshop.DomainLayer.MarketPackage.CreditCard;
using SupplyAddress = Workshop.DomainLayer.MarketPackage.SupplyAddress;
using DomainShoppingCart = Workshop.DomainLayer.UserPackage.Shopping.ShoppingCartDTO;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.Loggers;
using System.Globalization;
using System.IO;
using Moq;

namespace Workshop.ServiceLayer
{
    public class Service : IService
    {
        private Facade facade;
        public readonly bool WasInitializedWithFile;
        private readonly int Port;
        public Service(IExternalSystem externalSystem, string conf)
        {
            string starting_state_file = null;
            bool USE_DB = false;
            bool USE_EXTERNAL_SYSTEM = false;
            List<SystemAdminDTO> systemManagers = new List<SystemAdminDTO>();

            Func<string, bool> parse_ss = (ssfile) =>
            {
                string initializationState = "";
                try
                {
                    using (StreamReader streamReader = File.OpenText(ssfile))
                    {
                        initializationState = streamReader.ReadToEnd();
                    }
                }
                catch
                {
                    Logger.Instance.LogError("Starting state file does not exist");
                }
                facade = new Facade(USE_EXTERNAL_SYSTEM ? new ExternalSystem() : externalSystem, systemManagers);
                try
                {
                    foreach (string command in initializationState.Split('\n'))
                    {
                        string newcommand = command.Replace("\r", string.Empty);
                        string[] splits = newcommand.Split('(', ')');
                        if (splits.Length != 3 || splits[2].Trim().Length > 0)
                        {
                            throw new ArgumentException($"Command was in incorrect form: {newcommand}");
                        }
                        string[] actualParams = splits[1].Split(',');
                        try
                        {
                            switch (splits[0])
                            {
                                case "enter-market":
                                    if (actualParams.Length != 2) { throw new ArgumentException(); }
                                    facade.EnterMarket(int.Parse(actualParams[0]), DateTime.ParseExact(actualParams[1], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "exit-market":
                                    if (actualParams.Length != 1) { throw new ArgumentException(); }
                                    facade.ExitMarket(int.Parse(actualParams[0]));
                                    break;
                                case "register":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.Register(int.Parse(actualParams[0]), actualParams[1], actualParams[2], DateTime.ParseExact(actualParams[3], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "login":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.Login(int.Parse(actualParams[0]), actualParams[1], actualParams[2], DateTime.ParseExact(actualParams[3], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "logout":
                                    if (actualParams.Length != 2) { throw new ArgumentException(); }
                                    facade.Logout(int.Parse(actualParams[0]), actualParams[1]);
                                    break;
                                case "add-product":
                                    if (actualParams.Length != 8) { throw new ArgumentException(); }
                                    facade.AddProduct(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], actualParams[4], double.Parse(actualParams[5]), int.Parse(actualParams[6]), actualParams[7]);
                                    break;
                                case "nominate-store-manager":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.NominateStoreManager(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]), DateTime.ParseExact(actualParams[4], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "nominate-store-owner":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.NominateStoreOwner(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]), DateTime.ParseExact(actualParams[4], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "remove-store-owner-nomination":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.RemoveStoreOwnerNomination(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]));
                                    break;
                                case "get-workers-information":
                                    if (actualParams.Length != 3) { throw new ArgumentException(); }
                                    facade.GetWorkersInformation(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]));
                                    break;
                                case "close-store":
                                    if (actualParams.Length != 3) { throw new ArgumentException(); }
                                    facade.CloseStore(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]));
                                    break;
                                case "open-store":
                                    if (actualParams.Length != 3) { throw new ArgumentException(); }
                                    facade.OpenStore(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]));
                                    break;
                                case "create-new-store":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.CreateNewStore(int.Parse(actualParams[0]), actualParams[1], actualParams[2], DateTime.ParseExact(actualParams[3], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "review-Product":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.ReviewProduct(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], int.Parse(actualParams[4]));
                                    break;
                                case "search-Product":
                                    if (actualParams.Length != 6) { throw new ArgumentException(); }
                                    facade.SearchProduct(int.Parse(actualParams[0]), actualParams[1], actualParams[2], double.Parse(actualParams[3]), double.Parse(actualParams[4]), double.Parse(actualParams[5]));
                                    break;
                                case "get-all-stores":
                                    if (actualParams.Length != 1) { throw new ArgumentException(); }
                                    facade.GetAllStores(int.Parse(actualParams[0]));
                                    break;
                                case "add-to-cart":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.AddToCart(int.Parse(actualParams[0]), int.Parse(actualParams[1]), int.Parse(actualParams[2]), int.Parse(actualParams[3]));
                                    break;
                                case "view-cart":
                                    if (actualParams.Length != 1) { throw new ArgumentException(); }
                                    facade.ViewCart(int.Parse(actualParams[0]));
                                    break;
                                case "edit-cart":
                                    if (actualParams.Length != 3) { throw new ArgumentException(); }
                                    facade.EditCart(int.Parse(actualParams[0]), int.Parse(actualParams[1]), int.Parse(actualParams[2]));
                                    break;
                                case "buy-cart":
                                    if (actualParams.Length != 13) { throw new ArgumentException(); }
                                    CreditCard cc = new CreditCard(actualParams[1], actualParams[2], actualParams[3], actualParams[4], actualParams[5], actualParams[6]);
                                    SupplyAddress address = new SupplyAddress(actualParams[7], actualParams[8], actualParams[9], actualParams[10], actualParams[11]);
                                    facade.BuyCart(int.Parse(actualParams[0]), cc, address, DateTime.ParseExact(actualParams[12], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "add-product-discount":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.AddProductDiscount(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], int.Parse(actualParams[4]));
                                    break;
                                case "add-category-discount":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.AddCategoryDiscount(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], actualParams[4]);
                                    break;
                                case "add-store-discount":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.AddStoreDiscount(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3]);
                                    break;
                                case "remove-product-from-store":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.RemoveProductFromStore(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]));
                                    break;
                                case "change-product-name":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.ChangeProductName(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), actualParams[4]);
                                    break;
                                case "change-product-OfferedPrice":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.ChangeProductPrice(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), double.Parse(actualParams[4]));
                                    break;
                                case "change-product-quantity":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.ChangeProductQuantity(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), int.Parse(actualParams[4]));
                                    break;
                                case "change-product-category":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.ChangeProductCategory(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), actualParams[4]);
                                    break;
                                case "add-product-purchase-term":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.AddProducPurchaseTerm(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], int.Parse(actualParams[4]));
                                    break;
                                case "add-category-purchase-term":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.AddCategoryPurchaseTerm(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], actualParams[4]);
                                    break;
                                case "add-store-purchase-term":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.AddStorePurchaseTerm(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3]);
                                    break;
                                case "add-user-purchase-term":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.AddUserPurchaseTerm(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3]);
                                    break;
                                case "add-action-to-manager":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.AddActionToManager(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]), actualParams[4]);
                                    break;
                                case "take-notifications":
                                    if (actualParams.Length != 2) { throw new ArgumentException(); }
                                    facade.TakeNotifications(int.Parse(actualParams[0]), actualParams[1]);
                                    break;
                                case "get-members-online-stats":
                                    if (actualParams.Length != 2) { throw new ArgumentException(); }
                                    facade.GetMembersOnlineStats(int.Parse(actualParams[0]), actualParams[1]);
                                    break;
                                case "cancel-member":
                                    if (actualParams.Length != 3) { throw new ArgumentException(); }
                                    facade.CancelMember(int.Parse(actualParams[0]), actualParams[1], actualParams[2]);
                                    break;
                                case "get-member-permissions":
                                    if (actualParams.Length != 2) { throw new ArgumentException(); }
                                    facade.GetMemberPermissions(int.Parse(actualParams[0]), actualParams[1]);
                                    break;
                                case "reject-store-owner-nomination":
                                    if (actualParams.Length != 4) { throw new ArgumentException(); }
                                    facade.RejectStoreOwnerNomination(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]));
                                    break;
                                case "offer-bid":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.OfferBid(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), double.Parse(actualParams[4]));
                                    break;
                                case "counter-bid":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.CounterBid(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), double.Parse(actualParams[4]));
                                    break;
                                case "vote-for-bid":
                                    if (actualParams.Length != 5) { throw new ArgumentException(); }
                                    facade.VoteForBid(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), bool.Parse(actualParams[4]));
                                    break;
                                case "buy-bid-product":
                                    if (actualParams.Length != 16) { throw new ArgumentException(); }
                                    CreditCard cc1 = new CreditCard(actualParams[1], actualParams[2], actualParams[3], actualParams[4], actualParams[5], actualParams[6]);
                                    SupplyAddress address1 = new SupplyAddress(actualParams[7], actualParams[8], actualParams[9], actualParams[10], actualParams[11]);
                                    facade.BuyBidProduct(int.Parse(actualParams[0]), actualParams[12], int.Parse(actualParams[13]), int.Parse(actualParams[14]), cc1, address1, DateTime.ParseExact(actualParams[15], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    break;
                                case "get-bids-status":
                                    if (actualParams.Length != 3) { throw new ArgumentException(); }
                                    facade.GetBidsStatus(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]));
                                    break;
                                default:
                                    throw new ArgumentException();
                            }
                        }
                        catch
                        {
                            throw new ArgumentException($"Command was not in the correct format: {newcommand}");
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    this.facade = null;
                    Logger.Instance.LogError($"Initialization from file failed: \n{ex.Message}.\nSystem is in default condition.");
                    return false;
                }
            };

            foreach (string entry in conf.Split('\n'))
            {
                string[] parts = entry.Split('~');
                parts[parts.Length - 1] = parts[parts.Length - 1].Replace("\r", string.Empty);
                switch (parts[0])
                {
                    case "admin":
                        if (parts.Length != 4) throw new ArgumentException($"admin is not in the correct format: {entry}");
                        try { systemManagers.Add(new SystemAdminDTO(parts[1], parts[2], parts[3])); }
                        catch
                        {
                            throw new ArgumentException($"admin is not in the correct format: {entry}");
                        }
                        break;
                    case "ss":
                        if (parts.Length != 2) throw new ArgumentException($"starting state is not in the correct format: {entry}");
                        starting_state_file = parts[1];
                        break;
                    case "port":
                        if (parts.Length != 2) throw new ArgumentException($"port is not in the correct format: {entry}");
                        if (!int.TryParse(parts[1], out Port)) throw new ArgumentException($"port is not in the correct format: {entry}");
                        break;
                    case "db":
                        if (parts.Length != 1) throw new ArgumentException();
                        USE_DB = true;
                        break;
                    case "es":
                        if (parts.Length != 1) throw new ArgumentException();
                        USE_EXTERNAL_SYSTEM = true;
                        break;
                    default:
                        throw new ArgumentException("Unidentified command in config file");
                }
            }
            if (systemManagers.Count == 0)
            {
                throw new ArgumentException($"The system needs to have at least one admin! Incorrect config file!");
            }
            WasInitializedWithFile = starting_state_file != null ? parse_ss(starting_state_file) : false;
            if (facade == null)
            {
                facade = new Facade(USE_EXTERNAL_SYSTEM ? new ExternalSystem() : externalSystem, systemManagers);
            }
            Context.USE_DB = USE_DB;
        }

        public Service(IExternalSystem externalSystem)
        {
            List<SystemAdminDTO> systemManagers = new List<SystemAdminDTO>();
            systemManagers.Add(new SystemAdminDTO("admin", "admin", "22/08/1972"));
            facade = new Facade(externalSystem, systemManagers);
            Context.USE_DB = false;
        }

        public int GetPort()
        {
            return Port;
        }

        public Response<User> EnterMarket(int userId, DateTime date)
        {
            try
            {
                DomainUser domainUser = facade.EnterMarket(userId, date);
                User serviceUser = new User(domainUser);
                return new Response<User>(serviceUser, userId);
            }
            catch (Exception e)
            {
                return new Response<User>(e.Message, userId);
            }
        }

        public Response ExitMarket(int userId)
        {
            try
            {
                facade.ExitMarket(userId);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response Register(int userId, string username, string password, DateTime birthdate)
        {
            try
            {
                facade.Register(userId, username, password, birthdate);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response<KeyValuePair<Member, List<Notification>>> Login(int userId, string username, string password, DateTime date)
        {
            try
            {
                KeyValuePair<DomainMember, List<DomainNotification>> domainAnswer = facade.Login(userId, username, password, date);
                Member serviceMember = new Member(domainAnswer.Key);
                List<Notification> notificationList = domainAnswer.Value.Select(n => new Notification(n)).ToList();
                return new Response<KeyValuePair<Member, List<Notification>>>(new KeyValuePair<Member, List<Notification>>(serviceMember, notificationList), userId);
            }
            catch (Exception e)
            {
                return new Response<KeyValuePair<Member, List<Notification>>>(e.Message, userId);
            }
        }

        public Response Logout(int userId, string membername)
        {
            try
            {
                facade.Logout(userId, membername);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response<Product> AddProduct(int userId, string membername, int storeId, string productName, string description, double price, int quantity, string category)
        {
            try
            {
                DomainProduct domainProduct = facade.AddProduct(userId, membername, storeId, productName, description, price, quantity, category);
                Product serviceProduct = new Product(domainProduct);
                return new Response<Product>(serviceProduct, userId);
            }
            catch (Exception e)
            {
                return new Response<Product>(e.Message, userId);
            }
        }

        public Response<StoreOwner> NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
        {
            try
            {
                DomainStoreOwner domainOwner = facade.NominateStoreOwner(userId, nominatorUsername, nominatedUsername, storeId, date);
                StoreOwner serviceOwner = domainOwner == null ? null : new StoreOwner(domainOwner);
                return new Response<StoreOwner>(serviceOwner, userId);
            }
            catch (Exception e)
            {
                return new Response<StoreOwner>(e.Message, userId);
            }
        }

        public Response RejectStoreOwnerNomination(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            try
            {
                facade.RejectStoreOwnerNomination(userId, nominatorUsername, nominatedUsername, storeId);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response<StoreOwner>(e.Message, userId);
            }
        }

        public Response<StoreManager> NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
        {
            try
            {
                DomainStoreManager domainManager = facade.NominateStoreManager(userId, nominatorUsername, nominatedUsername, storeId, date);
                StoreManager serviceManager = new StoreManager(domainManager);
                return new Response<StoreManager>(serviceManager, userId);
            }
            catch (Exception e)
            {
                return new Response<StoreManager>(e.Message, userId);
            }
        }

        public Response<Member> RemoveStoreOwnerNomination(int userId, string nominatorMembername, string nominatedMembername, int storeId)
        {
            try
            {
                DomainMember domainMember = facade.RemoveStoreOwnerNomination(userId, nominatorMembername, nominatedMembername, storeId);
                Member serviceMember = new Member(domainMember);
                return new Response<Member>(serviceMember, userId);
            }
            catch (Exception e)
            {
                return new Response<Member>(e.Message, userId);
            }

        }
        public Response AddActionToManager(int userId, string owner, string manager, int storeId, string action)
        {
            try
            {
                facade.AddActionToManager(userId, owner, manager, storeId, action);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }


        public Response<List<Member>> GetWorkersInformation(int userId, string username, int storeId)
        {
            try
            {
                List<DomainMember> members = facade.GetWorkersInformation(userId, username, storeId);
                List<Member> returnMembers = members.Select(x => new Member(x)).ToList();
                return new Response<List<Member>>(returnMembers, userId);
            }
            catch (Exception e)
            {
                return new Response<List<Member>>(e.Message, userId);
            }
        }
        public Response CloseStore(int userId, string username, int storeId)
        {
            try
            {
                facade.CloseStore(userId, username, storeId);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }
        public Response OpenStore(int userId, string username, int storeId)
        {
            try
            {
                facade.OpenStore(userId, username, storeId);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response<Store> CreateNewStore(int userId, string creator, string storeName, DateTime date)
        {
            try
            {
                DomainStore domainStore = facade.CreateNewStore(userId, creator, storeName, date);
                Store store = new Store(domainStore);
                return new Response<Store>(store, userId);
            }
            catch (Exception e)
            {
                return new Response<Store>(e.Message, userId);
            }

        }

        public Response<ReviewDTO> ReviewProduct(int userId, string user, int productId, string review, int rating)
        {
            try
            {
                return new Response<ReviewDTO>(facade.ReviewProduct(userId, user, productId, review, rating), userId);
            }
            catch (Exception e)
            {
                return new Response<ReviewDTO>(e.Message, userId);
            }
        }

        public Response<List<Product>> SearchProduct(int userId, string keyWords, string category, double minPrice, double maxPrice, double productReview)
        {
            try
            {
                List<DomainProductDTO> products = facade.SearchProduct(userId, keyWords, category, minPrice, maxPrice, productReview);
                List<Product> returnProducts = products.Select(x => new Product(x)).ToList();
                return new Response<List<Product>>(returnProducts, userId);
            }
            catch (Exception e)
            {
                return new Response<List<Product>>(e.Message, userId);
            }
        }

        public Response<Product> AddToCart(int userId, int productId, int storeId, int quantity)
        {
            try
            {
                Product product = new Product(facade.AddToCart(userId, productId, storeId, quantity));
                return new Response<Product>(product, userId);
            }
            catch (Exception e)
            {
                return new Response<Product>(e.Message, userId);
            }
        }

        public Response<ShoppingCart> ViewCart(int userId)
        {
            try
            {
                DomainShoppingCart shoppingCartDTO = facade.ViewCart(userId);
                ShoppingCart shoppingCart = new ShoppingCart(shoppingCartDTO);
                shoppingCart.Price = facade.GetCartPrice(shoppingCartDTO);
                return new Response<ShoppingCart>(shoppingCart, userId);
            }
            catch (Exception e)
            {
                return new Response<ShoppingCart>(e.Message, userId);
            }
        }

        public Response<ShoppingCart> EditCart(int userId, int productId, int newQuantity)
        {
            try
            {
                DomainShoppingCart shoppingCartDTO = facade.EditCart(userId, productId, newQuantity);
                ShoppingCart shoppingCart = new ShoppingCart(shoppingCartDTO);
                shoppingCart.Price = facade.GetCartPrice(shoppingCartDTO);
                return new Response<ShoppingCart>(shoppingCart, userId);
            }
            catch (Exception e)
            {
                return new Response<ShoppingCart>(e.Message, userId);
            }
        }

        public Response<double> BuyCart(int userId, CreditCard cc, SupplyAddress address, DateTime buyTime)
        {
            try
            {
                return new Response<double>(facade.BuyCart(userId, cc, address, buyTime), userId);
            }
            catch (Exception e)
            {
                return new Response<double>(e.Message, userId);
            }
        }

        public Response AddProductDiscount(int userId, string user, int storeId, string jsonDiscount, int productId)
        {
            try
            {
                facade.AddProductDiscount(userId, user, storeId, jsonDiscount, productId);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response AddCategoryDiscount(int userId, string user, int storeId, string jsonDiscount, string categoryName)
        {
            try
            {
                facade.AddCategoryDiscount(userId, user, storeId, jsonDiscount, categoryName);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response AddStoreDiscount(int userId, string user, int storeId, string jsonDiscount)
        {
            try
            {
                facade.AddStoreDiscount(userId, user, storeId, jsonDiscount);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response AddProductPurchaseTerm(int userId, string user, int storeId, string jsonTerm, int productId)
        {
            try
            {
                facade.AddProducPurchaseTerm(userId, user, storeId, jsonTerm, productId);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response AddCategoryPurchaseTerm(int userId, string user, int storeId, string jsonTerm, string category)
        {
            try
            {
                facade.AddCategoryPurchaseTerm(userId, user, storeId, jsonTerm, category);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response AddStorePurchaseTerm(int userId, string user, int storeId, string jsonTerm)
        {
            try
            {
                facade.AddStorePurchaseTerm(userId, user, storeId, jsonTerm);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response AddUserPurchaseTerm(int userId, string user, int storeId, string jsonTerm)
        {
            try
            {
                facade.AddUserPurchaseTerm(userId, user, storeId, jsonTerm);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response RemoveProductFromStore(int userId, string username, int storeId, int productID)
        {
            try
            {
                facade.RemoveProductFromStore(userId, username, storeId, productID);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response ChangeProductName(int userId, string username, int storeId, int productID, string name)
        {
            try
            {
                facade.ChangeProductName(userId, username, storeId, productID, name);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response ChangeProductPrice(int userId, string username, int storeId, int productID, double price)
        {
            try
            {
                facade.ChangeProductPrice(userId, username, storeId, productID, price);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response ChangeProductQuantity(int userId, string username, int storeId, int productID, int quantity)
        {
            try
            {
                facade.ChangeProductQuantity(userId, username, storeId, productID, quantity);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response ChangeProductCategory(int userId, string username, int storeId, int productID, string category)
        {
            try
            {
                facade.ChangeProductCategory(userId, username, storeId, productID, category);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response<List<Store>> GetAllStores(int userId)
        {
            try
            {
                List<DomainStore> storeList = facade.GetAllStores(userId);
                List<Store> retList = new List<Store>();
                foreach (DomainStore ds in storeList)
                {
                    retList.Add(new Store(ds));
                }
                return new Response<List<Store>>(retList, userId);
            }
            catch (Exception e)
            {
                return new Response<List<Store>>(e.Message, userId);
            }
        }

        public Response<List<Notification>> TakeNotifications(int userId, string membername)
        {
            try
            {
                return new Response<List<Notification>>(facade.TakeNotifications(userId, membername).Select(x => new Notification(x)).ToList(), userId);
            }
            catch (Exception e)
            {
                return new Response<List<Notification>>(e.Message, userId);
            }
        }

        public Response<Dictionary<Member, bool>> GetMembersOnlineStats(int userId, string actingUsername)
        {
            try
            {
                Dictionary<DomainMember, bool> members = facade.GetMembersOnlineStats(userId, actingUsername);
                Dictionary<Member, bool> returnMembers = members.Keys.ToDictionary(keySelector: g => new Member(g), elementSelector: g => members[g]);
                return new Response<Dictionary<Member, bool>>(returnMembers, userId);
            }
            catch (Exception e)
            {
                return new Response<Dictionary<Member, bool>>(e.Message, userId);
            }
        }

        public Response CancelMember(int userId, string actingUsername, string canceledUsername)
        {
            try
            {
                facade.CancelMember(userId, actingUsername, canceledUsername);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        public Response<double> GetDailyIncomeMarketManager(int userId, string username)
        {
            try
            {
                double res = facade.GetDailyIncomeMarketManager(userId, username);
                return new Response<double>(res,userId);
            }
            catch (Exception e)
            {
                return new Response<double>(e.Message, userId);
            }
        }

        public Response<double> GetDailyIncomeStore(int userId, string username, int storeId)
        {
            try
            {
                double res = facade.GetDailyIncomeStoreOwner(userId, username, storeId);
                return new Response<double>(res, userId);
            }
            catch (Exception e)
            {
                return new Response<double>(e.Message, userId);
            }
        }

        public Response<List<PermissionInformation>> GetMemberPermissions(int userId, string membername)
        {
            try
            {
                return new Response<List<PermissionInformation>>(facade.GetMemberPermissions(userId, membername), userId);
            }
            catch (Exception e)
            {
                return new Response<List<PermissionInformation>>(e.Message, userId);
            }
        }

        public Response<List<StatisticsInformation>> MarketManagerDailyRangeInformation(int userId, string membername, DateTime beginning, DateTime end)
        {
            try
            {
                return new Response<List<StatisticsInformation>>(facade.MarketManagerDailyRangeInformation(userId, membername, beginning, end).Select(ucd => new StatisticsInformation(ucd)).ToList(), userId);
            }
            catch (Exception e)
            {
                return new Response<List<StatisticsInformation>>(e.Message, userId);
            }
        }

        public Response<List<Order>> GetStorePurchaseHistory(int userId, string membername, int storeId)
        {
            try
            {
                return new Response<List<Order>>(facade.GetStorePurchaseHistory(userId, membername, storeId).Select(dor => new Order(dor)).ToList(), userId);
            }
            catch (Exception ex)
            {
                return new Response<List<Order>>(ex.Message, userId);
            }
        }

        public Response<Bid> OfferBid(int userId, string username, int storeId, int productId, double price)
        {
            try
            {
                return new Response<Bid>(new Bid(facade.OfferBid(userId, username, storeId, productId, price)), userId);
            }
            catch (Exception ex)
            {
                return new Response<Bid>(ex.Message, userId);
            }
        }

        public Response<Bid> CounterBid(int userId, string membername, int storeId, int bidId, double newPrice)
        {
            try
            {
                return new Response<Bid>(new Bid(facade.CounterBid(userId, membername, storeId, bidId, newPrice)), userId);
            }
            catch (Exception ex)
            {
                return new Response<Bid>(ex.Message, userId);
            }
        }

        public Response<Bid> VoteForBid(int userId, string username, int storeId, int bidId, bool vote)
        {
            try
            {
                return new Response<Bid>(new Bid(facade.VoteForBid(userId, username, storeId, bidId, vote)), userId);
            }
            catch (Exception ex)
            {
                return new Response<Bid>(ex.Message, userId);
            }        
        }

        public Response<double> BuyBidProduct(int userId, string username, int storeId, int bidId, CreditCard cc, SupplyAddress address, DateTime buyTime)
        {
            try
            {
                return new Response<double>(facade.BuyBidProduct(userId, username, storeId, bidId, cc, address, buyTime), userId);
            }
            catch (Exception ex)
            {
                return new Response<double>(ex.Message, userId);
            }
        }

        public Response<List<Bid>> GetBidsStatus(int userId, string username, int storeId)
        {
            try
            {
                return new Response<List<Bid>>(facade.GetBidsStatus(userId, username, storeId).Select(db => new Bid(db)).ToList(), userId);
            }
            catch (Exception ex)
            {
                return new Response<List<Bid>>(ex.Message, userId);
            }
        }
    }
}
