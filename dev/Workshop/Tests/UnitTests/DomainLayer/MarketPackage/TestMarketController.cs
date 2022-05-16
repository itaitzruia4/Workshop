using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using System.Collections.Generic;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;

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
            userControllerMock.Setup(x => x.addToCart(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ShoppingBagProduct>(), It.IsAny<int>())).Returns((int id, string membername, ShoppingBagProduct sbp, int storeId) => { return new ShoppingBagProduct(sbp.Id, sbp.Name, sbp.Description, sbp.Price, sbp.Quantity, sbp.Category); });
           
            Mock<IMarketPaymentService> paymentMock = new Mock<IMarketPaymentService>();
            paymentMock.Setup(x => x.PayAmount(It.IsAny<string>(), It.IsAny<double>())).Callback((string username, double amount) => {});

            Mock<IMarketSupplyService> supplyMock = new Mock<IMarketSupplyService>();
            supplyMock.Setup(x => x.supplyToAddress(It.IsAny<string>(), It.IsAny<string>())).Callback((string username, string address) => { });

            marketController = new MarketController(userControllerMock.Object, paymentMock.Object, supplyMock.Object);
            marketController.InitializeSystem();
            Store store1 = marketController.CreateNewStore(1, "StoreFounder1", "shop1");

            // marketController = new MarketController();
            // userController = new UserController(security);
        }

        [TestMethod]
        public void TestCloseStore_Success()
        {
            // Arrange
            string username = "StoreFounder1"; int storeId = 1;

            // Act
            marketController.CloseStore(1, username, storeId);

            // Assert
            Assert.IsFalse(marketController.IsStoreOpen(1, username, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure_NoSuchStore()
        {
            // Arrange
            string username = "StoreFounder1"; int storeId = 1;

            //act
            marketController.CloseStore(1, username, storeId);

            // Assert
            Assert.ThrowsException<ArgumentException>(() => marketController.CloseStore(1, username, storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Success(){
            CollectionAssert.AreEqual(userControllerMock.Object.GetWorkers(1), marketController.GetWorkersInformation(1, "User1", 1));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission(){
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation(1, "Notallowed Cohen", 1));
        }

        [TestMethod]
        public void TestCreateNewStore_Success(){
            string username = "User1";
            string storeName = "Cool store 123";
            Store result = marketController.CreateNewStore(1, username, storeName);
            Assert.AreEqual(result.GetStoreName(), storeName);
            Assert.AreEqual(result.GetProducts().Count, 0);
            Assert.AreEqual(result.IsOpen(), true);

        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("User1", "")]
        public void TestCreateNewStore_Failure_EmptyOrNullInput(string username, string storeName){
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(1, username, storeName));
        }

        [TestMethod]
        public void TestSearchProduct_Success()
        {
            marketController.CreateNewStore(1, "User1", "store");
            Product p1 = marketController.AddProductToStore(1, "User1", 1, "prod1", "desc1", 10.0, 2, "cat1");
            Product p2 = marketController.AddProductToStore(1, "User1", 1, "prod2", "desc2", 9.6, 2, "cat2");
            List<ProductDTO> searchedProducts = marketController.SearchProduct(1, "User1", "", "", 8.7, 10.0, -1);
            Assert.IsTrue(searchedProducts.Count == 2);
            Assert.IsTrue(p1.GetProductDTO().Equals(searchedProducts[0]) || p1.GetProductDTO().Equals(searchedProducts[1]));
            Assert.IsTrue(p2.GetProductDTO().Equals(searchedProducts[0]) || p2.GetProductDTO().Equals(searchedProducts[1]));
        }

        [DataTestMethod]
        [DataRow("User1", "key", "catagory", 2, 9, 3)] //wrong price
        [DataRow("User1", "key", "catagory", 2, 9, 3)] //wrong Id
        [DataRow("User1", -1, "key", "catagory", 0, 24, 3)] //not the right keyword
        [DataRow("User1", -1, "", "catagory", 0, 24, 3)] //not the right catagory
        [DataRow("User1", -1, "", "", 0, 24, 3)] //no products to filter from 
        public void TestSearchProduct_Failure_WrongArguments(string user, string keyWords, string catagory, int minPrice, int maxPrice, int productReview)
        {
            Store st = marketController.CreateNewStore(1, user, "store");
            marketController.AddProductToStore(1, user, 1, "someName", "someDesc", 10.0, 2, "cat1");
            Assert.AreEqual(marketController.SearchProduct(1, user, keyWords, catagory, minPrice, maxPrice, productReview).Count, 0);
        }


        [DataTestMethod]
        [DataRow("member1", 3)]
        public void TestAddToCart_Success(string user, int quantity)
        {
            Store st = marketController.CreateNewStore(1, user, "store");
            Product product = marketController.AddProductToStore(1, user, st.GetId(), "someName", "someDesc", 10.0, quantity, "cat1");
            Assert.IsTrue(product.GetProductDTO().Equals(marketController.addToBag(1, user, product.Id, st.GetId(), quantity).GetProductDTO()));
        }

        [DataTestMethod]
        [DataRow("User1", 2, 1, 3)] //wrong id;
        [DataRow("User1", 1, 0, 3)] //wrong store;
        [DataRow("User1", 1, 1, 4)] //wrong quantity;
        public void TestAddToCart_Failure_WrongArguments(string user, int productId, int storeId, int quantity)
        {
            Store st = marketController.CreateNewStore(1, user, "store");
            marketController.AddProductToStore(1, user, st.GetId(), "someName", "someDesc", 10.0, 2, "cat1");
            Assert.ThrowsException<ArgumentException>(() => marketController.addToBag(1, user, productId, storeId, quantity));
            Product product = new Product(productId, "someName", "someDesc", 10.0, 2, "cat1");
        }
    }
}