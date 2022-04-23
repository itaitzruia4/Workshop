using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;

namespace Tests.UnitTests.DomainLayer.UserPackage
{
    [TestClass]
    public class TestUserController
    {
        private UserController userController;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);

            var reviewMock = new Mock<IReviewHandler>();
            reviewMock.Setup(x => x.AddReview(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));

            userController = new UserController(securityMock.Object, reviewMock.Object);
            userController.InitializeSystem();
        }

        [TestMethod]
        public void TestRegister_Success()
        {
            // Arrange
            string username = "test1", password = "pass1";
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

        [TestMethod]
        public void TestRegister_RegisterTwice()
        {
            string username = "user4", password = "pass4";
            Assert.IsFalse(userController.IsMember(username));
            userController.EnterMarket();

            userController.Register(username, password);
            Assert.IsTrue(userController.IsMember(username));
            Assert.ThrowsException<ArgumentException>(() => userController.Register(username, password));
        }

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_UserExists()
        {
            string username = "member1", password = "pass12";
            Assert.IsTrue(userController.IsMember(username));

            userController.EnterMarket();
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
        [DataRow("member1", "pass1")]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestLogin_Success(string username, string password)
        {
            // Arrange
            userController.EnterMarket();

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
        [DataRow("member1", "PASS1")]
        [DataRow("member2", "PASS2")]
        [ExpectedException(typeof(ArgumentException), "Wrong password")]
        public void TestLogin_WrongPassword(string username, string password)
        {
            // Arrange
            userController.EnterMarket();

            // Act
            userController.Login(username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Current user already logged in")]
        public void TestLogin_ThisUserAlreadyLoggedIn()
        {
            string username = "member1", password = "pass1";
            userController.EnterMarket();

            userController.Login(username, password);
            userController.Login(username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Other user already logged in")]
        public void TestLogin_OtherUserAlreadyLoggedIn()
        {
            string username1 = "member1", password1 = "pass1", username2 = "member2", password2 = "pass2";
            userController.EnterMarket();

            userController.Login(username1, password1);
            userController.Login(username2, password2);
        }

        [TestMethod]
        public void TestLogout_Success()
        {
            string username = "member1", password = "pass1";
            userController.EnterMarket();
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
            string username = "member1";
            userController.EnterMarket();

            userController.Logout(username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username not equal to logged in user name")]
        public void TestLogout_OtherUserLoggedIn()
        {
            string username1 = "member1", password1 = "pass1", username2 = "member2";
            userController.EnterMarket();
            userController.Login(username1, password1);

            userController.Logout(username2);
        }

        [TestMethod]
        [DataRow("member1", "pass1", 1)]
        [DataRow("member2", "pass2", 2)]
        public void TestNominateStoreOwner_Success(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member5";
            userController.EnterMarket();
            userController.Login(nominatorName, nominatorPassword);

            // Act
            StoreOwner storeOwner = userController.NominateStoreOwner(nominatorName, nominatedName, storeId);

            // Assert
            Member nominee = userController.GetMember(nominatedName);
            List<StoreRole> nominatedStoreRoles = nominee.GetStoreRoles(storeId);
            Assert.AreEqual(nominatedStoreRoles.Count, 1);
            Assert.AreSame(storeOwner, nominatedStoreRoles[0]);
        }

        [TestMethod]
        [DataRow("member3", "pass3", 3)]
        [DataRow("member4", "pass4", 3)]
        public void TestNominateStoreOwner_NotAuthorized(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member5";
            userController.EnterMarket();
            userController.Login(nominatorName, nominatorPassword);

            // Act + Assert
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreOwner(nominatorName, nominatedName, storeId));
        }

        [TestMethod]
        [DataRow("member1", "pass1", 1)]
        [DataRow("member2", "pass2", 2)]
        public void TestNominateStoreOwner_NominatorNotLoggedIn(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member5";
            userController.EnterMarket();
            
            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreOwner(nominatorName, nominatedName, storeId));
        }

        [TestMethod]
        [DataRow("member3", "pass3", 2)]
        public void TestNominateStoreOwner_NominatedStoreOwner(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member2";
            userController.EnterMarket();
            userController.Login(nominatorName, nominatorPassword);

            // Act + Assert
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(nominatorName, nominatedName, storeId));
        }

        //TODO: TestNominateStoreOwner_CircularNomination

        [TestMethod]
        [DataRow("member1", "pass1", 1)]
        [DataRow("member2", "pass2", 2)]
        public void TestNominateStoreManager_Success(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member5";
            userController.EnterMarket();
            userController.Login(nominatorName, nominatorPassword);

            // Act
            StoreManager storeManager = userController.NominateStoreManager(nominatorName, nominatedName, storeId);

            // Assert
            Member nominee = userController.GetMember(nominatedName);
            List<StoreRole> nominatedStoreRoles = nominee.GetStoreRoles(storeId);
            Assert.AreEqual(nominatedStoreRoles.Count, 1);
            Assert.AreSame(storeManager, nominatedStoreRoles[0]);
        }


        [TestMethod]
        [DataRow("member3", "pass3", 3)]
        [DataRow("member4", "pass4", 3)]
        public void TestNominateStoreManager_NotAuthorized(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member5";
            userController.EnterMarket();
            userController.Login(nominatorName, nominatorPassword);

            // Act + Assert
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreManager(nominatorName, nominatedName, storeId));
        }

        [TestMethod]
        [DataRow("member1", "pass1", 1)]
        [DataRow("member2", "pass2", 2)]
        public void TestNominateStoreManager_NominatorNotLoggedIn(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member5";
            userController.EnterMarket();

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(nominatorName, nominatedName, storeId));
        }

        [TestMethod]
        [DataRow("member3", "pass3", 2)]
        public void TestNominateStoreManager_NominatedStoreOwner(string nominatorName, string nominatorPassword, int storeId)
        {
            // Arrange
            string nominatedName = "member2";
            userController.EnterMarket();
            userController.Login(nominatorName, nominatorPassword);

            // Act + Assert
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(nominatorName, nominatedName, storeId));
        }
    }
}