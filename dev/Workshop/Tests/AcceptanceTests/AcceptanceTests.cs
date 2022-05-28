using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workshop.DomainLayer.Reviews;

namespace Tests.AcceptanceTests
{
    [TestClass]
    public class AcceptanceTests
    {
        IService service;
        private const string username = "Goodun";
        private const string password = "Goodp";
        private const string product = "product";

        [TestInitialize]
        public void InitSystem()
        {
            service = new Service();
        }

        [TestMethod]
        public void TestSystemInitiation()
        {
            Assert.IsNotNull(service);
            //Assert.IsNotNull(srv.getSystemmanager()); //Further tests will come later on
            //Assert.IsNotNull(srv.getExternalConnections());
        }

        [TestMethod]
        public void TestEnterMarket_Good()
        {
            Response<User> ru = service.EnterMarket(1);
            Assert.IsFalse(ru.ErrorOccured);
            Assert.IsNotNull(ru.Value);
        }

        [TestMethod]
        public void TestEnterMarket_Bad()
        {
            Assert.IsFalse(service.EnterMarket(1).ErrorOccured);
            Assert.IsTrue(service.EnterMarket(1).ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Good()
        {
            service.EnterMarket(1);
            Assert.IsFalse(service.ExitMarket(1).ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Bad_NeverEntered()
        {
            Assert.IsTrue(service.ExitMarket(1).ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Bad_ExitTwice()
        {
            service.EnterMarket(1);
            Assert.IsFalse(service.ExitMarket(1).ErrorOccured);
            Assert.IsTrue(service.ExitMarket(1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(1, username, password)]
        public void TestRegister_Good(int userId, string username, string password)
        {
            service.EnterMarket(userId);
            Assert.IsFalse(service.Register(userId, username, password, DateTime.Parse("Aug 22, 1972")).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        [DataRow("TestRegister_Bad", password)]
        [DataRow(username, "TestRegister_Bad")]
        public void TestRegister_Bad(string username, string password)
        {
            service.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.Register(1, username, password, DateTime.Parse("Aug 22, 1972")).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(1, username, password)]
        public void TestLogin_Good(int userId, string username, string password)
        {
            service.EnterMarket(userId);
            service.Register(userId, username, password, DateTime.Parse("Aug 22, 1972"));
            Response<KeyValuePair<Member, List<Notification>>> rMember = service.Login(userId, username, password);
            Assert.IsFalse(rMember.ErrorOccured);
            Assert.IsNotNull(rMember.Value.Value);
            Assert.IsNotNull(rMember.Value.Key);
        }

        [DataTestMethod]
        [DataRow("Fake1", "Fake2")]
        [DataRow("", "")]
        [DataRow("", "Fake2")]
        [DataRow("Fake1", "")]
        public void TestLogin_Bad_NoSuchUser(string username, string password)
        {
            Assert.IsTrue(service.Login(1, username, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, username)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestLogin_Bad_WrongPassword(string username, string password, string wrongPassword)
        {
            service.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.Login(1, username, wrongPassword).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, password)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestLogin_Bad_WrongUsername(string username, string password, string wrongUsername)
        {
            service.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.Login(1, wrongUsername, password).ErrorOccured);
        }

        public bool Login_Thread(int userId, string username, string password)
        {
            return service.Login(userId, username, password).ErrorOccured;
        }


        [TestMethod]
        public void TestLogin_Bad_LoginTwiceAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            service.EnterMarket(1);
            Assert.IsFalse(service.Register(1, "user1", "1", new DateTime(2001, 11, 17)).ErrorOccured);

            Thread thr1 = new Thread(() => res1 = Login_Thread(1, "user1", "1"));
            Thread thr2 = new Thread(() => res2 = Login_Thread(1, "user1", "1"));
            thr1.Start();
            thr2.Start();
            thr1.Join();
            thr2.Join();

            Assert.AreNotEqual(res1, res2);
        }

        [DataTestMethod]
        [DataRow(1, username, password)]
        public void TestLogout_Good(int userId, string username, string password)
        {
            TestLogin_Good(userId, username, password);
            Assert.IsFalse(service.Logout(userId, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogout_Bad_LogoutTwice(string username, string password)
        {
            TestLogin_Good(1, username, password);
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            Assert.IsTrue(service.Logout(1, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username)]
        [DataRow(null)]
        [DataRow("")]
        public void TestLogout_Bad_NoSuchUser(string username)
        {
            Assert.IsTrue(service.Logout(1, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogout_Bad_NotLoggedIn(string username, string password)
        {
            TestRegister_Good(1, username, password);
            Assert.IsTrue(service.Logout(1, null).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, product)]
        public Product TestAddProduct_Good(string username, string password, string product)
        {
            int storeId = 0;
            TestLogin_Good(1, username, password);
            storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Response<Product> prodResult = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1");
            Assert.IsFalse(prodResult.ErrorOccured);
            Assert.IsInstanceOfType(prodResult.Value, typeof(Product));
            return prodResult.Value;
        }

        [DataTestMethod]
        [DataRow(username, password, "no", "perm")]
        public void TestAddProduct_Bad_NoPermission(string username, string password, string noPermU, string noPermP)
        {
            TestLogin_Good(1, username, password);
            TestLogin_Good(2, noPermU, noPermP);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.AddProduct(2, noPermU, storeId, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
            Assert.IsTrue(service.AddProduct(2, username, storeId, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
            Assert.IsTrue(service.AddProduct(1, noPermU, storeId, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int TestNominateStoreOwner_Good(string username, string password, string nominated)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestRegister_Good(2, nominated, nominated);
            Assert.IsFalse(service.NominateStoreOwner(1, username, nominated, storeId).ErrorOccured);
            return storeId;
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public void TestNominateStoreOwner_Bad_NominateTwice(string username, string password, string nominated)
        {
            int storeId = TestNominateStoreOwner_Good(username, password, nominated);
            Assert.IsTrue(service.NominateStoreOwner(1, username, nominated, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestNominateStoreOwner_Bad_NominateUsrself(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.NominateStoreOwner(1, username, username, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void TestNominateStoreOwner_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestRegister_Good(2, nominated, nominated);
            TestRegister_Good(3, nominator, nominator);
            Assert.IsTrue(service.NominateStoreOwner(3, nominator, nominated, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int TestNominateStoreManager_Good(string username, string password, string nominated)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestRegister_Good(2, nominated, nominated);
            Assert.IsFalse(service.NominateStoreManager(1, username, nominated, storeId).ErrorOccured);
            return storeId;
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public void TestNominateStoreManager_Bad_NominateTwice(string username, string password, string nominated)
        {
            int storeId = TestNominateStoreManager_Good(username, password, nominated);
            Assert.IsTrue(service.NominateStoreManager(1, username, nominated, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestNominateStoreManager_Bad_NominateUsrself(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.NominateStoreManager(1, username, username, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void TestNominateStoreManager_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            TestRegister_Good(2, nominated, nominated);
            TestLogin_Good(3, nominator, nominator);
            Assert.IsTrue(service.NominateStoreManager(3, nominator, nominated, storeId).ErrorOccured);
        }


        public bool NominateStoreManager_Thread(int userId, string nominator, string password, string nominated, int storeId)
        {
            //Assert.IsFalse(service.EnterMarket(userId).ErrorOccured);
            Assert.IsFalse(service.Login(userId, nominator, password).ErrorOccured);

            return service.NominateStoreManager(userId, nominator, nominated, storeId).ErrorOccured;
        }


        [TestMethod]
        public void TestNominateStoreManager_BadNominateTwiceAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            service.EnterMarket(1);
            service.EnterMarket(2);
            service.EnterMarket(3);
            service.EnterMarket(4);
            Assert.IsFalse(service.Register(1, "Nominated", "none", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(2, "Nominator1", "1", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(3, "Nominator2", "2", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(4, "Owner", "own", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Login(4, "Owner", "own").ErrorOccured);

            int storeId = service.CreateNewStore(4, "Owner", "RandomStore").Value.StoreId;
            Assert.IsFalse(service.NominateStoreOwner(4, "Owner", "Nominator1", storeId).ErrorOccured);
            Assert.IsFalse(service.NominateStoreOwner(4, "Owner", "Nominator2", storeId).ErrorOccured);

            Thread thr1 = new Thread(() => res1 = NominateStoreManager_Thread(2, "Nominator1", "1", "Nominated", storeId));
            Thread thr2 = new Thread(() => res2 = NominateStoreManager_Thread(3, "Nominator2", "2", "Nominated", storeId));
            thr1.Start();
            thr2.Start();
            thr1.Join();
            thr2.Join();

            Assert.AreNotEqual(res1, res2);
        }


        [DataTestMethod]
        [DataRow(username, password)]
        public void TestGetWorkersInformation_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.GetWorkersInformation(1, username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void TestGetWorkersInformation_Bad(string username, string password, string npUser)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            TestLogin_Good(2, npUser, npUser);
            Assert.IsTrue(service.GetWorkersInformation(2, npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestCloseStore_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.CloseStore(1, username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void TestCloseStore_Bad(string username, string password, string npUser)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, username).Value.StoreId;
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            TestLogin_Good(2, npUser, npUser);
            Assert.IsTrue(service.CloseStore(2, npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public int TestCreateNewStore_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            Response<Store> res = service.CreateNewStore(1, username, "RandomStore");
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(res.Value.StoreId >= 0);
            return res.Value.StoreId;
        }

        [DataTestMethod]
        [DataRow(username, password)]
        [DataRow("", password)]
        [DataRow(username, "")]
        [DataRow("", "")]
        [DataRow(null, null)]
        public void TestCreateNewStore_Bad(string username, string password)
        {
            Assert.IsTrue(service.CreateNewStore(1, username, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            int prodId = service.AddProduct(1, username, storeId, "TestReviewProduct", "Good", 1, 2, "cat1").Value.Id;
            service.addToCart(1, username, prodId, storeId, 1);
            service.BuyCart(1, username, "Ronmi's home");
            Assert.IsFalse(service.ReviewProduct(1, username, 0, "Blank", 6).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_userLoggeedOut(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, "TestReviewProduct", "Good", 1, 1, "cat1");
            service.Logout(1, username);
            Assert.IsTrue(service.ReviewProduct(1, username, 0, "Blank", 6).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_noSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, "TestReviewProduct", "Good", 1, 1, "cat1");
            Assert.IsTrue(service.ReviewProduct(1, username, 2, "Blank", 6).ErrorOccured);
        }

        /*
            Response<List<Product>> SearchProduct(string user, int productId, string keyWords, string catagory, int minPrice, int maxPrice, int productReview);

            Response<Product> addToCart(string user, int productId, int storeId, int quantity);

            Response<ShoppingCart> viewCart(string user);

            Response<ShoppingCart> editCart(string user, int productId, int newQuantity);

            Response BuyCart(string user, string address);
         */

        public void AssertProductsEqual(Product prodA, Product prodB)
        {
            Assert.AreEqual(prodA.Id, prodB.Id);
            Assert.AreEqual(prodA.Name, prodB.Name);
            Assert.AreEqual(prodA.BasePrice, prodB.BasePrice);
            Assert.AreEqual(prodA.Description, prodB.Description);
        }

        public void AssertProductsNotEqual(Product prodA, Product prodB)
        {
            Assert.AreNotEqual(prodA.Id, prodB.Id);
            Assert.AreNotEqual(prodA.Name, prodB.Name);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestSearchProduct_Good_SpecificProduct(string username, string password)
        {
            Product prod = TestAddProduct_Good(username, password, product);
            Response<List<Product>> searchResult = service.SearchProduct(1, username, prod.Name, "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            AssertProductsEqual(prod, searchResult.Value.First());
        }

        [DataTestMethod]
        [DataRow(username, password, product)]
        public void TestSearchProduct_Good_SearchForEveryProduct(string username, string password, string product)
        {
            Product prod = TestAddProduct_Good(username, password, product);
            Response<List<Product>> searchResult = service.SearchProduct(1, username, "", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            Assert.IsTrue(searchResult.Value.Count() > 0);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestSearchProduct_Bad_NoProducts(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Response<List<Product>> searchResult = service.SearchProduct(1, username, "", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            Assert.AreEqual(searchResult.Value.Count(), 0);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestSearchProduct_Bad_WrongName(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<List<Product>> searchResult = service.SearchProduct(1, username, "Worong", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            if(searchResult.Value.Count > 0)
                AssertProductsNotEqual(prod, searchResult.Value.First());
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddToCart_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.addToCart(1, username, prod.Id, storeId, 1);
            Assert.IsFalse(resProd.ErrorOccured);
            Assert.IsNotNull(resProd.Value);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddToCart_Bad_NoSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.addToCart(1, username, 20, storeId, 1);
            Assert.IsTrue(resProd.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddToCart_Bad_NotEnoughQuantity(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.addToCart(1, username, prod.Id, storeId, 100);
            Assert.IsTrue(resProd.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddToCart_Bad_AddZero(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.addToCart(1, username, prod.Id, storeId, 0);
            Assert.IsTrue(resProd.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestViewCart_Good_EmptyCart(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<ShoppingCart> resSC = service.viewCart(1, username);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(resSC.Value.shoppingBags.Count, 0);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestViewCart_Good_FullCart(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            service.addToCart(1, username, prod.Id, storeId, 1);
            Response<ShoppingCart> resSC = service.viewCart(1, username);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(resSC.Value.shoppingBags.Count, 1);
            Assert.AreEqual(resSC.Value.shoppingBags[storeId].products.Count, 1);
            AssertProductsEqual(resSC.Value.shoppingBags[storeId].products.First(), prod);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestViewCart_Bad_NoUserLoggedIn(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            service.addToCart(1, username, prod.Id, storeId, 1);
            service.Logout(1, username);
            Response<ShoppingCart> resSC = service.viewCart(1, username);
            Assert.IsTrue(resSC.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestEditCart_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            service.addToCart(1, username, prod.Id, storeId, 1);
            Response<ShoppingCart> resSC = service.editCart(1, username, prod.Id, 5);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(resSC.Value.shoppingBags[storeId].products.First().Quantity, 5);
            resSC = service.editCart(1, username, prod.Id, 1);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(resSC.Value.shoppingBags[storeId].products.First().Quantity, 1);
        }


        [DataTestMethod]
        [DataRow(username, password)]
        public void TestBuyCart_Good_MoreThenEnoughInStock(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 2, "cat1").Value;
            service.addToCart(1, username, prod.Id, storeId, 1);
            Assert.IsFalse(service.BuyCart(1, username, "Ronmi's home").ErrorOccured);
            Assert.IsTrue(service.addToCart(1, username, prod.Id, storeId, 10).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestBuyCart_Good_LastOneInStock(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            service.addToCart(1, username, prod.Id, storeId, 1);
            Assert.IsFalse(service.BuyCart(1, username, "Ronmi's home").ErrorOccured);
            Assert.IsTrue(service.addToCart(1, username, prod.Id, storeId, 10).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestBuyCart_Bad_NothingInCart(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.BuyCart(1, username, "Ronmi's home").ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password)]
        public void TestRemoveProductFromStore_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.RemoveProductFromStore(1, username, storeId, prod.Id).ErrorOccured);
            Assert.IsTrue(service.addToCart(1, username, prod.Id, storeId, 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestRemoveProductFromStore_Bad(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.RemoveProductFromStore(1, username, storeId, 0).ErrorOccured);
        }



        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductName_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductName(1, username, storeId, prod.Id, "newName").ErrorOccured);
            prod = service.addToCart(1, username, prod.Id, storeId, 1).Value;
            Assert.AreEqual(prod.Name, "newName");
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductName_Bad_NoSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.ChangeProductName(1, username, storeId, 0, "newName").ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestChangeProductName_Bad_InvalidName(string username, string password, string newName)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductName(1, username, storeId, prod.Id, newName).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductPrice_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductPrice(1, username, storeId, prod.Id, 1711).ErrorOccured);
            prod = service.addToCart(1, username, prod.Id, storeId, 1).Value;
            Assert.AreEqual(prod.BasePrice, 1711);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductPrice_Bad_NoSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.ChangeProductPrice(1, username, storeId, 0, 1711).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, -1)]
        //[DataRow(username, password, 0)]
        public void TestChangeProductPrice_Bad_InvalidPrice(string username, string password, int price)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductPrice(1, username, storeId, prod.Id, price).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductQuantity_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductQuantity(1, username, storeId, prod.Id, 1711).ErrorOccured);
            Assert.IsFalse(service.addToCart(1, username, prod.Id, storeId, 1711).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductQuantity_Bad_NoSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.ChangeProductQuantity(1, username, storeId, 0, 1711).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, -1)]
        public void TestChangeProductQuantity_Bad_InvalidQuantity(string username, string password, int quantity)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductQuantity(1, username, storeId, prod.Id, quantity).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductCategory_Good(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductCategory(1, username, storeId, prod.Id, "newName").ErrorOccured);
            prod = service.addToCart(1, username, prod.Id, storeId, 1).Value;
            //Assert.AreEqual(prod.category, "newName");
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestChangeProductCategory_Bad_NoSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.ChangeProductCategory(1, username, storeId, 0, "newName").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestChangeProductCategory_Bad_InvalidCategory(string username, string password, string cat)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductCategory(1, username, storeId, prod.Id, cat).ErrorOccured);
        }

        public bool BuyProduct_Thread(int userId, string user, string password, int productId, int storeId, int quantity)
        {
            Assert.IsFalse(service.Login(userId, user, password).ErrorOccured);
            bool ret = service.addToCart(userId, user, productId, storeId, quantity).ErrorOccured;
            ret = ret & service.BuyCart(userId, user, "No Maidens?").ErrorOccured;
            return ret;
        }

        [TestMethod]
        public void TestBuyProductBad_BuylastAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            service.EnterMarket(1);
            service.EnterMarket(2);
            service.EnterMarket(3);
            Assert.IsFalse(service.Register(1, "buyer1", "1", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(2, "buyer2", "2", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(3, "Owner", "own", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Login(3, "Owner", "own").ErrorOccured);

            int storeId = service.CreateNewStore(3, "Owner", "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(3, "Owner", storeId, product, "Good", 1.0, 1, "cat1").Value;

            Thread thr1 = new Thread(() => res1 = BuyProduct_Thread(1, "buyer1", "1", prod.Id, storeId, 1));
            Thread thr2 = new Thread(() => res2 = BuyProduct_Thread(2, "buyer2", "2", prod.Id, storeId, 1));
            thr1.Start();
            thr2.Start();
            thr1.Join();
            thr2.Join();

            Assert.AreNotEqual(res1, res2);
        }

        // PurchaseTerm tests
        private Func<int, string> makeSimpleProductPurchaseTerm(string type, string action, string value)
        {
            Func<int, string> func = id => "{ tag: 'ProductPurchaseSimpleTerm', type: '" + type + "', action: '" + action + "', value: '" + value + "', productId: " + id + "}";
            return func;
        }

        private Func<string, string> makeSimpleCategoryPurchaseTerm(string type, string action, string value)
        {
            Func<string, string> func = category => "{ tag: 'CategoryPurchaseSimpleTerm', type: '" + type + "', action: '" + action + "', value: '" + value + "', category: '" + category + "'}";
            return func;
        }

        private string makeSimpleBagPurchaseTerm(string type, string action, string value)
        {
            return "{ tag: 'BagPurchaseSimpleTerm', type: '" + type + "', action: '" + action + "', value: '" + value + "'}";
        }

        private string makeSimpleUserPurchaseTerm(string action, int age)
        {
            return "{ tag: 'UserPurchaseSimpleTerm', action: '" + action + "', age: '" + age + "'}";
        }

        private string makeAndPurchaseTerm(string l_term, string r_term)
        {
            return "{ tag: 'PurchaseCompositeTerm', value: 'and', lhs: " + l_term + ", rhs: " + r_term + "}";
        }

        private string makeOrPurchaseTerm(string l_term, string r_term)
        {
            return "{ tag: 'PurchaseCompositeTerm', value: 'or', lhs: " + l_term + ", rhs: " + r_term + "}";
        }


        [DataTestMethod]
        [DataRow("member1", "p", ">", "10")]
        [DataRow("member1", "q", "<", "5")]
        [DataRow("member1", "h", "!=", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2023")]
        public void TestAddProductPurchaseTerm_Good_Simple(string member, string type, string action, string value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm(type, action, value)(product.Id), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "t", ">", "10")]
        [DataRow("member1", "t", ">", "-2")]
        [DataRow("member1", "q", "$", "5")]
        [DataRow("member1", "h", "!=", "25:00")]
        [DataRow("member1", "d", "!=", "27/08/2020")]
        public void TestAddProductPurchaseTerm_Bad_Simple_WrongParameters(string member, string type, string action, string value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsTrue(service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm(type, action, value)(product.Id), product.Id).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddProductPurchaseTerm_Good_And(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductPurchaseTerm(1, member, store.StoreId, (makeAndPurchaseTerm(makeSimpleProductPurchaseTerm(l_type, l_action, l_value)(product.Id), makeSimpleProductPurchaseTerm(r_type, r_action, r_value)(product.Id))), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddProductPurchaseTerm_Good_Or(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductPurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleProductPurchaseTerm(l_type, l_action, l_value)(product.Id), makeSimpleProductPurchaseTerm(r_type, r_action, r_value)(product.Id)), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10")]
        [DataRow("member1", "q", "<", "5")]
        [DataRow("member1", "h", "!=", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2023")]
        public void TestAddCategoryPurchaseTerm_Good_Simple(string member, string type, string action, string value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm(type, action, value)("Category1"), "Category1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "t", ">", "10")]
        [DataRow("member1", "q", "<", "-2")]
        [DataRow("member1", "h", "$", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2020")]
        public void TestAddCategoryPurchaseTerm_Bad_Simple_WrongParameters(string member, string type, string action, string value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsTrue(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm(type, action, value)("Category1"), "Category1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddCategoryPurchaseTerm_Good_And(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeAndPurchaseTerm(makeSimpleCategoryPurchaseTerm(l_type, l_action, l_value)("Category1"), makeSimpleCategoryPurchaseTerm(r_type, r_action, r_value)("Category1")), "Category1").ErrorOccured);
        }


        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddCategoryPurchaseTerm_Good_Or(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleCategoryPurchaseTerm(l_type, l_action, l_value)("Category1"), makeSimpleCategoryPurchaseTerm(r_type, r_action, r_value)("Category1")), "Category1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10")]
        [DataRow("member1", "q", "<", "5")]
        [DataRow("member1", "h", "!=", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2023")]
        public void TestAddStorePurchaseTerm_Good_Simple(string member, string type, string action, string value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsFalse(service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm(type, action, value)).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "t", ">", "10")]
        [DataRow("member1", "q", "<", "-2")]
        [DataRow("member1", "h", "$", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2020")]
        public void TestAddStorePurchaseTerm_Bad_WrongParameters(string member, string type, string action, string value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsTrue(service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm(type, action, value)).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddStorePurchaseTerm_Good_And(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsFalse(service.AddStorePurchaseTerm(1, member, store.StoreId, makeAndPurchaseTerm(makeSimpleBagPurchaseTerm(l_type, l_action, l_value), makeSimpleBagPurchaseTerm(r_type, r_action, r_value))).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddStorePurchaseTerm_Good_Or(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsFalse(service.AddStorePurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleBagPurchaseTerm(l_type, l_action, l_value), makeSimpleBagPurchaseTerm(r_type, r_action, r_value))).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "$", 10)]
        [DataRow("member1", ">=", -2)]
        public void TestAddUserPurchaseTerm_Good_Simple(string member, string action, int age)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsFalse(service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(action, age)).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", ">", 10)]
        [DataRow("member1", ">=", 13)]
        [DataRow("member1", "!=", 18)]
        public void TestAddUserPurchaseTerm_Bad_Simple_WrongParameters(string member, string action, int age)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsTrue(service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(action, age)).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow("member1", ">", 10, "!=", 18)]
        [DataRow("member1", ">=", 13, ">", 10)]
        [DataRow("member1", "!=", 18, ">=", 16)]
        public void Test_AddUserPurchaseTerm_Good_And(string member, string l_action, int l_age, string r_action, int r_age)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsFalse(service.AddUserPurchaseTerm(1, member, store.StoreId, makeAndPurchaseTerm(makeSimpleUserPurchaseTerm(l_action, l_age), makeSimpleUserPurchaseTerm(r_action, r_age))).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", ">", 10, "!=", 18)]
        [DataRow("member1", ">=", 13, ">", 10)]
        [DataRow("member1", "!=", 18, ">=", 16)]
        public void Test_AddUserPurchaseTerm_Good_Or(string member, string l_action, int l_age, string r_action, int r_age)
        {
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Assert.IsFalse(service.AddUserPurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleUserPurchaseTerm(l_action, l_age), makeSimpleUserPurchaseTerm(r_action, r_age))).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddProductPurchaseTerm_GoodTerm()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("p", ">", "200")(p1.Id), p1.Id);
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("q", "<", "4")(p1.Id), p1.Id);
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("d", "=", DateTime.Now.ToString("dd/MM/yyyy"))(p1.Id), p1.Id);
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("h", ">", "00:01")(p1.Id), p1.Id);
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            Assert.IsFalse(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 2)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        [DataRow("=", 1)]
        public void Test_AddProductPurchaseTerm_Bad_BadQuantity(string action, int n)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("q", action, "2")(p1.Id), p1.Id);
            service.addToCart(1, member, p1.Id, store.StoreId, n);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 2)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        [DataRow("=", 1)]
        public void Test_AddProductPurchaseTerm_Bad_BadPrice(string action, int n)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("p", action, "200")(p1.Id), p1.Id);
            service.addToCart(1, member, p1.Id, store.StoreId, n);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 4)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        public void Test_AddCategoryPurchaseTerm_Good(string action, int n)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Desc1", 100.0, 10, "Category1").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Desc2", 100.0, 10, "Category1").Value;
            service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm("q", action, "3")("Category1"), "Category1");
            service.addToCart(1, member, p1.Id, store.StoreId, n - 1);
            service.addToCart(1, member, p2.Id, store.StoreId, 1);
            Assert.IsFalse(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("=", 2)]
        [DataRow("<", 2)]
        [DataRow(">", 1)]
        public void Test_AddCategoryPurchaseTerm_Bad_BadQuantity_WithTwoProducts(string action, int n)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Desc1", 100.0, 10, "Category1").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Desc2", 100.0, 10, "Category1").Value;
            service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm("q", action, "3")("Category1"), "Category1");
            service.addToCart(1, member, p1.Id, store.StoreId, n);
            service.addToCart(1, member, p2.Id, store.StoreId, n);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 2)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        [DataRow("=", 1)]
        public void Test_AddCategoryPurchaseTerm_Bad_BadQuantity(string action, int n)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Desc1", 100.0, 10, "Category1").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Desc2", 100.0, 10, "Category1").Value;
            service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm("q", action, "2")("Category1"), "Category1");
            if (n != 1)
            {
                service.addToCart(1, member, p1.Id, store.StoreId, n - 1);
                service.addToCart(1, member, p2.Id, store.StoreId, 1);
            }
            else
            {
                service.addToCart(1, member, p1.Id, store.StoreId, 1);
            }
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddUserPurchaseTerm_Good()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(">", 18));
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            Assert.IsFalse(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddUserPurchaseTerm_Bad_BadAge()
        {
            string member = "member1";
            service.EnterMarket(1);
            service.Register(1, member, "password1", DateTime.Parse("Aug 30, 2016"));
            service.Login(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(">", 18));
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("h", ">", "00:01")]
        [DataRow("h", "<", "23:59")]
        [DataRow("d", "=", "NULL")]
        public void Test_AddStorePurchaseTerm_Good(string type, string action, string val)
        {
            if (val.Equals("NULL"))
            {
                val = DateTime.Now.ToString("dd/MM/yyyy");
            }
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm(type, action, val));
            service.addToCart(1, member, p1.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddStorePurchaseTerm_BadHour()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm("h", "=", "04:04"));
            service.addToCart(1, member, p1.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddStorePurchaseTerm_Bad_BadDate()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm("d", "=", "30/08/2030"));
            service.addToCart(1, member, p1.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(1, member, "TestAddress").ErrorOccured);
        }

        // Discount policy tests

        public Func<int, string> makeSimpleProductDiscount(double percent)
        {
            Func<int, string> func = id => "{\"tag\": \"SimpleDiscount\",\"priceAction\":" +
                "{\"tag\": \"ProductPriceActionSimple\",\"percentage\":" +
                percent.ToString() + ", \"productId\": " + id.ToString() + "}}";
            return func;
        }

        public Func<int, string> makeConditionalProductDiscount(double percent, int qunatity)
        {
            Func<int, string> func = id => "{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: " + percent.ToString() + ", productId: " + id + " }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '>', value: " + qunatity + ", productId: " + id.ToString() + "}}";
            return func;
        }

        public Func<string, string> makeSimpleCategoryDiscount(double percent)
        {
            Func<string, string> func = category => "{\"tag\": \"SimpleDiscount\",\"priceAction\":" +
                "{\"tag\": \"CategoryPriceActionSimple\",\"percentage\":" +
                percent.ToString() + ", \"category\": \"" + category + "\"}}";
            return func;
        }

        public string makeSimpleStoreDiscount(double percent)
        {
            return "{tag: 'SimpleDiscount', priceAction: { tag: 'StorePriceActionSimple', percentage: " + percent.ToString() + "}}";
        }

        public Func<int, string> makeAndproductDiscount(double lPercent, double rPercent)
        {
            Func<int, string> func = id => "{ \"tag\": \"AndDiscount\",\"lhs\": {\"tag\": \"SimpleDiscount\"," +
                                            "\"priceAction\": {\"tag\": \"ProductPriceActionSimple\"," +
                                            "\"percentage\": " + lPercent.ToString() + ", \"productId\": " + id.ToString() +
                                            "}},\"rhs\": {\"tag\": \"SimpleDiscount\", \"priceAction\": { " +
                                            "\"tag\": \"ProductPriceActionSimple\", \"percentage\": " + rPercent.ToString() +
                                            ",\"productId\": " + id.ToString() + "}}}";

            return func;
        }

        public Func<int, string> makeOrproductDiscount(double lPercent, double rPercent)
        {
            Func<int, string> func = id => "{ \"tag\": \"OrDiscount\",\"lhs\": {\"tag\": \"SimpleDiscount\"," +
                                            "\"priceAction\": {\"tag\": \"ProductPriceActionSimple\"," +
                                            "\"percentage\": " + lPercent.ToString() + ", \"productId\": " + id.ToString() +
                                            "}},\"rhs\": {\"tag\": \"SimpleDiscount\", \"priceAction\": { " +
                                            "\"tag\": \"ProductPriceActionSimple\", \"percentage\": " + rPercent.ToString() +
                                            ",\"productId\": " + id.ToString() + "}}}";

            return func;
        }

        public Func<int, string> makeXorproductDiscount(double lPercent, double rPercent)
        {
            Func<int, string> func = id => "{ \"tag\": \"XorDiscount\",\"lhs\": {\"tag\": \"SimpleDiscount\"," +
                                            "\"priceAction\": {\"tag\": \"ProductPriceActionSimple\"," +
                                            "\"percentage\": " + lPercent.ToString() + ", \"productId\": " + id.ToString() +
                                            "}},\"rhs\": {\"tag\": \"SimpleDiscount\", \"priceAction\": { " +
                                            "\"tag\": \"ProductPriceActionSimple\", \"percentage\": " + rPercent.ToString() +
                                            ",\"productId\": " + id.ToString() + "}}, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: 3, productId: 1}}";

            return func;
        }

        [DataTestMethod]
        [DataRow(30)]
        [DataRow(30.5)]
        [DataRow(0.5)]
        [DataRow(100)]
        public void Test_AddProductDiscount_Good_Simple(double percent)
        {
            Test_AddProductDiscount_Good(makeSimpleProductDiscount(percent));
        }

        [DataTestMethod]
        [DataRow(30, 30.5)]
        [DataRow(30.5, 30)]
        [DataRow(0.5, 100)]
        [DataRow(100, 0.5)]
        public void Test_AddProductDiscount_Good_And(double lPercent, double rPercent)
        {
            Test_AddProductDiscount_Good(makeAndproductDiscount(lPercent, rPercent));
        }

        [DataTestMethod]
        [DataRow(30, 30.5)]
        [DataRow(30.5, 30)]
        [DataRow(0.5, 100)]
        [DataRow(100, 0.5)]
        public void Test_AddProductDiscount_Good_Or(double lPercent, double rPercent)
        {
            Test_AddProductDiscount_Good(makeOrproductDiscount(lPercent, rPercent));
        }

        [DataTestMethod]
        [DataRow(30, 30.5)]
        [DataRow(30.5, 30)]
        [DataRow(0.5, 100)]
        [DataRow(100, 0.5)]
        public void Test_AddProductDiscount_Good_Xor(double lPercent, double rPercent)
        {
            Test_AddProductDiscount_Good(makeXorproductDiscount(lPercent, rPercent));
        }

        private void Test_AddProductDiscount_Good(Func<int, string> discount)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductDiscount(1, member, store.StoreId, discount(product.Id), product.Id).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddProductDiscount_Bad_NoSuchProduct()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(30)(product.Id + 1), product.Id + 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: -2, productId: 1}} ")]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: 0.5, productId: 1}} ")]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '$', value: 3, productId: 1}} ")]
        public void Test_AddProductDiscount_Bad_WrongParameters(string discount)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsFalse(product.Id != 1);
            Assert.IsTrue(service.AddProductDiscount(1, member, store.StoreId, discount, 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-1.5)]
        [DataRow(200)]
        public void Test_AddProductDiscount_Bad_BadPercentage_Simple(double percent)
        {
            Test_AddProductDiscount_Bad_BadDiscount(makeSimpleProductDiscount(percent));
        }

        public void Test_AddProductDiscount_Bad_BadDiscount(Func<int, string> discount)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsTrue(service.AddProductDiscount(1, member, store.StoreId, discount(product.Id), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: 5,category: 'cat1'}} ")]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'p',action: '<',value: 0.5,category: 'cat1'}} ")]
        public void Test_AddCategoryDiscount_Good(string discount)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "cat1");
            Assert.IsFalse(service.AddCategoryDiscount(1, member, store.StoreId, discount, "cat1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: -2,category: 'cat1'}} ")]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: 0.5,category: 'cat1'}} ")]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '$',value: 3,category: 'cat1'}} ")]
        public void TestAddCategoryDiscount_Bad_WrongParameters(string discount)
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "cat1");
            Assert.IsTrue(service.AddCategoryDiscount(1, member, store.StoreId, discount, "cat1").ErrorOccured);
        }

        [TestMethod]
        public void Test_BuyCart_ProductAndCategoryDiscounts()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddCategoryDiscount(1, member, store.StoreId, makeSimpleCategoryDiscount(50)("Cat1"), "Cat1");
            service.AddCategoryDiscount(1, member, store.StoreId, makeSimpleCategoryDiscount(10)("Cat2"), "Cat2");
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            service.addToCart(1, member, p2.Id, store.StoreId, 2);
            service.addToCart(1, member, p3.Id, store.StoreId, 5);
            double expected_price = 335;
            Assert.AreEqual(expected_price, service.BuyCart(1, member, "TestAddress").Value);
        }

        [TestMethod]
        public void Test_BuyCart_ProductAndStoreDiscounts()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddStoreDiscount(1, member, store.StoreId, makeSimpleStoreDiscount(10));
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            service.addToCart(1, member, p2.Id, store.StoreId, 2);
            service.addToCart(1, member, p3.Id, store.StoreId, 5);
            double expected_price = 205;
            Assert.AreEqual(expected_price, service.BuyCart(1, member, "TestAddress").Value);
        }

        [TestMethod]
        public void Test_BuyCart_ConditionalProductDiscounts()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeConditionalProductDiscount(10, 2)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeConditionalProductDiscount(20, 2)(p2.Id), p2.Id);
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            service.addToCart(1, member, p2.Id, store.StoreId, 2);
            service.addToCart(1, member, p3.Id, store.StoreId, 5);
            double expected_price = 30;
            Assert.AreEqual(expected_price, service.BuyCart(1, member, "TestAddress").Value);
        }

        [TestMethod]
        public void Test_BuyCart_CompositeProductDiscounts()
        {
            string member = "member1";
            TestLogin_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1").Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeAndproductDiscount(10, 20)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(10, 20)(p2.Id), p2.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(30, 40)(p3.Id), p3.Id);
            service.addToCart(1, member, p1.Id, store.StoreId, 3);
            service.addToCart(1, member, p2.Id, store.StoreId, 2);
            service.addToCart(1, member, p3.Id, store.StoreId, 5);
            double expected_price = 270;
            Assert.AreEqual(expected_price, service.BuyCart(1, member, "TestAddress").Value);
        }

        // NEW FOR VERSION 3

        [TestMethod]
        public void Test_Scenario1()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            string member3 = "Member3";
            TestLogin_Good(1, member1, "Password1");
            TestLogin_Good(2, member2, "Password2");
            TestLogin_Good(3, member3, "Password3");
            Store store = service.CreateNewStore(1, member1, "Store1").Value;
            service.NominateStoreOwner(1, member1, member2, store.StoreId);
            Product p1 = service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 100.0, 5, "Category1").Value;
            Product p2 = service.AddProduct(1, member1, store.StoreId, "Product2", "Description2", 10.0, 5, "Category2").Value;
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(10)("Category1"), "Category1");
            service.AddProductDiscount(2, member2, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.addToCart(3, member3, p1.Id, store.StoreId, 2); // 200 before discount, 180 after
            service.addToCart(3, member3, p2.Id, store.StoreId, 3); // 30 before discount, 24 after
            Assert.AreEqual(204, service.BuyCart(3, member3, "TestAddress").Value);
            string review_string = "Good product, that is!";
            ReviewDTO rev = service.ReviewProduct(3, member3, p1.Id, review_string, 4).Value;
            Assert.AreEqual(review_string, rev.Review);
            Assert.AreEqual(4, rev.Rating);
            Assert.AreEqual(member3, rev.Reviewer);
            Assert.AreEqual(p1.Id, rev.ProductId);
        }

        [TestMethod]
        public void Test_Scenario2()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            string member3 = "Member3";
            TestLogin_Good(1, member1, "Password1");
            TestLogin_Good(2, member2, "Password2");
            TestLogin_Good(3, member3, "Password3");
            Store store = service.CreateNewStore(1, member1, "Store1").Value;
            service.NominateStoreOwner(1, member1, member2, store.StoreId);
            service.NominateStoreOwner(2, member2, member3, store.StoreId);
            Product p1 = service.AddProduct(3, member3, store.StoreId, "Product1", "Description1", 100.0, 5, "Category1").Value;
            Product p2 = service.AddProduct(2, member2, store.StoreId, "Product2", "Description2", 10.0, 5, "Category2").Value;
            service.RemoveStoreOwnerNomination(1, member1, member2, store.StoreId);
            Assert.IsTrue(service.RemoveProductFromStore(3, member3, store.StoreId, p2.Id).ErrorOccured);
            Assert.IsTrue(service.RemoveProductFromStore(2, member2, store.StoreId, p2.Id).ErrorOccured);
            Assert.IsFalse(service.RemoveProductFromStore(1, member1, store.StoreId, p2.Id).ErrorOccured);
            service.addToCart(3, member3, p1.Id, store.StoreId, 2);
            Assert.AreEqual(200, service.BuyCart(3, member3, "TestAddress").Value);
            string review_string = "Good product, that is!";
            ReviewDTO rev = service.ReviewProduct(3, member3, p1.Id, review_string, 4).Value;
            Assert.AreEqual(review_string, rev.Review);
            Assert.AreEqual(4, rev.Rating);
            Assert.AreEqual(member3, rev.Reviewer);
            Assert.AreEqual(p1.Id, rev.ProductId);
            Assert.IsTrue(service.ReviewProduct(2, member2, p1.Id, review_string, 4).ErrorOccured);
        }

        [TestMethod]
        public void Test_HoldedNotifications_NoHoldedNotifications()
        {
            TestRegister_Good(1, "Member2", "Password2");
            TestRegister_Good(2, "Member3", "Password3");
            TestRegister_Good(0, "Member1", "Password1");
            Response<KeyValuePair<Member, List<Notification>>> response1 = service.Login(0, "Member1", "Password1");

            Assert.AreEqual(response1.Value.Value.Count, 0);
            Response<Store> resStore = service.CreateNewStore(0, "Member1", "Store1");

            Assert.IsFalse(resStore.ErrorOccured);
            Store store1 = resStore.Value;

            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response2 = service.Login(1, "Member2", "Password2");

            Assert.IsFalse(response2.ErrorOccured);
            Assert.IsNotNull(response2.Value);
            Assert.IsNotNull(response2.Value.Value);
            Assert.AreEqual(response2.Value.Value.Count, 0);

            Response<KeyValuePair<Member, List<Notification>>> response3 = service.Login(2, "Member3", "Password3");

            Assert.IsFalse(response3.ErrorOccured);
            Assert.IsNotNull(response3.Value);
            Assert.IsNotNull(response3.Value.Value);
            Assert.AreEqual(response3.Value.Value.Count, 0);
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_CloseStore()
        {
            TestRegister_Good(1, "Member2", "Password2");
            TestRegister_Good(2, "Member3", "Password3");
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resStore = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resStore.ErrorOccured);
            Store store1 = resStore.Value;

            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId);

            service.Login(1, "Member2", "Password2");
            service.Login(2, "Member3", "Password3");  
            service.Logout(1, "Member2");
            service.Logout(2, "Member3");
            service.CloseStore(0, "Member1", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2");

            Assert.IsFalse(response4.ErrorOccured);
            Assert.AreEqual(response4.Value.Value.Count, 1);
            Assert.AreEqual(response4.Value.Value[0].Sender, "Member1");
            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3");

            Assert.IsFalse(response5.ErrorOccured);
            Assert.AreEqual(response5.Value.Value.Count, 1);
            Assert.AreEqual(response5.Value.Value[0].Sender, "Member1");
        }

        [TestMethod]
        public void Test_CloseStore_Good()
        {
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse (resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_CloseStore_Bad_NoPermission()
        {
            TestLogin_Good(0, "Member1", "Password1");
            TestLogin_Good(1, "Member2", "Password2");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(1, "Member2", store1.StoreId);
            Assert.IsTrue(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_CloseStore_Bad_NoSuchStore()
        {
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId + 1);
            Assert.IsTrue(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_CloseStore_Bad_AlreadyClosed()
        {
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp2.ErrorOccured);
            Response resp3 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsTrue(resp3.ErrorOccured);
        }

        [TestMethod]
        public void Test_OpenStore_Good()
        {
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp2.ErrorOccured);
            Response resp3 = service.OpenStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp3.ErrorOccured);
        }

        [TestMethod]
        public void Test_OpenStore_Bad_NoPermission()
        {
            TestLogin_Good(0, "Member1", "Password1");
            TestLogin_Good(1, "Member2", "Password2");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp2.ErrorOccured);
            Response resp3 = service.OpenStore(1, "Member2", store1.StoreId);
            Assert.IsTrue(resp3.ErrorOccured);
        }

        [TestMethod]
        public void Test_OpenStore_Bad_NoSuchStore()
        {
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp2.ErrorOccured);
            Response resp3 = service.OpenStore(0, "Member1", store1.StoreId + 1);
            Assert.IsTrue(resp3.ErrorOccured);
        }

        [TestMethod]
        public void Test_OpenStore_Bad_AlreadyOpen()
        {
            TestLogin_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1");
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.OpenStore(0, "Member1", store1.StoreId);
            Assert.IsTrue(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_OpenStore()
        {
            // Store owners need to get a notification when their store opens again
            TestRegister_Good(1, "Member2", "Password2");
            TestRegister_Good(2, "Member3", "Password3");
            TestLogin_Good(0, "Member1", "Password1");
            Store store1 = service.CreateNewStore(0, "Member1", "Store1").Value;

            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId);

            service.CloseStore(0, "Member1", store1.StoreId);

            service.Login(1, "Member2", "Password2");
            service.Login(2, "Member3", "Password3");
            service.Logout(1, "Member2");
            service.Logout(2, "Member3");

            service.OpenStore(0, "Member1", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2");
            Assert.IsFalse(response4.ErrorOccured);
            Assert.AreEqual(response4.Value.Value.Count, 1);
            Assert.AreEqual(response4.Value.Value[0].Sender, "Member1");

            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3");
            Assert.IsFalse(response5.ErrorOccured);
            Assert.AreEqual(response5.Value.Value.Count, 1);
            Assert.AreEqual(response5.Value.Value[0].Sender, "Member1");
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_BuyingProductFromYourStore()
        {
            // Store owners need to get a notification when someone buys from them
            TestLogin_Good(0, "Member1", "Password1");
            TestRegister_Good(1, "Member2", "Password2");
            TestRegister_Good(2, "Member3", "Password3");
            TestRegister_Good(3, "Member4", "Password4");

            Store store1 = service.CreateNewStore(0, "Member1", "Store1").Value;
            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId);
            Product product1 = service.AddProduct(0, "Member1", store1.StoreId, "ProductName1", "Description1", 2.7, 10, "Category1").Value;
            service.Logout(0, "Member1");
            service.Login(3, "Member4", "Password4");
            service.addToCart(3, "Member4", product1.Id, store1.StoreId, 1);
            service.BuyCart(3, "Member4", "My Home!");
            Response<KeyValuePair<Member, List<Notification>>> response6 = service.Login(0, "Member1", "Password1");
            Assert.AreEqual(response6.Value.Value.Count, 1);
            Assert.AreEqual(response6.Value.Value[0].Sender, "Member4");
            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2");
            Assert.AreEqual(response4.Value.Value.Count, 1);
            Assert.AreEqual(response4.Value.Value[0].Sender, "Member4");
            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3");
            Assert.AreEqual(response5.Value.Value.Count, 1);
            Assert.AreEqual(response5.Value.Value[0].Sender, "Member4");
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_RemovingNomination()
        {
            // Store owners and managers need to get a notification when their nomination is removed
            TestRegister_Good(1, "Member2", "Password2");
            TestRegister_Good(2, "Member3", "Password3");
            TestLogin_Good(0, "Member1", "Password1");
            TestLogin_Good(3, "Member4", "Password4");
            Store store1 = service.CreateNewStore(0, "Member1", "Store1").Value;
            service.NominateStoreOwner(0, "Member1", "Member4", store1.StoreId);
            service.NominateStoreOwner(3, "Member4", "Member2", store1.StoreId);
            service.Logout(3, "Member4");
            service.NominateStoreManager(3, "Member4", "Member3", store1.StoreId);
            service.RemoveStoreOwnerNomination(0, "Member1", "Member4", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response3 = service.Login(3, "Member4", "Password4");
            Assert.AreEqual(response3.Value.Value.Count, 1);
            Assert.AreEqual(response3.Value.Value[0].Sender, "Member1");

            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2");
            Assert.AreEqual(response4.Value.Value.Count, 1);
            Assert.AreEqual(response4.Value.Value[0].Sender, "Member1");

            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3");
            Assert.AreEqual(response5.Value.Value.Count, 1);
            Assert.AreEqual(response5.Value.Value[0].Sender, "Member1");
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_GettingMessaged()
        {
            // MAKE SURE IT IS IMPLEMENTED ONCE MESSAGES ARE ADDED
            throw new NotImplementedException("Test_HoldedNotifications_From_GettingMessaged");
        }

        [TestMethod]
        public void Test_InitializeFromFile_Good()
        {
            Service service = new Service("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1,store1)");
            Assert.IsTrue(service.WasInitializedWithFile);
            Assert.IsTrue(service.EnterMarket(1).ErrorOccured);
            Assert.IsTrue(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsTrue(service.Login(1, "user1", "pass1").ErrorOccured);
            Assert.IsTrue(service.GetAllStores(1).Value.Count == 1);
            Assert.IsTrue(service.GetAllStores(1).Value[0].Name.Equals("store1"));
        }

        [TestMethod]
        public void Test_InitializeFromFile_Consistent()
        {
            // Sanity check
            Service service = new Service();
            Assert.IsFalse(service.WasInitializedWithFile);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_IllegalOrderOfActions()
        {
            Service service = new Service("enter-market(1)\n" +
                "login(1,user1,pass1\n" +
                "create-new-store(1,user1,store1)");
            // Expecting error: need to register before logging in
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1).ErrorOccured); // Make sure nothing happend after it failed
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("Aug 22, 1972")).ErrorOccured); // Make sure nothing happend after it failed
            Assert.IsFalse(service.Login(1, "user1", "pass1").ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_UnrecognizedCommand()
        {
            Service service = new Service("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1,store1)\n" +
                "blah-blah(1,user1,pass1,store1)");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1").ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_BadArgumentsForCommand_Overshoot()
        {
            Service service = new Service("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1,store1,failureInLife1)");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1").ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_BadArgumentsForCommand_Undershoot()
        {
            Service service = new Service("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1)");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1").ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_BadArgumentsForCommand_WrongType()
        {
            Service service = new Service("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(FAILME,user1,store1)");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1").ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_AddActionToManager_Good()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            TestLogin_Good(1, member1, "password1");
            TestLogin_Good(2, member2, "password2");
            Store store = service.CreateNewStore(1, member1, "Store1").Value;
            service.NominateStoreManager(1, member1, member2, store.StoreId);
            Assert.IsTrue(service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
            Assert.IsFalse(service.AddActionToManager(1, member1, member2, store.StoreId, "AddProduct").ErrorOccured);
            Assert.IsFalse(service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddActionToManager_Bad_NoSuchAction()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            TestLogin_Good(1, member1, "password1");
            TestLogin_Good(2, member2, "password2");
            Store store = service.CreateNewStore(1, member1, "Store1").Value;
            service.NominateStoreManager(1, member1, member2, store.StoreId);
            Assert.IsTrue(service.AddActionToManager(1, member1, member2, store.StoreId, "BlahBlah").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddActionToManager_Bad_DoesntWorkOnNotManager()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            TestLogin_Good(1, member1, "password1");
            TestLogin_Good(2, member2, "password2");
            Store store = service.CreateNewStore(1, member1, "Store1").Value;
            Assert.IsTrue(service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
            Assert.IsTrue(service.AddActionToManager(1, member1, member2, store.StoreId, "AddProduct").ErrorOccured);
            Assert.IsTrue(service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddActionToManager_Bad_DoesntWorkFromNotOwner()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            string member3 = "Member3";
            TestLogin_Good(1, member1, "password1");
            TestLogin_Good(2, member2, "password2");
            TestLogin_Good(3, member3, "password3");
            Store store = service.CreateNewStore(1, member1, "Store1").Value;
            service.NominateStoreManager(1, member1, member2, store.StoreId);
            service.NominateStoreManager(1, member1, member3, store.StoreId);
            Assert.IsTrue(service.AddProduct(3, member3, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
            Assert.IsTrue(service.AddActionToManager(2, member2, member3, store.StoreId, "AddProduct").ErrorOccured);
            Assert.IsTrue(service.AddProduct(3, member3, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
        }
    }
}
