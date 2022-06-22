using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using System.Collections.Generic;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.ServiceLayer;

namespace Tests.UnitTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestMarketController
    {
        private MarketController marketController;
        private Mock<IUserController> userControllerMock;
        
        [TestInitialize]
        public void Setup()
        {
            userControllerMock = new Mock<IUserController>();
            userControllerMock.Setup(x => x.AssertCurrentUser(It.IsAny<int>(), It.IsAny<string>())).Callback((int x, string user) => {});
            userControllerMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action>())).Returns((string user, int storeId, Action action) => !user.Equals("Notallowed Cohen"));
            userControllerMock.Setup(x => x.GetWorkers(It.IsAny<int>())).Returns(new List<Member>(new Member[] {new Member("Worker1", "pass1", DateTime.Parse("Aug 22, 1972")) }));
            userControllerMock.Setup(x => x.GetMember(It.IsAny<string>())).Returns(new Member("StoreFounder1", "pass1", DateTime.Parse("Aug 22, 1972")));
            userControllerMock.Setup(x => x.AddToCart(It.IsAny<int>(), It.IsAny<ShoppingBagProduct>(), It.IsAny<int>())).Returns((int id, ShoppingBagProduct sbp, int storeId) => { return new ShoppingBagProduct(sbp.Id, sbp.Name, sbp.Description, sbp.Price, sbp.Quantity, sbp.Category, sbp.StoreId); });
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            marketController = new MarketController(userControllerMock.Object, externalSystem.Object);
            marketController.InitializeSystem();
        }

        /// Tests for MarketController.CloseStore method
        /// <see cref="MarketController.CloseStore"/>
        [TestMethod]
        public void TestCloseStore_Success()
        {
            string member = "StoreFounder1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1", DateTime.Now);
            int storeId = store1.GetId();
            marketController.CloseStore(1, member, storeId);
            Assert.IsFalse(marketController.IsStoreOpen(1, member, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure_NoSuchStore()
        {
            string member = "StoreFounder1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1", DateTime.Now);
            int storeId = store1.GetId();
            marketController.CloseStore(1, member, storeId);
            Assert.ThrowsException<ArgumentException>(() => marketController.CloseStore(1, member, storeId));
        }

        /// Tests for MarketController.GetWorkersInformation method
        /// <see cref="MarketController.GetWorkersInformation"/>
        [TestMethod]
        public void TestGetWorkersInformation_Success(){
            string member = "StoreFounder1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1", DateTime.Now);
            int storeId = store1.GetId();
            CollectionAssert.AreEqual(userControllerMock.Object.GetWorkers(storeId), marketController.GetWorkersInformation(1, "User1", storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission(){
            string member = "StoreFounder1";
            Store store1 = marketController.CreateNewStore(1, member, "shop1", DateTime.Now);
            int storeId = store1.GetId();
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation(1, "Notallowed Cohen", storeId));
        }

        /// Tests for MarketController.CreateNewStore method
        /// <see cref="MarketController.CreateNewStore"/>
        [TestMethod]
        public void TestCreateNewStore_Success(){
            string username = "User1";
            string storeName = "Cool store 123";
            Store result = marketController.CreateNewStore(1, username, storeName, DateTime.Now);
            Assert.AreEqual(result.GetStoreName(), storeName);
            Assert.AreEqual(result.GetProducts().Count, 0);
            Assert.AreEqual(result.IsOpen(), true);

        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("User1", "")]
        public void TestCreateNewStore_Failure_EmptyOrNullInput(string username, string storeName){
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(1, username, storeName, DateTime.Now));
        }

        /// Tests for MarketController.SearchProduct method
        /// <see cref="MarketController.SearchProduct"/>
        [TestMethod]
        public void TestSearchProduct_Success()
        {
            Store st = marketController.CreateNewStore(1, "User1", "store", DateTime.Now);
            int storeId = st.GetId();
            Product p1 = marketController.AddProductToStore(1, "User1", storeId, "prod1", "desc1", 10.0, 2, "cat1");
            Product p2 = marketController.AddProductToStore(1, "User1", storeId, "prod2", "desc2", 9.6, 2, "cat2");
            List<ProductDTO> searchedProducts = marketController.SearchProduct(1, "", "", 8.7, 10.0, -1);
            Assert.IsTrue(searchedProducts.Count == 2);
            Assert.IsTrue(p1.GetProductDTO().Equals(searchedProducts[0]) || p1.GetProductDTO().Equals(searchedProducts[1]));
            Assert.IsTrue(p2.GetProductDTO().Equals(searchedProducts[0]) || p2.GetProductDTO().Equals(searchedProducts[1]));
        }

        [DataTestMethod]
        [DataRow("member1", "", "", 2, 9, -1)] //wrong OfferedPrice
        [DataRow("member1", "keyword", "", -1, -1, -1)] //not the right keyword
        [DataRow("member1", "", "differentCateogry", -1, -1, -1)] //not the right catagory
        public void TestSearchProduct_Failure_WrongArguments(string user, string keyWords, string catagory, int minPrice, int maxPrice, int productReview)
        {
            Store st = marketController.CreateNewStore(1, user, "store", DateTime.Now);
            marketController.AddProductToStore(1, user, st.GetId(), "someName", "someDesc", 10.0, 2, "cat1");
            Assert.AreEqual(marketController.SearchProduct(1, keyWords, catagory, minPrice, maxPrice, productReview).Count, 0);
        }

        [DataTestMethod]
        [DataRow("User1", 2, 1, 3)] //wrong id;
        [DataRow("User1", 1, 0, 3)] //wrong store;
        [DataRow("User1", 1, 1, 4)] //wrong quantity;
        public void TestAddToBag_Failure_WrongArguments(string user, int productId, int storeId, int quantity)
        {
            Store st = marketController.CreateNewStore(1, user, "store", DateTime.Now);
            marketController.AddProductToStore(1, user, st.GetId(), "someName", "someDesc", 10.0, 2, "cat1");
            Assert.ThrowsException<ArgumentException>(() => marketController.AddToCart(1, productId, storeId, quantity));
            Product product = new Product(productId, "someName", "someDesc", 10.0, 2, "cat1", 1);
        }
    }
}