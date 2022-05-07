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
            userControllerMock.Setup(x => x.AssertCurrentUser(It.IsAny<string>())).Callback((string user) => {});
            userControllerMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action>())).Returns((string user, int storeId, Action action) => !user.Equals("Notallowed Cohen"));
            userControllerMock.Setup(x => x.GetWorkers(It.IsAny<int>())).Returns(new List<Member>(new Member[] {new Member("Worker1", "pass1")}));
            userControllerMock.Setup(x => x.GetMember(It.IsAny<string>())).Returns(new Member("StoreFounder1", "pass1"));
            userControllerMock.Setup(x => x.addToCart(It.IsAny<string>(), It.IsAny<ShoppingBagProduct>(), It.IsAny<int>())).Returns(new ShoppingBagProduct(1, "someName", "someDesc", 10.0, 3));
           
            Mock<IMarketPaymentService> paymentMock = new Mock<IMarketPaymentService>();
            paymentMock.Setup(x => x.PayAmount(It.IsAny<string>(), It.IsAny<double>())).Callback((string username, double amount) => {});

            Mock<IMarketSupplyService> supplyMock = new Mock<IMarketSupplyService>();
            supplyMock.Setup(x => x.supplyToAddress(It.IsAny<string>(), It.IsAny<string>())).Callback((string username, string address) => { });

            marketController = new MarketController(userControllerMock.Object, paymentMock.Object, supplyMock.Object);
            marketController.InitializeSystem();
            Store store1 = marketController.CreateNewStore("StoreFounder1", "shop1");

            // marketController = new MarketController();
            // userController = new UserController(security);
        }

        [TestMethod]
        public void TestCloseStore_Success()
        {
            // Arrange
            string username = "StoreFounder1"; int storeId = 1;

            // Act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.IsFalse(marketController.IsStoreOpen(username, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure()
        {
            // Arrange
            string username = "StoreFounder1"; int storeId = 1;

            //act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.ThrowsException<ArgumentException>(() => marketController.CloseStore(username, storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Success(){
            CollectionAssert.AreEqual(userControllerMock.Object.GetWorkers(1), marketController.GetWorkersInformation("User1", 1));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission(){
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation("Notallowed Cohen", 1));
        }

        [TestMethod]
        public void TestCreateNewStore_Success(){
            string username = "User1";
            string storeName = "Cool store 123";
            Store result = marketController.CreateNewStore(username, storeName);
            Assert.AreEqual(result.GetStoreName(), storeName);
            Assert.AreEqual(result.GetProducts().Count, 0);
            Assert.AreEqual(result.IsOpen(), true);

        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("User1", "")]
        public void TestCreateNewStore_Failure_EmptyOrNullInput(string username, string storeName){
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(username, storeName));
        }

        [DataTestMethod]
        [DataRow("User1", 1, "key", "catagory", 2, 11, 3)]
        [DataRow("User1", 1, "key", "catagory", 0, 24, 3)]
        public void TestSearchProduct_Success(string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview)
        {
            marketController.CreateNewStore(user, "store");
            marketController.AddProductToStore(user, 1, productId, "someName", "someDesc", 10.0, 2);
            Product product = new Product(productId, "someName", "someDesc", 10.0, 2);
            List<Product> products = new List<Product>();
            products.Add(product);
            ProductDTO product2 = marketController.SearchProduct(user, productId, keyWords, catagory, minPrice, maxPrice, productReview)[0];
            Assert.IsTrue(product.EqualsFields(product2));
        }

        [DataTestMethod]
        [DataRow("User1", 1, "key", "catagory", 2, 9, 3)] //wrong price
        [DataRow("User1", 2, "key", "catagory", 2, 9, 3)] //wrong Id
        [DataRow("User1", -1, "key", "catagory", 0, 24, 3)] //not the right keyword
        [DataRow("User1", -1, "", "catagory", 0, 24, 3)] //not the right catagory
        [DataRow("User1", -1, "", "", 0, 24, 3)] //no products to filter from 
        public void TestSearchProduct_Failure(string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview)
        {
            marketController.CreateNewStore(user, "store");
            marketController.AddProductToStore(user, 1, 1, "someName", "someDesc", 10.0, 2);
            Product product = new Product(productId, "someName", "someDesc", 10.0, 2);
            List<ProductDTO> empty = new List<ProductDTO>();
            CollectionAssert.AreEqual(empty,marketController.SearchProduct(user, productId, keyWords, catagory, minPrice, maxPrice, productReview));
        }


        [DataTestMethod]
        [DataRow("User1", 1, 1, 3)]
        public void TestAddToCart_Success(string user, int productId, int storeId, int quantity)
        {
            marketController.CreateNewStore(user, "store");
            //Product product = new Product(productId, "someName", "someDesc", 10.0, quantity);
            Product product = marketController.AddProductToStore(user, 1, 1, "someName", "someDesc", 10.0, 3);
            Assert.IsTrue(product.EqualsFields(marketController.addToBag(user, productId, storeId, quantity).GetProductDTO()));
        }

        [DataTestMethod]
        [DataRow("User1", 2, 1, 3)] //wrong id;
        [DataRow("User1", 1, 0, 3)] //wrong store;
        [DataRow("User1", 1, 1, 4)] //wrong quantity;
        public void TestAddToCart_Failure(string user, int productId, int storeId, int quantity)
        {
            marketController.CreateNewStore(user, "store");
            marketController.AddProductToStore(user, 1, 1, "someName", "someDesc", 10.0, 2);
            Assert.ThrowsException<ArgumentException>(() => marketController.addToBag(user, productId, storeId, quantity));
            Product product = new Product(productId, "someName", "someDesc", 10.0, 2);
        }
    }
}