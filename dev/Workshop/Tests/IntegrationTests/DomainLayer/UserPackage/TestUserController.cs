using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using SystemAdminDTO = Workshop.ServiceLayer.ServiceObjects.SystemAdminDTO;
namespace Tests.IntegrationTests.DomainLayer.UserPackage
{
    [TestClass]
    public class TestUserController
    {
        private UserController userController;
        private int member2StoreId = 1;
        private SupplyAddress address = new SupplyAddress("Ronmi", "Mayor 1", "Ashkelon", "Israel", "784112");
        private CreditCard cc = new CreditCard("001122334455667788", "11", "26", "LeBron Michal", "555", "208143751");

        [TestInitialize]
        public void Setup()
        {
            ISecurityHandler security = new HashSecurityHandler();
            IReviewHandler review = new ReviewHandler();

            userController = new UserController(security, review, new List<SystemAdminDTO>());

            userController.EnterMarket(1, DateTime.Now);

            userController.Register(1, "member1", "pass1", DateTime.Parse("Aug 22, 1972"));
            userController.Login(1, "member1", "pass1", DateTime.Now);
            userController.AddToCart(1, new ShoppingBagProduct(1, "product1", "nntdd", 12.0, 1, "cat1", 1), 1);

            List<ShoppingBagProduct> member1prods = new List<ShoppingBagProduct>();
            member1prods.Add(new ShoppingBagProduct(1, "prod1", "desc1", 11.90, 3, "cat1", 1));
            List<ProductDTO> pdtos1 = new List<ProductDTO>();
            foreach (ShoppingBagProduct sbp in member1prods)
            {
                pdtos1.Add(sbp.GetProductDTO());
            }
            userController.AddOrder(1, new OrderDTO(1, "member1", address, 1, pdtos1, DateTime.Now,1.0), "member1");
            userController.Logout(1, "member1");

            userController.Register(1, "member3", "pass3", DateTime.Parse("Aug 22, 1972"));
            userController.Register(1, "member4", "pass4", DateTime.Parse("Aug 22, 1972"));

            userController.Register(1, "member2", "pass2", DateTime.Parse("Aug 22, 1972"));
            userController.Login(1, "member2", "pass2", DateTime.Now);
            userController.AddStoreFounder("member2", member2StoreId, DateTime.Now);

            userController.NominateStoreManager(1, "member2", "member3", member2StoreId, DateTime.Now);
            userController.AddPermissionToStoreManager(1, "member2", "member3", 1, Action.NominateStoreOwner);
            userController.AddPermissionToStoreManager(1, "member2", "member3", 1, Action.NominateStoreManager);

            //userController.NominateStoreOwner(1, "member2", "member4", member2StoreId);

            userController.Logout(1, "member2");

            userController.Register(1, "member5", "pass5", DateTime.Parse("Aug 22, 1972"));

            userController.ExitMarket(1);
        }

        /// Tests for UserController.Register method
        /// <see cref="UserController.Register"/>
        
        [TestMethod]
        public void TestRegister_Success()
        {
            // Arrange
            string username = "test1", password = "pass1";
            userController.EnterMarket(1, DateTime.Now);

            // Act
            userController.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));

            // Assert
            Assert.IsTrue(userController.IsMember(username));
        }

        [DataTestMethod]
        [DataRow("user", "")]
        [DataRow("", "pass")]
        [DataRow("", "")]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestRegister_Failure_EmptyDetails(string username, string password)
        {
            userController.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsFalse(userController.IsMember(username));
        }

        [DataTestMethod]
        [DataRow("user", null)]
        [DataRow(null, "pass")]
        [DataRow(null, null)]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestRegister_Failure_NullDetails(string username, string password)
        {
            userController.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsFalse(userController.IsMember(username));
        }

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_Failure_RegisterTwice()
        {
            string username = "user4", password = "pass4";
            Assert.IsFalse(userController.IsMember(username));
            userController.EnterMarket(1, DateTime.Now);

            userController.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(userController.IsMember(username));
            userController.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
        }

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_Failure_UserExists()
        {
            string username = "member1", password = "pass12";
            Assert.IsTrue(userController.IsMember(username));

            userController.EnterMarket(1, DateTime.Now);
            userController.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
        }

        /// Tests for UserController.EnterMarket method
        /// <see cref="UserController.EnterMarket"/>
        
        [TestMethod]
        public void TestEnterMarket_Success()
        {
            userController.EnterMarket(1, DateTime.Now);
        }

        [TestMethod]
        public void TestEnterMarket_Failure_AlreadyEntered()
        {
            userController.EnterMarket(1, DateTime.Now);
            Assert.ThrowsException<InvalidOperationException>(() => userController.EnterMarket(1, DateTime.Now));
        }

        /// Tests for UserController.Login method
        /// <see cref="UserController.Login"/>
        
        [TestMethod]
        [DataRow("member1", "pass1")]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        public void TestLogin_Success(string username, string password)
        {
            // Arrange
            userController.EnterMarket(1, DateTime.Now);

            // Act
            userController.Login(1, username, password, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username does not exist")]
        public void TestLogin_Failure_NoSuchUser()
        {
            // Arrange
            string username = "user0", password = "password0";
            userController.EnterMarket(1, DateTime.Now);

            // Act
            userController.Login(1, username, password, DateTime.Now);
        }

        [TestMethod]
        [DataRow("user", "")]
        [DataRow("", "pass")]
        [DataRow("", "")]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestLogin_Failure_EmptyDetails(string username, string password)
        {
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, username, password, DateTime.Now);
        }

        [TestMethod]
        [DataRow("user", null)]
        [DataRow(null, "pass")]
        [DataRow(null, null)]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be null")]
        public void TestLogin_Failure_NullDetails(string username, string password)
        {
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, username, password, DateTime.Now);
        }

        [TestMethod]
        [DataRow("member1", "PASS1")]
        [DataRow("member2", "PASS2")]
        [ExpectedException(typeof(ArgumentException), "Wrong password")]
        public void TestLogin_Failure_WrongPassword(string username, string password)
        {
            // Arrange
            userController.EnterMarket(1, DateTime.Now);

            // Act
            userController.Login(1, username, password, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Current user already logged in")]
        public void TestLogin_Failure_ThisUserAlreadyLoggedIn()
        {
            string username = "member1", password = "pass1";
            userController.EnterMarket(1, DateTime.Now);

            userController.Login(1, username, password, DateTime.Now);
            userController.Login(1, username, password, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Other user already logged in")]
        public void TestLogin_Failure_OtherUserAlreadyLoggedIn()
        {
            string username1 = "member1", password1 = "pass1", username2 = "member2", password2 = "pass2";
            userController.EnterMarket(1, DateTime.Now);

            userController.Login(1, username1, password1, DateTime.Now);
            userController.Login(1, username2, password2, DateTime.Now);
        }

        /// Tests for UserController.Logout method
        /// <see cref="UserController.Logout"/>
        /// 
        [TestMethod]
        public void TestLogout_Success()
        {
            string username = "member1", password = "pass1";
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, username, password, DateTime.Now);

            userController.Logout(1, username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No such user")]
        public void TestLogout_Failure_NoSuchUser()
        {
            userController.Logout(1, "imaginary_user");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No user logged in")]
        public void TestLogout_Failure_NoUserLoggedIn()
        {
            string username = "member1";
            userController.EnterMarket(1, DateTime.Now);

            userController.Logout(1, username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username not equal to logged in user name")]
        public void TestLogout_Failure_OtherUserLoggedIn()
        {
            string username1 = "member1", password1 = "pass1", username2 = "member2";
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, username1, password1, DateTime.Now);

            userController.Logout(1, username2);
        }

        /// Tests for UserController.NominateStoreManager method
        /// <see cref="UserController.NominateStoreManager"/>
        [TestMethod]
        [DataRow("member2")]
        [DataRow("member3")]
        [DataRow("member4")]
        public void TestNominateStoreManager_Failure_NominatorNotLoggedIn(string nominator)
        {
            userController.EnterMarket(1, DateTime.Now);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(1, nominator, "member1", member2StoreId, DateTime.Now));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_Failure_NoSuchNominated(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, nominator, nominatorPassword, DateTime.Now);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(1, nominator, "arya stark", member2StoreId, DateTime.Now));
        }

        [TestMethod]
        public void TestNominateStoreManager_Failure_NoPermission()
        {
            userController.EnterMarket(1, DateTime.Now);
            string nominator = "member1";
            userController.Login(1, nominator, "pass1", DateTime.Now);
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreManager(1, nominator, "member5", member2StoreId, DateTime.Now));
        }

        /// Tests for UserController.ReviewProduct method
        /// <see cref="UserController.ReviewProduct"/>
        
        [TestMethod]
        public void TestReviewProduct_Success()
        {
            userController.EnterMarket(1, DateTime.Now);
            string username = "member1";
            int id = 1;
            string review = "Honest review123";
            userController.Login(1, username, "pass1", DateTime.Now);
            ReviewDTO dto = userController.ReviewProduct(1, username, id, review, 4);
            Assert.AreEqual(review, dto.Review);
            Assert.AreEqual(dto.Reviewer, username);
            Assert.AreEqual(dto.ProductId, id);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void TestReviewProduct_Failure_EmptyOrNullReview(string review)
        {
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, "member1", "pass1", DateTime.Now);
            Assert.ThrowsException<ArgumentException>(() => userController.ReviewProduct(1, "member1", 1, review, 4));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(11)]
        public void TestReviewProduct_Failure_OutOfRangeRating(int rating)
        {
            userController.EnterMarket(1, DateTime.Now);
            userController.Login(1, "member1", "pass1", DateTime.Now);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => userController.ReviewProduct(1, "member1", 1, "TestReview", rating));
        }
    }
}