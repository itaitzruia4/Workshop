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
using Workshop.DomainLayer.Reviews;

namespace Workshop.ServiceLayer
{
    public class Service : IService
    {
        private Facade facade;

        public Service()
        {
            this.facade = new Facade();
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

        public Response<Member> Login(int userId, string username, string password)
        {
            try
            {
                DomainMember domainMember = facade.Login(userId, username, password);
                Member serviceMember = new Member(domainMember);
                return new Response<Member>(serviceMember, userId);
            }
            catch (Exception e)
            {
                return new Response<Member>(e.Message, userId);
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
                Product product = new Product(facade.addToCart(userId, user, productId, storeId, quantity));
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
                ShoppingCart shoppingCart = new ShoppingCart(facade.viewCart(userId, user));
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
                ShoppingCart shoppingCart = new ShoppingCart(facade.editCart(userId, user, productId, newQuantity));
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

        Response AddProductPurchaseTerm(int userId, string user, int storeId, string jsonTerm, int product_id)
        {
            try
            {
                facade.AddProducPurchaseTerm(userId, user, storeId, jsonTerm, product_id);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        Response AddCategoryPurchaseTerm(int userId, string user, int storeId, string jsonTerm, string category_name)
        {
            try
            {
                facade.AddCategoryPurchaseTerm(userId, user, storeId, jsonTerm, category_name);
                return new Response(userId);
            }
            catch (Exception e)
            {
                return new Response(e.Message, userId);
            }
        }

        Response AddStorePurchaseTerm(int userId, string user, int storeId, string jsonTerm)
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

        Response AddUserPurchaseTerm(int userId, string user, int storeId, string jsonTerm)
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

        public Response ChangeProductPrice(int userId, string username, int storeId, int productID, int price)
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
                List<DomainStore> storeList = facade.GetAllStores();
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
