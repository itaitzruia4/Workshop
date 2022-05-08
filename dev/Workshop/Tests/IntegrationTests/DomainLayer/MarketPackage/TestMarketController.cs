using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Tests.IntegrationTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestMarketController
    {
        private UserController userController;
        private MarketController marketController;

        [TestInitialize]
        public void Setup()
        {
            ISecurityHandler security = new HashSecurityHandler();
            IReviewHandler review = new ReviewHandler();
            IPaymentExternalService paymentExternalService = new ProxyPaymentExternalService(null);
            IMarketPaymentService paymentService = new PaymentAdapter(paymentExternalService);

            ISupplyExternalService supplyExternalService = new ProxySupplyExternalService(null);
            IMarketSupplyService supplyService = new SupplyAdapter(supplyExternalService);

            userController = new UserController(security, review);
            marketController = new MarketController(userController, paymentService, supplyService);
            userController.InitializeSystem();
            marketController.InitializeSystem();

            userController.EnterMarket();

            userController.Register("member1", "pass1");
            userController.Register("member2", "pass2");
            userController.Register("Notallowed cohen", "pass");
            userController.Login("member1", "pass1");
            Store store1 = marketController.CreateNewStore("member1", "shop1");
            userController.NominateStoreManager("member1", "member2", store1.GetId());
        }

        [TestMethod]
        public void TestCloseStore_Success()
        {
            // Arrange
            string username = "member1"; int storeId = 1;

            // Act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.IsFalse(marketController.IsStoreOpen(username, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure()
        {
            // Arrange
            string username = "member1"; int storeId = 1;

            //act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.ThrowsException<ArgumentException>(() => marketController.CloseStore(username, storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Success()
        {
            CollectionAssert.AreEqual(userController.GetWorkers(1), marketController.GetWorkersInformation("member1", 1));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission()
        {
            userController.Login("Notallowed cohen", "pass");
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation("Notallowed cohen", 1));
        }

        [TestMethod]
        public void TestCreateNewStore_Success()
        {
            string storeName = "Cool store 123";
            Store result = marketController.CreateNewStore("member1", storeName);
            Assert.AreEqual(result.GetStoreName(), storeName);
            Assert.AreEqual(result.GetProducts().Count, 0);
            Assert.AreEqual(result.IsOpen(), true);
        }

        [TestMethod]
        public void TestCreateNewStore_Failure_NotLoggedIn()
        {
            string username = "CompletelyRandomNameNoChanceAnyoneWouldEverWriteIt";
            userController.Logout("member1");
            userController.Register(username, "pass");
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(username, "Store123"));
        }


        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow(null, "")]
        [DataRow("", null)]
        public void TestCreateNewStore_Failure_EmptyOrNullInput(string username, string storeName)
        {
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(username, storeName));
        }

        //checks cart is empty and products were taken from stores
        [DataTestMethod]
        [DataRow("member1", "here", 1, 3)]
        [DataRow("member1", "here", 1, 4)]
        public void BuyCart(string user, string address, int productId, int userQuantity)
        {
            int storeId = marketController.CreateNewStore(user, "store").GetId();
            marketController.AddProductToStore(user, storeId, productId, "someName", "someDesc", 10.0, 5);
            ShoppingBagProduct product2 = userController.addToCart(user, new ShoppingBagProduct(productId, "someName", "someDesc", 10.0, userQuantity), storeId);
            int leftovers = marketController.getStoreInfo(user, storeId).products[productId].Quantity - userQuantity;
            marketController.BuyCart(user, address);
            Assert.IsTrue(userController.viewCart(user).shoppingBags.Count==0);
            Assert.IsTrue(marketController.getStoreInfo(user,storeId).products[productId].Quantity == leftovers);
        }

    }
}
