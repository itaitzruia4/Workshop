using System.Collections.Generic;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Workshop.DomainLayer.UserPackage.Shopping;



namespace Workshop.DomainLayer
{
    public class Facade
    {
        private IUserController UserController;
        private IMarketController MarketController;

        internal Facade()
        {
            IPaymentExternalService paymentExternalService = new ProxyPaymentExternalService(null);
            IMarketPaymentService paymentService = new PaymentAdapter(paymentExternalService);

            ISupplyExternalService supplyExternalService = new ProxySupplyExternalService(null);
            IMarketSupplyService supplyService = new SupplyAdapter(supplyExternalService);

            UserController = new UserController(new HashSecurityHandler(), new ReviewHandler());
            MarketController = new MarketController(UserController, paymentService, supplyService);
        }

        public User EnterMarket()
        {
            return UserController.EnterMarket();
        }

        public void ExitMarket()
        {
            UserController.ExitMarket();
        }

        public Member Login(string username, string password)
        {
            return UserController.Login(username, password);
        }

        public void Logout(string username)
        {
            UserController.Logout(username);
        }

        public void Register(string username, string password)
        {
            UserController.Register(username, password);
        }

        internal Product AddProduct(string username, int storeId, int productId, string productName, string description, double price, int quantity)
        {
            return MarketController.AddProductToStore(username, storeId, productId, productName, description, price, quantity);
        }

        internal StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId)
        {
            return MarketController.NominateStoreManager(nominatorUsername, nominatedUsername, storeId);
        }

        internal StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            return MarketController.NominateStoreOwner(nominatorUsername, nominatedUsername, storeId);
        }

        internal List<Member> GetWorkersInformation(string username, int storeId)
        {
            return MarketController.GetWorkersInformation(username, storeId);
        }
        internal void CloseStore(string username, int storeId)
        {
            MarketController.CloseStore(username, storeId);
        }

        internal Store CreateNewStore(string creator, string storeName)
        {
            return MarketController.CreateNewStore(creator, storeName);
        }

        internal ReviewDTO ReviewProduct(string user, int productId, string review)
        {
            return UserController.ReviewProduct(user, productId, review);
        }
        internal ProductDTO getProductInfo(string user, int productId)
        {
            return MarketController.getProductInfo(user, productId);
        }
        internal StoreDTO getStoreInfo(string user, int storeId)
        {
            return MarketController.getStoreInfo(user, storeId);
        }
        internal List<ProductDTO> SearchProduct(string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview)
        {
            return MarketController.SearchProduct(user, productId, keyWords,catagory,minPrice,maxPrice,productReview);
        }
        internal ShoppingBagProduct addToCart(string user, int productId, int storeId,int quantity)
        {
            return MarketController.addToBag(user, productId, storeId, quantity);
        }
        internal ShoppingCartDTO viewCart(string user)
        {
            return UserController.viewCart(user);
        }
        internal ShoppingCartDTO editCart(string user, int productId, int newQuantity)
        {
            return UserController.editCart(user, productId, newQuantity);
        }
        internal void BuyCart(string user,string address)
        {
            MarketController.BuyCart(user,address);
        }
        
    }
}