using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer;
using Workshop.ServiceLayer.ServiceObjects;
using DomainUser = Workshop.DomainLayer.UserPackage.User;
using DomainMember = Workshop.DomainLayer.UserPackage.Permissions.Member;
using DomainProduct = Workshop.DomainLayer.MarketPackage.Product;
using DomainProductDTO = Workshop.DomainLayer.MarketPackage.ProductDTO;
using DomainStoreManager = Workshop.DomainLayer.UserPackage.Permissions.StoreManager;
using DomainStoreOwner = Workshop.DomainLayer.UserPackage.Permissions.StoreOwner;
using DomainStoreFounder = Workshop.DomainLayer.UserPackage.Permissions.StoreFounder;
using DomainStore = Workshop.DomainLayer.MarketPackage.Store;
using DomainNotification = Workshop.DomainLayer.UserPackage.Notifications.Notification;

using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.Loggers;
using System.Globalization;

namespace Workshop.ServiceLayer
{
    public class Service : IService
    {
        private Facade facade;
        public readonly bool WasInitializedWithFile;

        public Service()
        {
            this.facade = new Facade();
            WasInitializedWithFile = false;
        }

        public Service(string initializationState)
        {
            this.facade = new Facade();
            try
            {
                foreach (string command in initializationState.Split('\n'))
                {
                    string[] splits = command.Split('(', ')');
                    if (splits.Length != 3 || splits[2].Trim().Length > 0)
                    {
                        throw new ArgumentException($"Command was in incorrect form: {command}");
                    }
                    string tempParams = splits[1];
                    string[] actualParams = tempParams.Split(',');
                    try
                    {
                        switch (splits[0])
                        {
                            case "enter-market":
                                if (actualParams.Length != 1) { throw new ArgumentException(); }
                                facade.EnterMarket(int.Parse(actualParams[0]));
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
                                if (actualParams.Length != 3) { throw new ArgumentException(); }
                                facade.Login(int.Parse(actualParams[0]), actualParams[1], actualParams[2]);
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
                                if (actualParams.Length != 4) { throw new ArgumentException(); }
                                facade.NominateStoreManager(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]));
                                break;
                            case "nominate-store-owner":
                                if (actualParams.Length != 4) { throw new ArgumentException(); }
                                facade.NominateStoreOwner(int.Parse(actualParams[0]), actualParams[1], actualParams[2], int.Parse(actualParams[3]));
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
                                if (actualParams.Length != 3) { throw new ArgumentException(); }
                                facade.CreateNewStore(int.Parse(actualParams[0]), actualParams[1], actualParams[2]);
                                break;
                            case "review-product":
                                if (actualParams.Length != 5) { throw new ArgumentException(); }
                                facade.ReviewProduct(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), actualParams[3], int.Parse(actualParams[4]));
                                break;
                            case "search-product":
                                if (actualParams.Length != 7) { throw new ArgumentException(); }
                                facade.SearchProduct(int.Parse(actualParams[0]), actualParams[1], actualParams[2], actualParams[3], double.Parse(actualParams[4]), double.Parse(actualParams[5]), double.Parse(actualParams[6]));
                                break;
                            case "get-all-stores":
                                if (actualParams.Length != 1) { throw new ArgumentException(); }
                                facade.GetAllStores(int.Parse(actualParams[0]));
                                break;
                            case "add-to-cart":
                                if (actualParams.Length != 5) { throw new ArgumentException(); }
                                facade.AddToCart(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]), int.Parse(actualParams[4]));
                                break;
                            case "view-cart":
                                if (actualParams.Length != 2) { throw new ArgumentException(); }
                                facade.ViewCart(int.Parse(actualParams[0]), actualParams[1]);
                                break;
                            case "edit-cart":
                                if (actualParams.Length != 4) { throw new ArgumentException(); }
                                facade.EditCart(int.Parse(actualParams[0]), actualParams[1], int.Parse(actualParams[2]), int.Parse(actualParams[3]));
                                break;
                            case "buy-cart":
                                if (actualParams.Length != 3) { throw new ArgumentException(); }
                                facade.BuyCart(int.Parse(actualParams[0]), actualParams[1], actualParams[2]);
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
                            case "change-product-price":
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
                            default:
                                throw new ArgumentException();
                        }
                    }
                    catch
                    {
                        throw new ArgumentException($"Command was not in the correct format: {command}");
                    }
                }
                WasInitializedWithFile = true;
            }
            catch (Exception ex)
            {
                this.facade = new Facade();
                Logger.Instance.LogError($"Initialization from file failed: \n{ex.Message}.\nSystem is in default condition.");
                WasInitializedWithFile = false;
            }
        }

        public Response<User> EnterMarket(int userId)
        {
            try
            {
                DomainUser domainUser = facade.EnterMarket(userId);
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

        public Response<KeyValuePair<Member, List<Notification>>> Login(int userId, string username, string password)
        {
            try
            {
                KeyValuePair<DomainMember, List<DomainNotification>> domainAnswer = facade.Login(userId, username, password);
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

        public Response<StoreOwner> NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            try
            {
                DomainStoreOwner domainOwner = facade.NominateStoreOwner(userId, nominatorUsername, nominatedUsername, storeId);
                StoreOwner serviceOwner = new StoreOwner(domainOwner);
                return new Response<StoreOwner>(serviceOwner, userId);
            }
            catch (Exception e)
            {
                return new Response<StoreOwner>(e.Message, userId);
            }
        }

        public Response<StoreManager> NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            try
            {
                DomainStoreManager domainManager = facade.NominateStoreManager(userId, nominatorUsername, nominatedUsername, storeId);
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

        public Response<Store> CreateNewStore(int userId, string creator, string storeName)
        {
            try
            {
                DomainStore domainStore = facade.CreateNewStore(userId, creator, storeName);
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

        public Response<List<Product>> SearchProduct(int userId, string user, string keyWords, string category, double minPrice, double maxPrice, double productReview)
        {
            try
            {
                List<DomainProductDTO> products = facade.SearchProduct(userId, user, keyWords, category, minPrice, maxPrice, productReview);
                List<Product> returnProducts = products.Select(x => new Product(x)).ToList();
                return new Response<List<Product>>(returnProducts, userId);
            }
            catch (Exception e)
            {
                return new Response<List<Product>>(e.Message, userId);
            }
        }

        public Response<Product> addToCart(int userId, string user, int productId, int storeId, int quantity)
        {
            try
            {
                Product product = new Product(facade.AddToCart(userId, user, productId, storeId, quantity));
                return new Response<Product>(product, userId);
            }
            catch (Exception e)
            {
                return new Response<Product>(e.Message, userId);
            }
        }

        public Response<ShoppingCart> viewCart(int userId, string user)
        {
            try
            {
                ShoppingCart shoppingCart = new ShoppingCart(facade.ViewCart(userId, user));
                return new Response<ShoppingCart>(shoppingCart, userId);
            }
            catch (Exception e)
            {
                return new Response<ShoppingCart>(e.Message, userId);
            }
        }

        public Response<ShoppingCart> editCart(int userId, string user, int productId, int newQuantity)
        {
            try
            {
                ShoppingCart shoppingCart = new ShoppingCart(facade.EditCart(userId, user, productId, newQuantity));
                return new Response<ShoppingCart>(shoppingCart, userId);
            }
            catch (Exception e)
            {
                return new Response<ShoppingCart>(e.Message, userId);
            }
        }

        public Response<double> BuyCart(int userId, string user, string address)
        {
            try
            {
                return new Response<double>(facade.BuyCart(userId, user, address), userId);
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
    }
}
