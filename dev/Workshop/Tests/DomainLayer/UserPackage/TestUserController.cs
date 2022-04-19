using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;

namespace Tests
{
    [TestClass]
    public class TestUserController
    {
        private ISecurityHandler security;
        private UserController userController;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s.ToUpper());
            security = securityMock.Object;

            userController = new UserController(security);
        }

        [DataTestMethod]
        [DataRow("user1", "pass1")]
        [DataRow("user2", "pass2")]
        public void TestRegister_Success(string username, string password)
        {
            userController.Register(username, password);
            Assert.IsTrue(userController.IsMember(username));
        }

        [DataTestMethod]
        [DataRow("user", "")]
        [DataRow("", "pass")]
        [DataRow("", "")]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestRegister_EmptyDetails(string username, string password)
        {
            userController.Register(username, password);
            Assert.IsFalse(userController.IsMember(username));
        }

        [DataTestMethod]
        [DataRow("user", null)]
        [DataRow(null, "pass")]
        [DataRow(null, null)]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestRegister_NullDetails(string username, string password)
        {
            userController.Register(username, password);
            Assert.IsFalse(userController.IsMember(username));
        }

        [TestMethod]
        [DataRow("user", "pass")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_UserExists(string username, string password)
        {
            Assert.IsFalse(userController.IsMember(username));
            userController.Register(username, password);
            Assert.IsTrue(userController.IsMember(username));
            userController.Register(username, password);
        }

        [TestMethod]
        public void TestEnterMarket_Success()
        {
            userController.EnterMarket();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "You have already entered the market")]
        public void TestEnterMarket_AlreadyEntered()
        {
            userController.EnterMarket();
            userController.EnterMarket();
        }
    }
}
