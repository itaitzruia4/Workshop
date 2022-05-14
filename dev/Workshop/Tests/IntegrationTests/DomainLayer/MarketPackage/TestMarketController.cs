using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
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

            userController.EnterMarket(1);

            userController.Register(1, "member1", "pass1", 40);
            userController.Register(1, "member2", "pass2", 40);
            userController.Register(1, "Notallowed cohen", "pass", 40);
            userController.Login(1, "member1", "pass1");
            Store store1 = marketController.CreateNewStore(1, "member1", "shop1");
            userController.NominateStoreManager(1, "member1", "member2", store1.GetId());
        }

        [TestMethod]
        public void TestCloseStore_Success()
        {
            // Arrange
            string username = "member1"; int storeId = 1;

            // Act
            marketController.CloseStore(1, username, storeId);

            // Assert
            Assert.IsFalse(marketController.IsStoreOpen(1, username, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure()
        {
            // Arrange
            string username = "member1"; int storeId = 1;

            //act
            marketController.CloseStore(1, username, storeId);

            // Assert
            Assert.ThrowsException<ArgumentException>(() => marketController.CloseStore(1, username, storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Success()
        {
            CollectionAssert.AreEqual(userController.GetWorkers(1), marketController.GetWorkersInformation(1, "member1", 1));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission()
        {
            userController.EnterMarket(2);
            userController.Login(2, "Notallowed cohen", "pass");
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation(2, "Notallowed cohen", 1));
        }

        [TestMethod]
        public void TestCreateNewStore_Success()
        {
            string storeName = "Cool store 123";
            Store result = marketController.CreateNewStore(1, "member1", storeName);
            Assert.AreEqual(result.GetStoreName(), storeName);
            Assert.AreEqual(result.GetProducts().Count, 0);
            Assert.AreEqual(result.IsOpen(), true);
        }

        [TestMethod]
        public void TestCreateNewStore_Failure_NotLoggedIn()
        {
            string username = "CompletelyRandomNameNoChanceAnyoneWouldEverWriteIt";
            userController.Logout(1, "member1");
            userController.Register(1, username, "pass", 40);
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(1, username, "Store123"));
        }


        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow(null, "")]
        [DataRow("", null)]
        public void TestCreateNewStore_Failure_EmptyOrNullInput(string username, string storeName)
        {
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(1, username, storeName));
        }

        //checks cart is empty and products were taken from stores
        [DataTestMethod]
        [DataRow("member1", "here", 1, 3, "cat1")]
        [DataRow("member1", "here", 1, 4, "cat1")]
        public void BuyCart(string user, string address, int productId, int userQuantity, string category)
        {
            int storeId = marketController.CreateNewStore(1, user, "store").GetId();
            marketController.AddProductToStore(1, user, storeId, productId, "someName", "someDesc", 10.0, 5, "cat1");
            ShoppingBagProduct product2 = userController.addToCart(1, user, new ShoppingBagProduct(productId, "someName", "someDesc", 10.0, userQuantity, category), storeId);
            int leftovers = marketController.getStoreInfo(1, user, storeId).products[productId].Quantity - userQuantity;
            marketController.BuyCart(1, user, address);
            Assert.IsTrue(userController.viewCart(1, user).shoppingBags.Count == 0);
            Assert.IsTrue(marketController.getStoreInfo(1, user, storeId).products[productId].Quantity == leftovers);
        }
        [TestMethod]
        public void TestRemoveStoreOwnerNomination_Success()
        {
            userController.Register(1, "coolStoreOwner", "pass", 40);
            marketController.NominateStoreOwner(1, "member1", "coolStoreOwner", 1);
            List<StoreRole> original_roles = new List<StoreRole>(userController.GetMember("coolStoreOwner").GetStoreRoles(1));
            Member res = marketController.RemoveStoreOwnerNomination(1, "member1", "coolStoreOwner", 1);
            Assert.IsTrue(res.Username == "coolStoreOwner");
            Assert.IsTrue(res.GetStoreRoles(1).Count != original_roles.Count);
        }

        [TestMethod]
        public void TestRemoveStoreOwnerNomination_Failure_NoNomination()
        {
            Assert.ThrowsException<ArgumentException>(() => marketController.RemoveStoreOwnerNomination(1, "member1", "member2", 1));
        }
    }
}