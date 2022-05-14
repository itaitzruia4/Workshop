using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using ShoppingCartDTO = Workshop.DomainLayer.UserPackage.Shopping.ShoppingCartDTO;

namespace Tests.UnitTests.DomainLayer.UserPackage
{
    [TestClass]
    public class TestUserController
    {
        private UserController userController;
        private int member2StoreId = 1;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);

            var reviewMock = new Mock<IReviewHandler>();
            reviewMock.Setup(x => x.AddReview(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                                   .Returns((string u, int pid, string r, int i) => new ReviewDTO(u, pid, r, i));

            userController = new UserController(securityMock.Object, reviewMock.Object);
            userController.InitializeSystem();

            userController.EnterMarket(1);

            userController.Register(1, "member1", "pass1", 40);
            userController.Login(1, "member1", "pass1");
            userController.addToCart(1, "member1", new ShoppingBagProduct(1, "product1", "nntdd", 12.0, 1, "cat1"), 1);
            
            List<ShoppingBagProduct> member1prods = new List<ShoppingBagProduct>();
            member1prods.Add(new ShoppingBagProduct(1, "prod1", "desc1", 11.90, 3, "cat1"));
            userController.AddOrder(1, new OrderDTO(1, "member1", "whatever", "blasToysRus", member1prods, 12.30), "member1");
            userController.Logout(1, "member1");

            userController.Register(1, "member3", "pass3", 40);
            userController.Register(1, "member4", "pass4", 40);

            userController.Register(1, "member2", "pass2", 40);
            userController.Login(1, "member2", "pass2");
            userController.AddStoreFounder("member2", member2StoreId);

            userController.NominateStoreManager(1, "member2", "member3", member2StoreId);
            userController.AddPermissionToStoreManager(1, "member2", "member3", 1, Action.NominateStoreOwner);
            userController.AddPermissionToStoreManager(1, "member2", "member3", 1, Action.NominateStoreManager);

            userController.NominateStoreOwner(1, "member2", "member4", member2StoreId);

            userController.Logout(1, "member2");

            userController.Register(1, "member5", "pass5", 40);

            userController.ExitMarket(1);
        }

        [TestMethod]
        public void TestRegister_Success()
        {
            // Arrange
            string username = "test1", password = "pass1";
            userController.EnterMarket(1);

            // Act
            userController.Register(1, username, password, 40);

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
            userController.Register(1, username, password, 40);
            Assert.IsFalse(userController.IsMember(username));
        }

        [DataTestMethod]
        [DataRow("user", null)]
        [DataRow(null, "pass")]
        [DataRow(null, null)]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestRegister_NullDetails(string username, string password)
        {
            userController.Register(1, username, password, 40);
            Assert.IsFalse(userController.IsMember(username));
        }

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_RegisterTwice()
        {
            string username = "user4", password = "pass4";
            Assert.IsFalse(userController.IsMember(username));
            userController.EnterMarket(1);

            userController.Register(1, username, password, 40);
            Assert.IsTrue(userController.IsMember(username));
            userController.Register(1, username, password, 40);
        }

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegister_UserExists()
        {
            string username = "member1", password = "pass12";
            Assert.IsTrue(userController.IsMember(username));

            userController.EnterMarket(1);
            userController.Register(1, username, password, 40);
        }

        [TestMethod]
        public void TestEnterMarket_Success()
        {
            userController.EnterMarket(1);
        }

        [TestMethod]
        public void TestEnterMarket_AlreadyEntered()
        {
            userController.EnterMarket(1);
            Assert.ThrowsException<InvalidOperationException>(() => userController.EnterMarket(1));
        }

        [TestMethod]
        [DataRow("member1", "pass1")]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        public void TestLogin_Success(string username, string password)
        {
            // Arrange
            userController.EnterMarket(1);

            // Act
            userController.Login(1, username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username does not exist")]
        public void TestLogin_NoSuchUser()
        {
            // Arrange
            string username = "user0", password = "password0";
            userController.EnterMarket(1);

            // Act
            userController.Login(1, username, password);
        }

        [TestMethod]
        [DataRow("user", "")]
        [DataRow("", "pass")]
        [DataRow("", "")]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be empty")]
        public void TestLogin_EmptyDetails(string username, string password)
        {
            userController.EnterMarket(1);
            userController.Login(1, username, password);
        }

        [TestMethod]
        [DataRow("user", null)]
        [DataRow(null, "pass")]
        [DataRow(null, null)]
        [ExpectedException(typeof(ArgumentException), "Username or password cannot be null")]
        public void TestLogin_NullDetails(string username, string password)
        {
            userController.EnterMarket(1);
            userController.Login(1, username, password);
        }

        [TestMethod]
        [DataRow("member1", "PASS1")]
        [DataRow("member2", "PASS2")]
        [ExpectedException(typeof(ArgumentException), "Wrong password")]
        public void TestLogin_WrongPassword(string username, string password)
        {
            // Arrange
            userController.EnterMarket(1);

            // Act
            userController.Login(1, username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Current user already logged in")]
        public void TestLogin_ThisUserAlreadyLoggedIn()
        {
            string username = "member1", password = "pass1";
            userController.EnterMarket(1);

            userController.Login(1, username, password);
            userController.Login(1, username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Other user already logged in")]
        public void TestLogin_OtherUserAlreadyLoggedIn()
        {
            string username1 = "member1", password1 = "pass1", username2 = "member2", password2 = "pass2";
            userController.EnterMarket(1);

            userController.Login(1, username1, password1);
            userController.Login(1, username2, password2);
        }

        [TestMethod]
        public void TestLogout_Success()
        {
            string username = "member1", password = "pass1";
            userController.EnterMarket(1);
            userController.Login(1, username, password);

            userController.Logout(1, username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No such user")]
        public void TestLogout_NoSuchUser()
        {
            userController.Logout(1, "imaginary_user");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No user logged in")]
        public void TestLogout_NoUserLoggedIn()
        {
            string username = "member1";
            userController.EnterMarket(1);

            userController.Logout(1, username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Username not equal to logged in user name")]
        public void TestLogout_OtherUserLoggedIn()
        {
            string username1 = "member1", password1 = "pass1", username2 = "member2";
            userController.EnterMarket(1);
            userController.Login(1, username1, password1);

            userController.Logout(1, username2);
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_Success(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            userController.NominateStoreOwner(1, nominator, "member1", member2StoreId);
        }

        [TestMethod]
        [DataRow("member2")]
        [DataRow("member3")]
        [DataRow("member4")]
        public void TestNominateStoreOwner_NominatorNotLoggedIn(string nominator)
        {
            userController.EnterMarket(1);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreOwner(1, nominator, "member1", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_NoSuchNominated(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreOwner(1, nominator, "arya stark", member2StoreId));
        }

        [TestMethod]
        public void TestNominateStoreOwner_NoPermission()
        {
            userController.EnterMarket(1);
            string nominator = "member1";
            userController.Login(1, nominator, "pass1");
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreOwner(1, nominator, "member5", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        public void TestNominateStoreOwner_NominatedAlreadyStoreOwner(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(1, nominator, "member4", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_SelfNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(1, nominator, nominator, member2StoreId));
        }

        [TestMethod]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreOwner_CircularNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreOwner(1, nominator, "member2", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_Success(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            userController.NominateStoreManager(1, nominator, "member1", member2StoreId);
        }

        [TestMethod]
        [DataRow("member2")]
        [DataRow("member3")]
        [DataRow("member4")]
        public void TestNominateStoreManager_NominatorNotLoggedIn(string nominator)
        {
            userController.EnterMarket(1);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(1, nominator, "member1", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_NoSuchNominated(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<ArgumentException>(() => userController.NominateStoreManager(1, nominator, "arya stark", member2StoreId));
        }

        [TestMethod]
        public void TestNominateStoreManager_NoPermission()
        {
            userController.EnterMarket(1);
            string nominator = "member1";
            userController.Login(1, nominator, "pass1");
            Assert.ThrowsException<MemberAccessException>(() => userController.NominateStoreManager(1, nominator, "member5", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        public void TestNominateStoreManager_NominatedAlreadyStoreOwner(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(1, nominator, "member4", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_NominatedAlreadyStoreManager(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(1, nominator, "member3", member2StoreId));
        }

        [TestMethod]
        [DataRow("member2", "pass2")]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_SelfNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(1, nominator, nominator, member2StoreId));
        }

        [TestMethod]
        [DataRow("member3", "pass3")]
        [DataRow("member4", "pass4")]
        public void TestNominateStoreManager_CircularNomination(string nominator, string nominatorPassword)
        {
            userController.EnterMarket(1);
            userController.Login(1, nominator, nominatorPassword);
            Assert.ThrowsException<InvalidOperationException>(() => userController.NominateStoreManager(1, nominator, "member2", member2StoreId));
        }

        [TestMethod]
        public void TestReviewProduct_Success(){
            userController.EnterMarket(1);
            string username = "member1";
            int id = 1;
            int rating = 4;
            string review = "Honest review123";
            userController.Login(1, username, "pass1");
            ReviewDTO dto = userController.ReviewProduct(1, username, id, review, rating);
            Assert.AreEqual(review, dto.Review);
            Assert.AreEqual(dto.Reviewer, username);
            Assert.AreEqual(dto.ProductId, id);
            Assert.AreEqual(dto.Rating, rating);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void TestReviewProduct_Failure_EmptyOrNullReview(string review){
            Assert.ThrowsException<KeyNotFoundException>(() => userController.ReviewProduct(1, "User1", 1, review, 4));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(11)]
        public void TestReviewProduct_Failure_OutOfRangeRating(int rating)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => userController.ReviewProduct(1, "User1", 1, "TestReview", rating));
        }

        [DataTestMethod]
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, 1, "cat1")] //normal case
        [DataRow("member2", 2, "product2", "descp", 15.0, 3, 1, "cat1")] //normal case
        [DataRow("member2", 1, "product3", "desci", 4.0, 5, 1, "cat1")] //normal case
        public void TestviewCart_Success(string user, int prodId, string prodName, string desc, double price, int quantity, int storeId, string category)
        {
            userController.EnterMarket(1);
            userController.Login(1, "member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(1, user, new ShoppingBagProduct(prodId, prodName, desc, price, quantity, category), storeId);
            ShoppingCartDTO shoppingCart = userController.viewCart(1, user);
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(product2.GetProductDTO()));
        }

        [DataTestMethod]
        [DataRow("member1", 1, "product1", "desc", 12.0, 1, 1, "cat1")] //not loggedin user
        [DataRow("member3", 2, "product2", "descp", 15.0, 3, 1, "cat1")] //not loggedin user
        public void TestviewCart_Failure(string user, int prodId, string prodName, string desc, double price, int quantity, int storeId, string category)
        {
            userController.EnterMarket(1);
            userController.Login(1, "member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(1, "member2", new ShoppingBagProduct(prodId, prodName, desc, price, quantity, category), storeId);
            Assert.ThrowsException<ArgumentException>(() => userController.viewCart(1, user));
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(product2.GetProductDTO()));
        }

        [DataTestMethod]
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, 1,1, "cat1")] //normal case
        [DataRow("member2", 1, "product1", "descp", 15.0, 3, 1,1, "cat1")] //normal case
        [DataRow("member2", 1, "product1", "desci", 4.0, 1, 5,1, "cat1")] //normal case
        public void TestEditCart_Success(string user, int prodId, string prodName, string desc, double price, int quantity,int newQuantity, int storeId, string category)
        {
            userController.EnterMarket(1);
            userController.Login(1, "member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(1, user, new ShoppingBagProduct(prodId, prodName, desc, price, quantity, category), storeId);
            ShoppingCartDTO shoppingCart = userController.editCart(1, user, prodId, newQuantity);
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            ProductDTO product = product2.GetProductDTO();
            product.Quantity = newQuantity;
            Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(product));
        }

        [DataTestMethod]
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, 0, 1, "cat1")] //normal case
        [DataRow("member2", 1, "product1", "descp", 15.0, 3, 0, 1, "cat1")] //normal case
        public void TestEditCartDelete_Success(string user, int prodId, string prodName, string desc, double price, int quantity, int newQuantity, int storeId, string category)
        {
            userController.EnterMarket(1);
            userController.Login(1, "member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(1, user, new ShoppingBagProduct(prodId, prodName, desc, price, quantity, category), storeId);
            ShoppingCartDTO shoppingCart = userController.editCart(1, user, prodId, newQuantity);
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            Assert.IsTrue(shoppingCart.shoppingBags[1].products.Count==0);
        }

        [DataTestMethod] //prodId is 1
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, -1, 1, "cat1")] //negative quantity
        [DataRow("member2", 5, "product5", "descp", 15.0, 3, 0, 1, "cat1")] //not a real product delete
        [DataRow("member2", 5, "product5", "desci", 4.0, 1, 5, 1, "cat1")] //not a real product edit
        public void TestEditCart_Failure(string user, int prodId, string prodName, string desc, double price, int quantity, int newQuantity, int storeId, string category)
        {
            userController.EnterMarket(1);
            userController.Login(1, "member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(1, user, new ShoppingBagProduct(1, prodName, desc, price, quantity, category), storeId);
            Assert.ThrowsException<ArgumentException>(() => userController.editCart(1, user, prodId, newQuantity));
        }
    }
}