using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.ServiceLayer;
using SystemAdminDTO = Workshop.ServiceLayer.ServiceObjects.SystemAdminDTO;

namespace Tests.IntegrationTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestMarketController
    {
        private UserController userController;
        private MarketController marketController;
        private SupplyAddress address = new SupplyAddress("Ronmi", "Mayor 1", "Ashkelon", "Israel", "784112");
        private CreditCard cc = new CreditCard("001122334455667788", "11", "26", "LeBron Michal", "555", "208143751");
        [TestInitialize]
        public void Setup()
        {
            ISecurityHandler security = new HashSecurityHandler();
            IReviewHandler review = new ReviewHandler();
            userController = new UserController(security, review, new List<SystemAdminDTO>());
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            marketController = new MarketController(userController, externalSystem.Object);
            marketController.InitializeSystem();

            userController.EnterMarket(1);

            userController.Register(1, "member1", "pass1", DateTime.Parse("Aug 22, 1972"));
            userController.Register(1, "member2", "pass2", DateTime.Parse("Aug 22, 1972"));
            userController.Register(1, "Notallowed cohen", "pass", DateTime.Parse("Aug 22, 1972"));
            userController.Login(1, "member1", "pass1");
        }

        /// Tests for MarketController.CloseStore method
        /// <see cref="MarketController.CloseStore"/>
        [TestMethod]
        public void TestCloseStore_Success()
        {
            string member = "member1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1");
            int storeId = store1.GetId();
            marketController.CloseStore(1, member, storeId);
            Assert.IsFalse(marketController.IsStoreOpen(1, member, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure_StoreAlreadyClosed()
        {
            string member = "member1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1");
            int storeId = store1.GetId();
            marketController.CloseStore(1, member, storeId);
            Assert.ThrowsException<ArgumentException>(() => marketController.CloseStore(1, member, storeId));
        }

        /// Tests for MarketController.GetWorkersInformation method
        /// <see cref="MarketController.GetWorkersInformation"/>
        [TestMethod]
        public void TestGetWorkersInformation_Success()
        {
            string member = "member1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1");
            int storeId = store1.GetId();
            CollectionAssert.AreEqual(userController.GetWorkers(storeId), marketController.GetWorkersInformation(1, member, storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission()
        {
            string member = "member1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1");
            int storeId = store1.GetId();
            userController.EnterMarket(2);
            userController.Login(2, "Notallowed cohen", "pass");
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation(2, "Notallowed cohen", storeId));
        }

        /// Tests for MarketController.CreateNewStore method
        /// <see cref="MarketController.CreateNewStore"/>
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
            userController.Register(1, username, "pass", DateTime.Parse("Aug 22, 1972"));
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

        /// Tests for MarketController.BuyCart method
        /// <see cref="MarketController.BuyCart"/>
        [DataTestMethod]
        [DataRow("member1", 3, "cat1")]
        [DataRow("member1", 4, "cat1")]
        public void TestBuyCart_Success(string user, int userQuantity, string category)
        {
            int storeId = marketController.CreateNewStore(1, user, "store").GetId();
            Product prod1 = marketController.AddProductToStore(1, user, storeId, "someName", "someDesc", 10.0, 5, category);
            ShoppingBagProduct product2 = userController.addToCart(1, new ShoppingBagProduct(prod1.Id, prod1.Name, prod1.Description, prod1.Price, userQuantity, prod1.Category, storeId), storeId);
            int leftovers = marketController.getStoreInfo(1, user, storeId).products[prod1.Id].Quantity;
            marketController.BuyCart(1, cc, address);
            Assert.AreEqual(userController.viewCart(1).shoppingBags.Count, 0);
            Assert.IsTrue(marketController.getStoreInfo(1, user, storeId).products[prod1.Id].Quantity == leftovers);
        }

        /// Tests for MarketController.NominateStoreOwner method
        /// <see cref="MarketController.NominateStoreOwner"/>
        [TestMethod]
        public void TestRemoveStoreOwnerNomination_Success()
        {
            string member = "member1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1");
            int storeId = store1.GetId();
            userController.Register(1, "coolStoreOwner", "pass", DateTime.Parse("Aug 22, 1972"));
            marketController.NominateStoreOwner(1, "member1", "coolStoreOwner", storeId);
            List<StoreRole> original_roles = new List<StoreRole>(userController.GetMember("coolStoreOwner").GetStoreRoles(storeId));
            Member res = marketController.RemoveStoreOwnerNomination(1, "member1", "coolStoreOwner", storeId);
            Assert.IsTrue(res.Username == "coolStoreOwner");
            Assert.IsTrue(res.GetStoreRoles(storeId).Count != original_roles.Count);
        }

        [TestMethod]
        public void TestRemoveStoreOwnerNomination_Failure_NoNomination()
        {
            Assert.ThrowsException<ArgumentException>(() => marketController.RemoveStoreOwnerNomination(1, "member1", "member2", 1));
        }
    }
}