using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Tests.IntegrationTests.DomainLayer.UserPackage
{
    [TestClass]
    public class TestUserController
    {
        private UserController userController;
        private int member2StoreId = 1;

        [TestInitialize]
        public void Setup()
        {
            ISecurityHandler security = new HashSecurityHandler();
            IReviewHandler review = new ReviewHandler();

            userController = new UserController(security, review);
            userController.InitializeSystem();

            userController.EnterMarket();

            userController.Register("member1", "pass1");
            userController.Login("member1", "pass1");
            userController.addToCart("member1", new ShoppingBagProduct(1, "product1", "nntdd", 12.0, 1), 1);
            // TODO invoke BuyCart for member1
            // orderHandler.addOrder(new OrderDTO(1, "member1", "whatever", "blasToysRus", member1prods, 12.30), "member1");
            userController.Logout("member1");

            userController.Register("member3", "pass3");
            userController.Register("member4", "pass4");

            userController.Register("member2", "pass2");
            userController.Login("member2", "pass2");
            userController.AddStoreFounder("member2", member2StoreId);

            userController.NominateStoreManager("member2", "member3", member2StoreId);
            userController.AddPermissionToStoreManager("member2", "member3", 1, Action.NominateStoreOwner);
            userController.AddPermissionToStoreManager("member2", "member3", 1, Action.NominateStoreManager);

            userController.NominateStoreOwner("member2", "member4", member2StoreId);

            userController.Logout("member2");

            userController.Register("member5", "pass5");

            userController.ExitMarket();
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

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_RegisterTwice()
        {
            string username = "user4", password = "pass4";
            Assert.IsFalse(userController.IsMember(username));
            userController.EnterMarket();

            userController.Register(username, password);
            Assert.IsTrue(userController.IsMember(username));
            userController.Register(username, password);
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
        public void TestEnterMarket_AlreadyEntered()
        {
            userController.EnterMarket();
            Assert.ThrowsException<InvalidOperationException>(() => userController.EnterMarket());
        }

        [TestMethod]
        [DataRow("member1", "pass1")]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
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
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_Success(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            userController.NominateStoreOwner(nominator, "member1", member2StoreId);
        }

        [TestMethod]
        [DataRow("member2")]
        [DataRow("member3")]
        [DataRow("member4")]
        public void TestNominateStoreOwner_NominatorNotLoggedIn(string nominator)
        {
            userController.EnterMarket();
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreOwner(nominator, "member1", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_NoSuchNominated(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreOwner(nominator, "arya stark", member2StoreId));
        }

        [TestMethod]
        public void TestNominateStoreOwner_NoPermission()
        {
            userController.EnterMarket();
            string nominator = "member1";
            userController.Login(nominator, "pass1");
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreOwner(nominator, "member5", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        public void TestNominateStoreOwner_NominatedAlreadyStoreOwner(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(nominator, "member4", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_SelfNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(nominator, nominator, member2StoreId));
        }

        [TestMethod]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_CircularNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(nominator, "member2", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_Success(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            userController.NominateStoreManager(nominator, "member1", member2StoreId);
        }

        [TestMethod]
        [DataRow("member2")]
        [DataRow("member3")]
        [DataRow("member4")]
        public void TestNominateStoreManager_NominatorNotLoggedIn(string nominator)
        {
            userController.EnterMarket();
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(nominator, "member1", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_NoSuchNominated(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(nominator, "arya stark", member2StoreId));
        }

        [TestMethod]
        public void TestNominateStoreManager_NoPermission()
        {
            userController.EnterMarket();
            string nominator = "member1";
            userController.Login(nominator, "pass1");
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreManager(nominator, "member5", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        public void TestNominateStoreManager_NominatedAlreadyStoreOwner(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(nominator, "member4", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_NominatedAlreadyStoreManager(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(nominator, "member3", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_SelfNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(nominator, nominator, member2StoreId));
        }

        [TestMethod]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_CircularNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket();
            userController.Login(nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(nominator, "member2", member2StoreId));
        }

        [TestMethod]
        public void TestReviewProduct_Success()
        {
            userController.EnterMarket();
            string username = "member1";
            int id = 1;
            string review = "Honest review123";
            userController.Login(username, "pass1");
            ReviewDTO dto = userController.ReviewProduct(username, id, review);
            Assert.Equals(review, dto.Review);
            Assert.Equals(dto.Reviewer, username);
            Assert.Equals(dto.ProductId, id);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void TestReviewProduct_Failure_EmptyOrNullReview(string review)
        {
            Assert.ThrowsException<ArgumentException>(() => userController.ReviewProduct("User1", 1, review));
        }
    }
}