using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using Workshop.DomainLayer.MarketPackage;

namespace Tests
{
    [TestClass]
    public class TestMarketController
    {
        private ISecurityHandler security;
        private IUserController userController;
        private MarketController marketController;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            security = securityMock.Object;

            var userControllerMock = new Mock<IUserController>();
            userControllerMock.Setup(x => x.AssertCurrentUser(It.IsAny<string>()));
            userControllerMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Workshop.DomainLayer.UserPackage.Permissions.Action>())).Returns(true);

            marketController = new MarketController(userControllerMock.Object);
            marketController.InitializeMarketController();
        }

        [TestMethod]
        public void Temp_Success()
        {
            bool IHateMyLife = true;
        }

        [TestMethod]
        public void TestCloseStore_Success()
        {
            // Arrange
            string username = "user1";
            int storeId = 1;

            // Act
            marketController.CloseStore(username, storeId);

            // Assert
            Assert.IsFalse(marketController.IsStoreOpen(username,storeId));
        }
    }
}
