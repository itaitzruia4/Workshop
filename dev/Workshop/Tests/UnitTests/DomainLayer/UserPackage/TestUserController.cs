using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using Workshop.DomainLayer.UserPackage.Shopping;

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
            reviewMock.Setup(x => x.AddReview(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns<ReviewDTO>(null);

            userController = new UserController(securityMock.Object, reviewMock.Object);
            userController.InitializeSystem();

            userController.EnterMarket();

            userController.Register("member1", "pass1");
            userController.Login("member1", "pass1");
            userController.addToCart("member1", new ShoppingBagProduct(1, "product1", "nntdd", 12.0, 1), 1);
            // TODO invoke BuyCart for member1
            // orderHandler.addOrder(new OrderDTO(1, "member1", "whatever", "blasToysRus", member1prods, 12.30), "member1");
            userController.Logout("member1");

            userController.Register("member2", "pass2");
            userController.Login("member2", "pass2");
            // TODO invoke Create store for member2
            userController.Logout("member2");

            userController.Register("member3", "pass3");
            userController.Login("member3", "pass3");
            // TODO nominate member3 to store manager
            userController.Logout("member3");

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
        public void TestReviewProduct_Success(){
            userController.EnterMarket();
            string username = "member1";
            int id = 1;
            string review = "Honest review123";
            userController.Login(username, "pass1");
            userController.ReviewProduct(username, id, review);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void TestReviewProduct_Failure(string review){
            Assert.ThrowsException<ArgumentException>(() => userController.ReviewProduct("User1", 1, review));
        }

        [DataTestMethod]
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, 1)] //normal case
        [DataRow("member2", 2, "product2", "descp", 15.0, 3, 1)] //normal case
        [DataRow("member2", 1, "product3", "desci", 4.0, 5, 1)] //normal case
        public void TestviewCart_Success(string user, int prodId, string prodName, string desc, double price, int quantity, int storeId)
        {
            userController.EnterMarket();
            userController.Login("member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(user, new ShoppingBagProduct(prodId, prodName, desc, price, quantity), storeId);
            ShoppingCartDTO shoppingCart = userController.viewCart(user);
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(product2.GetProductDTO()));
        }

        [DataTestMethod]
        [DataRow("member1", 1, "product1", "desc", 12.0, 1, 1)] //not loggedin user
        [DataRow("member3", 2, "product2", "descp", 15.0, 3, 1)] //not loggedin user
        public void TestviewCart_Failure(string user, int prodId, string prodName, string desc, double price, int quantity, int storeId)
        {
            userController.EnterMarket();
            userController.Login("member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart("member2", new ShoppingBagProduct(prodId, prodName, desc, price, quantity), storeId);
            Assert.ThrowsException<ArgumentException>(() => userController.viewCart(user));
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(product2.GetProductDTO()));
        }

        [DataTestMethod]
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, 1,1)] //normal case
        [DataRow("member2", 1, "product1", "descp", 15.0, 3, 1,1)] //normal case
        [DataRow("member2", 1, "product1", "desci", 4.0, 1, 5,1)] //normal case
        public void TestEditCart_Success(string user, int prodId, string prodName, string desc, double price, int quantity,int newQuantity, int storeId)
        {
            userController.EnterMarket();
            userController.Login("member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(user, new ShoppingBagProduct(prodId, prodName, desc, price, quantity), storeId);
            ShoppingCartDTO shoppingCart = userController.editCart(user, prodId, newQuantity);
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            ProductDTO product = product2.GetProductDTO();
            product.Quantity = newQuantity;
            Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(product));
        }

        [DataTestMethod]
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, 0, 1)] //normal case
        [DataRow("member2", 1, "product1", "descp", 15.0, 3, 0, 1)] //normal case
        public void TestEditCartDelete_Success(string user, int prodId, string prodName, string desc, double price, int quantity, int newQuantity, int storeId)
        {
            userController.EnterMarket();
            userController.Login("member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(user, new ShoppingBagProduct(prodId, prodName, desc, price, quantity), storeId);
            ShoppingCartDTO shoppingCart = userController.editCart(user, prodId, newQuantity);
            //Assert.IsTrue(shoppingCart.shoppingBags[1].products[0].EqualsFields(preInsertedProduct.GetProductDTO()));
            Assert.IsTrue(shoppingCart.shoppingBags[1].products.Count==0);
        }

        [DataTestMethod] //prodId is 1
        [DataRow("member2", 1, "product1", "desc", 12.0, 1, -1, 1)] //negative quantity
        [DataRow("member2", 5, "product5", "descp", 15.0, 3, 0, 1)] //not a real product delete
        [DataRow("member2", 5, "product5", "desci", 4.0, 1, 5, 1)] //not a real product edit
        public void TestEditCart_Failure(string user, int prodId, string prodName, string desc, double price, int quantity, int newQuantity, int storeId)
        {
            userController.EnterMarket();
            userController.Login("member2", "pass2");
            ShoppingBagProduct product2 = userController.addToCart(user, new ShoppingBagProduct(1, prodName, desc, price, quantity), storeId);
            Assert.ThrowsException<ArgumentException>(() => userController.editCart(user, prodId, newQuantity));
        }
    }
}