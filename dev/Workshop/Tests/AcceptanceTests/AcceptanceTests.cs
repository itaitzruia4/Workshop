using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.IsFalse(service.Register(userId, username, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        [DataRow("TestRegister_Bad", password)]
        [DataRow(username, "TestRegister_Bad")]
        public void TestRegister_Bad(string username, string password)
        {
            service.Register(1, username, password);
            Assert.IsTrue(service.Register(1, username, password).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(1, username, password)]
        public void TestLogin_Good(int userId, string username, string password)
        {
            service.EnterMarket(userId);
            service.Register(userId, username, password);
            Response<Member> rMember = service.Login(userId, username, password);
            Assert.IsFalse(rMember.ErrorOccured);
            Assert.IsNotNull(rMember.Value);
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
            service.Register(1, username, password);
            Assert.IsTrue(service.Login(1, username, wrongPassword).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, password)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestLogin_Bad_WrongUsername(string username, string password, string wrongUsername)
        {
            service.Register(1, username, password);
            Assert.IsTrue(service.Login(1, wrongUsername, password).ErrorOccured);
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
            Response<Product> prodResult = service.AddProduct(1, username, storeId, 0, product, "Good", 1.0, 1, "cat1");
            Assert.IsFalse(prodResult.ErrorOccured);
            Assert.IsInstanceOfType(prodResult.Value, typeof(Product));
            return prodResult.Value;
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddProduct_Bad_AddTwice(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.AddProduct(1, username, storeId, 0, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
            Assert.IsTrue(service.AddProduct(1, username, storeId, 0, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
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
            Assert.IsFalse(service.EnterMarket(userId).ErrorOccured);
            Assert.IsFalse(service.Login(userId, nominator, password).ErrorOccured);

            return service.NominateStoreManager(userId, nominator, nominated, storeId).ErrorOccured;
        }


        [TestMethod]
        public void TestNominateStoreManager_BadNominateTwiceAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            service.EnterMarket(1);
            Assert.IsFalse(service.Register(1, "Nominated", "none").ErrorOccured);
            Assert.IsFalse(service.Register(2, "Nominator1", "1").ErrorOccured);
            Assert.IsFalse(service.Register(3, "Nominator2", "2").ErrorOccured);
            Assert.IsFalse(service.Register(4, "Owner", "own").ErrorOccured);
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
            int prodId = service.AddProduct(1, username, storeId, 0, "TestReviewProduct", "Good", 1, 2, "cat1").Value.Id;
            service.addToCart(1, username, prodId, storeId, 1);
            service.BuyCart(1, username, "Ronmi's home");
            Assert.IsFalse(service.ReviewProduct(1, username, 0, "Blank").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_userLoggeedOut(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, 0, "TestReviewProduct", "Good", 1, 1, "cat1");
            service.Logout(1, username);
            Assert.IsTrue(service.ReviewProduct(1, username, 0, "Blank").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_noSuchProduct(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, 0, "TestReviewProduct", "Good", 1, 1, "cat1");
            Assert.IsTrue(service.ReviewProduct(1, username, 2, "Blank").ErrorOccured);
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
            Assert.Equals(prodA.Id, prodB.Id);
            Assert.Equals(prodA.Name, prodB.Name);
            Assert.Equals(prodA.BasePrice, prodB.BasePrice);
            Assert.Equals(prodA.Description, prodB.Description);
            Assert.Equals(prodA.Quantity, prodB.Quantity);
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
            Response<List<Product>> searchResult = service.SearchProduct(1, username, prod.Id, prod.Name, "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            AssertProductsEqual(prod, searchResult.Value.First());
        }

        [DataTestMethod]
        [DataRow(username, password, product)]
        public void TestSearchProduct_Good_SearchForEveryProduct(string username, string password, string product)
        {
            Product prod = TestAddProduct_Good(username, password, product);
            Response<List<Product>> searchResult = service.SearchProduct(1, username, -1, "", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            Assert.IsTrue(searchResult.Value.Count() > 0);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestSearchProduct_Bad_NoProducts(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Response<List<Product>> searchResult = service.SearchProduct(1, username, -1, "", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            Assert.AreEqual(searchResult.Value.Count(), 0);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestSearchProduct_Bad_WrongName(string username, string password)
        {
            TestLogin_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, 0, product, "Good", 1.0, 1, "cat1").Value;
            Response<List<Product>> searchResult = service.SearchProduct(1, username, -1, "Worong", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            if(searchResult.Value.Count > 0)
                AssertProductsNotEqual(prod, searchResult.Value.First());
        }


        public void TestBuyProductBad_BuylastAtTheSameTime()
        {
            Assert.Fail();
        }

        public void TestBuyProductBad_BuyAndDeleteAtTheSameTime()
        {
            Assert.Fail();
        }

    }
}
