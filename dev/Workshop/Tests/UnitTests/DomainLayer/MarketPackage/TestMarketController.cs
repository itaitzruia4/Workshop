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
            userControllerMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action>())).Callback(async (string user, int storeId, Action action) => {if (action == Action.GetWorkersInformation) return !user.Equals("Notallowed Cohen"); return true;});
            userControllerMock.Setup(x => x.GetWorkers(It.IsAny<int>())).Returns(new List<Member>(new Member[] {new Member("Worker1", "pass1")}));
            userControllerMock.Setup(x => x.GetMember(It.IsAny<string>())).Returns(new Member("StoreFounder1", "pass1"));

            Mock<IMarketPaymentService> paymentMock = new Mock<IMarketPaymentService>();
            paymentMock.Setup(x => x.PayAmount(It.IsAny<string>(), It.IsAny<double>())).Callback((string username, double amount) => {});

            Mock<IMarketSupplyService> supplyMock = new Mock<IMarketSupplyService>();
            supplyMock.Setup(x => x.supplyToAddress(It.IsAny<string>(), It.IsAny<string>())).Callback((string username, string address) => { });

            marketController = new MarketController(userControllerMock.Object, paymentMock.Object, supplyMock.Object);
            marketController.InitializeSystem();

            // marketController = new MarketController();
            // userController = new UserController(security);
        }

        [TestMethod]
        public void TestCloseStore_Success()
        {
            // Arrange
            string username = "user1"; int storeId = 1;

            // Act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.IsFalse(marketController.IsStoreOpen(username, storeId));
        }

        [TestMethod]
        public void TestCloseStore_Failure()
        {
            // Arrange
            string username = "user1"; int storeId = 1;

            // Act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.ThrowsException<Exception>(() => marketController.CloseStore(username, storeId));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Success(){
            CollectionAssert.AreEqual(userControllerMock.Object.GetWorkers(1), marketController.GetWorkersInformation("User1", 1));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission(){
            Assert.ThrowsException<ArgumentException>(() => marketController.GetWorkersInformation("Notallowed Cohen", 1));
        }

        [TestMethod]
        public void TestCreateNewStore_Success(){
            int result = marketController.CreateNewStore("User1", "Cool store123");
            Assert.AreEqual(result, 5);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("User1", "")]
        public void TestCreateNewStore_Failure(string username, string storeName){
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(username, storeName));
        }
    }
}