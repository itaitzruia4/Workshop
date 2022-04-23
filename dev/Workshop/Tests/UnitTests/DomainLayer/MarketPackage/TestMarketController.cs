using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Tests
{
    [TestClass]
    public class TestMarketController
    {
        private MarketController marketController;

        [TestInitialize]
        public void Setup()
        {

            var userControllerMock = new Mock<IUserController>();
            userControllerMock.Setup(x => x.AssertCurrentUser(It.IsAny<string>())).Callback((string user) => {});
            userControllerMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action>())).Returns(true);

            marketController = new MarketController(userControllerMock.Object);
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
    }
}