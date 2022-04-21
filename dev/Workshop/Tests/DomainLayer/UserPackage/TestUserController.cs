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
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            security = securityMock.Object;

            userController = new UserController(security);
        }

        [TestMethod]
        public void TestRegister_Success()
        {
            // Arrange
            string username = "user1", password = "pass1";
            userController.EnterMarket();

            // Act
            userController.Register(username, password);

            // Assert
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

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_UserExists()
        {
            string username = "user4", password = "pass4";
            Assert.IsFalse(userController.IsMember(username));
            userController.EnterMarket();

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
        [ExpectedException(typeof(InvalidOperationException), "Cannot enter market twice in a row from the same thread")]
        public void TestEnterMarket_AlreadyEntered()
        {
            userController.EnterMarket();
            userController.EnterMarket();
        }

        [TestMethod]
        public void TestLogin_Success()
        {
            // Arrange
            string username = "user53", password = "pass53";
            userController.EnterMarket();
            userController.Register(username, password);

            // Act
            userController.Login(username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username does not exist")]
        public void TestLogin_NoSuchUser()
        {
            // Arrange
            string username = "user0", password = "password0";
            userController.EnterMarket();

            // Act
            userController.Login(username, password);
        }

        [TestMethod]
        [DataRow("user", "")]
        [DataRow("", "pass")]
        [DataRow("", "")]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestLogin_EmptyDetails(string username, string password)
        {
            userController.EnterMarket();
            userController.Login(username, password);
        }

        [TestMethod]
        [DataRow("user", null)]
        [DataRow(null, "pass")]
        [DataRow(null, null)]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be null")]
        public void TestLogin_NullDetails(string username, string password)
        {
            userController.EnterMarket();
            userController.Login(username, password);
        }

        [TestMethod]
        [DataRow("user1", "pass1", "PASS1")]
        [DataRow("user2", "pass2", "PASS2")]
        [ExpectedException(typeof(ArgumentException), "Wrong password")]
        public void TestLogin_WrongPassword(string username, string originalPassword, string wrongPassword)
        {
            // Arrange
            userController.EnterMarket();
            userController.Register(username, originalPassword);

            // Act
            userController.Login(username, wrongPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Current user already logged in")]
        public void TestLogin_ThisUserAlreadyLoggedIn()
        {
            string username = "user1", password = "pass1";
            userController.EnterMarket();
            userController.Register(username, password);

            userController.Login(username, password);
            userController.Login(username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Other user already logged in")]
        public void TestLogin_OtherUserAlreadyLoggedIn()
        {
            string username1 = "user1", password1 = "pass1", username2 = "user2", password2 = "pass2";
            userController.EnterMarket();
            userController.Register(username1, password1);
            userController.Register(username2, password2);

            userController.Login(username1, password1);
            userController.Login(username2, password2);
        }

        [TestMethod]
        public void TestLogout_Success()
        {
            string username = "user", password = "pass";
            userController.EnterMarket();
            userController.Register(username, password);
            userController.Login(username, password);

            userController.Logout(username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No such user")]
        public void TestLogout_NoSuchUser()
        {
            userController.Logout("imaginary_user");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No user logged in")]
        public void TestLogout_NoUserLoggedIn()
        {
            string username = "user", password = "pass";
            userController.EnterMarket();
            userController.Register(username, password);
            
            userController.Logout(username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username not equal to logged in user name")]
        public void TestLogout_OtherUserLoggedIn()
        {
            string username1 = "user1", password1 = "pass1", username2 = "user2", password2 = "pass2";
            userController.EnterMarket();
            userController.Register(username1, password1);
            userController.Register(username2, password2);
            userController.Login(username1, password1);

            userController.Logout(username2);
        }
    }
}
