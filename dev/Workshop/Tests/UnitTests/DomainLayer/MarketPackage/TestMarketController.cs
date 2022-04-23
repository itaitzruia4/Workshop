using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;

namespace Tests
{
    [TestClass]
    public class TestMarketController
    {
        private MarketController marketController;
        private UserController userController;
        private ISecurityHandler security;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            security = securityMock.Object;

            marketController = new MarketController();
            userController = new UserController(security);
        }

        [TestMethod]
        public void TestGetWorkersInformation_Success()
        {
            // Arrange
            string username = "user1", password = "pass1";
            userController.EnterMarket();

            // Act
            userController.Register(username, password);

            // Assert
            Assert.IsTrue(userController.IsMember(username));
        }

    }
}