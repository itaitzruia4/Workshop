using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Workshop.DomainLayer.Reviews;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using SupplyAddress = Workshop.DomainLayer.MarketPackage.SupplyAddress;
using CreditCard = Workshop.DomainLayer.MarketPackage.CreditCard;
using Moq;
using System.Threading;

namespace Tests.AcceptanceTests
{
    [TestClass]
    public class AcceptanceTests
    {
        IService service;
        private const string username = "Goodun";
        private const string password = "Goodp";
        private const string product = "Product";
        private SupplyAddress address;
        private CreditCard cc;
        private Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();

        [TestInitialize]
        public void InitSystem()
        {
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Random().Next(10000, 100000));
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Random().Next(10000, 100000));
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Returns(true);
            Workshop.DataLayer.DataHandler.Instance.Value.clear();
            address = new SupplyAddress("Ronmi", "Mayor 1", "Ashkelon", "Israel", "784112");
            cc = new CreditCard("001122334455667788", "11", "26", "LeBron Michal", "555", "208143751");
            service = new Service(externalSystem.Object);
        }

        [TestCleanup]
        public void TestCleanup() 
        {
            Workshop.DataLayer.DataHandler.Instance.Value.clear();
        }

        [TestMethod]
        public void Test_SystemInitiation()
        {
            Assert.IsNotNull(service);
            //Assert.IsNotNull(srv.getSystemmanager()); //Further tests will come later on
            //Assert.IsNotNull(srv.getExternalConnections());
        }

        [TestMethod]
        public void Test_EnterMarket_Good()
        {
            Response<User> ru = service.EnterMarket(1, DateTime.Now);
            Assert.IsFalse(ru.ErrorOccured);
            Assert.IsNotNull(ru.Value);
        }

        [TestMethod]
        public void Test_EnterMarket_Bad()
        {
            Assert.IsFalse(service.EnterMarket(1, DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.EnterMarket(1, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_ExitMarket_Good()
        {
            service.EnterMarket(1, DateTime.Now);
            Assert.IsFalse(service.ExitMarket(1).ErrorOccured);
        }

        [TestMethod]
        public void Test_ExitMarket_Bad_NeverEntered()
        {
            Assert.IsTrue(service.ExitMarket(1).ErrorOccured);
        }

        [TestMethod]
        public void Test_ExitMarket_Bad_ExitTwice()
        {
            service.EnterMarket(1, DateTime.Now);
            Assert.IsFalse(service.ExitMarket(1).ErrorOccured);
            Assert.IsTrue(service.ExitMarket(1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(1, username, password)]
        public void Test_Register_Good(int userId, string username, string password)
        {
            service.EnterMarket(userId, DateTime.Now);
            Assert.IsFalse(service.Register(userId, username, password, DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            //Thread.Sleep(60000);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        [DataRow("TestRegister_Bad", password)]
        [DataRow(username, "TestRegister_Bad")]
        public void Test_Register_Bad(string username, string password)
        {
            service.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.Register(1, username, password, DateTime.Parse("Aug 22, 1972")).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(1, username, password)]
        public void Test_Login_Good(int userId, string username, string password)
        {
            service.EnterMarket(userId, DateTime.Now);
            service.Register(userId, username, password, DateTime.Parse("Aug 22, 1972"));
            Response<KeyValuePair<Member, List<Notification>>> rMember = service.Login(userId, username, password, DateTime.Now);
            Assert.IsFalse(rMember.ErrorOccured);
            Assert.IsNotNull(rMember.Value.Value);
            Assert.IsNotNull(rMember.Value.Key);
        }

        [DataTestMethod]
        [DataRow("Fake1", "Fake2")]
        [DataRow("", "")]
        [DataRow("", "Fake2")]
        [DataRow("Fake1", "")]
        public void Test_Login_Bad_NoSuchUser(string username, string password)
        {
            service.EnterMarket(1, DateTime.Now);
            Assert.IsTrue(service.Login(1, username, password, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, username)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void Test_Login_Bad_WrongPassword(string username, string password, string wrongPassword)
        {
            service.EnterMarket(1, DateTime.Now);
            service.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.Login(1, username, wrongPassword, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, password)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void Test_Login_Bad_WrongUsername(string username, string password, string wrongUsername)
        {
            service.EnterMarket(1, DateTime.Now);
            service.Register(1, username, password, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.Login(1, wrongUsername, password, DateTime.Now).ErrorOccured);
        }

        public void Test_Login_Bad_LoggedInFromAnotherUser()
        {
            Test_Login_Good(1, "Member1", "Pass1");
            service.EnterMarket(2, DateTime.Now);
            Assert.AreEqual("Member Member1 is already logged in from another user", service.Login(2, "Member1", "Pass1", DateTime.Now).ErrorMessage);
        }

        /*public bool Login_Thread(int userId, string username, string password)
        {
            return service.Login(userId, username, password, DateTime.Now).ErrorOccured;
        }
            return service.Login(userId, username, password).ErrorOccured;
        }*/

        /*[TestMethod]
        public void Test_Login_Bad_LoginTwiceAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            service.EnterMarket(1, DateTime.Now);
            Assert.IsFalse(service.Register(1, "user1", "1", new DateTime(2001, 11, 17)).ErrorOccured);

            Thread thr1 = new Thread(() => res1 = Login_Thread(1, "user1", "1"));
            Thread thr2 = new Thread(() => res2 = Login_Thread(1, "user1", "1"));
            thr1.Start();
            thr2.Start();
            thr1.Join();
            thr2.Join();

            Assert.AreNotEqual(res1, res2);
        }*/

        [DataTestMethod]
        [DataRow(1, username, password)]
        public void Test_Logout_Good(int userId, string username, string password)
        {
            Test_Login_Good(userId, username, password);
            Assert.IsFalse(service.Logout(userId, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_Logout_Bad_LogoutTwice(string username, string password)
        {
            Test_Login_Good(1, username, password);
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            Assert.IsTrue(service.Logout(1, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username)]
        [DataRow(null)]
        [DataRow("")]
        public void Test_Logout_Bad_NoSuchUser(string username)
        {
            Assert.IsTrue(service.Logout(1, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_Logout_Bad_NotLoggedIn(string username, string password)
        {
            Test_Register_Good(1, username, password);
            Assert.IsTrue(service.Logout(1, null).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, product)]
        public Product Test_AddProduct_Good(string username, string password, string product)
        {
            int storeId = 0;
            Test_Login_Good(1, username, password);
            storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Response<Product> prodResult = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1");
            Assert.IsFalse(prodResult.ErrorOccured);
            Assert.IsInstanceOfType(prodResult.Value, typeof(Product));
            return prodResult.Value;
        }

        [DataTestMethod]
        [DataRow(username, password, "no", "perm")]
        public void Test_AddProduct_Bad_NoPermission(string username, string password, string noPermU, string noPermP)
        {
            Test_Login_Good(1, username, password);
            Test_Login_Good(2, noPermU, noPermP);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.AddProduct(2, noPermU, storeId, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
            Assert.IsTrue(service.AddProduct(2, username, storeId, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
            Assert.IsTrue(service.AddProduct(1, noPermU, storeId, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int Test_NominateStoreOwner_Good(string username, string password, string nominated)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Test_Register_Good(2, nominated, nominated);
            Assert.IsFalse(service.NominateStoreOwner(1, username, nominated, storeId, DateTime.Now).ErrorOccured);
            return storeId;
        }

        [TestMethod]
        public void Test_NominateStoreOwner_FullVoteNeeded()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "mem2", "pass2");
            Test_Login_Good(3, "mem3", "pass3");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Response<StoreOwner> resp1 = service.NominateStoreOwner(1, "mem1", "mem2", st.StoreId, DateTime.Now);
            Assert.IsFalse(resp1.ErrorOccured);
            Assert.IsNotNull(resp1.Value);

            Response<StoreOwner> resp2 = service.NominateStoreOwner(1, "mem1", "mem3", st.StoreId, DateTime.Now);
            Assert.IsFalse(resp2.ErrorOccured);
            Assert.IsNull(resp2.Value);

            Response<StoreOwner> resp3 = service.NominateStoreOwner(2, "mem2", "mem3", st.StoreId, DateTime.Now);
            Assert.IsFalse(resp3.ErrorOccured);
            Assert.IsNotNull(resp3.Value);
        }

        [TestMethod]
        public void Test_NominateStoreOwner_Bad_NominateTwice()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "mem2", "pass2");
            Test_Login_Good(3, "mem3", "pass3");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Response<StoreOwner> resp1 = service.NominateStoreOwner(1, "mem1", "mem2", st.StoreId, DateTime.Now);
            Assert.IsFalse(resp1.ErrorOccured);
            Assert.IsNotNull(resp1.Value);

            Assert.IsTrue(service.NominateStoreOwner(1, "mem1", "mem2", st.StoreId, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_NominateStoreOwner_Bad_NominateUsrself(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.NominateStoreOwner(1, username, username, storeId, DateTime.Now).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void Test_NominateStoreOwner_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Test_Register_Good(2, nominated, nominated);
            Test_Register_Good(3, nominator, nominator);
            Assert.IsTrue(service.NominateStoreOwner(3, nominator, nominated, storeId, DateTime.Now).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int Test_NominateStoreManager_Good(string username, string password, string nominated)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Test_Register_Good(2, nominated, nominated);
            Assert.IsFalse(service.NominateStoreManager(1, username, nominated, storeId, DateTime.Now).ErrorOccured);
            return storeId;
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public void Test_NominateStoreManager_Bad_NominateTwice(string username, string password, string nominated)
        {
            int storeId = Test_NominateStoreManager_Good(username, password, nominated);
            Assert.IsTrue(service.NominateStoreManager(1, username, nominated, storeId, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_NominateStoreManager_Bad_NominateUsrself(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.NominateStoreManager(1, username, username, storeId, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void Test_NominateStoreManager_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            Test_Register_Good(2, nominated, nominated);
            Test_Login_Good(3, nominator, nominator);
            Assert.IsTrue(service.NominateStoreManager(3, nominator, nominated, storeId, DateTime.Now).ErrorOccured);
        }

        public bool NominateStoreManager_Thread(int userId, string nominator, string password, string nominated, int storeId)
        {
            //Assert.IsFalse(service.EnterMarket(userId).ErrorOccured);
            Assert.IsFalse(service.Login(userId, nominator, password, DateTime.Now).ErrorOccured);

            return service.NominateStoreManager(userId, nominator, nominated, storeId, DateTime.Now).ErrorOccured;
        }

        [TestMethod]
        public void Test_NominateStoreManager_BadNominateTwiceAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            Test_Login_Good(4, "Owner", "own");
            Test_Login_Good(2, "Nominator1", "1");
            Test_Register_Good(1, "Nominated", "none");
            Test_Register_Good(3, "Nominator2", "2");

            int storeId = service.CreateNewStore(4, "Owner", "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsFalse(service.NominateStoreOwner(4, "Owner", "Nominator1", storeId, DateTime.Now).ErrorOccured);
            Response<StoreOwner> resp = service.NominateStoreOwner(2, "Nominator1", "Nominator2", storeId, DateTime.Now);
            Assert.IsFalse(resp.ErrorOccured);
            Assert.IsNull(resp.Value);
            Assert.IsFalse(service.NominateStoreOwner(4, "Owner", "Nominator2", storeId, DateTime.Now).ErrorOccured);

            service.Logout(2, "Nominator1");

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
        public void Test_GetWorkersInformation_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsFalse(service.GetWorkersInformation(1, username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void Test_GetWorkersInformation_Bad(string username, string password, string npUser)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            Test_Login_Good(2, npUser, npUser);
            Assert.IsTrue(service.GetWorkersInformation(2, npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_CloseStore_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsFalse(service.CloseStore(1, username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void Test_CloseStore_Bad(string username, string password, string npUser)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, username, DateTime.Now).Value.StoreId;
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
            Test_Login_Good(2, npUser, npUser);
            Assert.IsTrue(service.CloseStore(2, npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public int Test_CreateNewStore_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            Response<Store> res = service.CreateNewStore(1, username, "RandomStore", DateTime.Now);
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
        public void Test_CreateNewStore_Bad(string username, string password)
        {
            Assert.IsTrue(service.CreateNewStore(1, username, password, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ReviewProduct_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            int prodId = service.AddProduct(1, username, storeId, "TestReviewProduct", "Good", 1, 2, "cat1").Value.Id;
            service.AddToCart(1, prodId, storeId, 1);
            service.BuyCart(1, cc, address, DateTime.Now);
            Assert.IsFalse(service.ReviewProduct(1, username, prodId, "Blank", 6).ErrorOccured);
        }

        [TestMethod]
        public void Test_ReviewProduct_SendsNotifications()
        {
            Test_Login_Good(1, "member1", "pass");
            Test_Login_Good(2, "member2", "pass");
            int storeId = service.CreateNewStore(1, "member1", "RandomStore", DateTime.Now).Value.StoreId;
            int prodId = service.AddProduct(1, "member1", storeId, "TestReviewProduct", "Good", 1, 2, "cat1").Value.Id;
            service.AddToCart(2, prodId, storeId, 1);
            service.BuyCart(2, cc, address, DateTime.Now);
            service.TakeNotifications(1, "member1");
            service.TakeNotifications(2, "member2");
            Assert.IsFalse(service.ReviewProduct(2, "member2", prodId, "Blank", 6).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(1, "member1").Value.Count);
            Assert.AreEqual(0, service.TakeNotifications(2, "member2").Value.Count);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ReviewProduct_Bad_userLoggeedOut(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            service.AddProduct(1, password, storeId, "TestReviewProduct", "Good", 1, 1, "cat1");
            service.Logout(1, username);
            Assert.IsTrue(service.ReviewProduct(1, username, 0, "Blank", 6).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ReviewProduct_Bad_noSuchProduct(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            service.AddProduct(1, password, storeId, "TestReviewProduct", "Good", 1, 1, "cat1");
            Assert.IsTrue(service.ReviewProduct(1, username, 2, "Blank", 6).ErrorOccured);
        }

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
        public void Test_SearchProduct_Good_SpecificProduct(string username, string password)
        {
            Product prod = Test_AddProduct_Good(username, password, product);
            Response<List<Product>> searchResult = service.SearchProduct(1, prod.Name, "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            AssertProductsEqual(prod, searchResult.Value.First());
        }

        [DataTestMethod]
        [DataRow(username, password, product)]
        public void Test_SearchProduct_Good_SearchForEveryProduct(string username, string password, string product)
        {
            Product prod = Test_AddProduct_Good(username, password, product);
            Response<List<Product>> searchResult = service.SearchProduct(1, "", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            Assert.IsTrue(searchResult.Value.Count() > 0);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_SearchProduct_Bad_NoProducts(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Response<List<Product>> searchResult = service.SearchProduct(1, "", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            Assert.AreEqual(0, searchResult.Value.Count());
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_SearchProduct_Bad_WrongName(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<List<Product>> searchResult = service.SearchProduct(1, "Worong", "", -1, -1, -1);
            Assert.IsFalse(searchResult.ErrorOccured);
            if (searchResult.Value.Count > 0)
                AssertProductsNotEqual(prod, searchResult.Value.First());
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_AddToCart_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.AddToCart(1, prod.Id, storeId, 1);
            Assert.IsFalse(resProd.ErrorOccured);
            Assert.IsNotNull(resProd.Value);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_AddToCart_Bad_NoSuchProduct(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.AddToCart(1, 20, storeId, 1);
            Assert.IsTrue(resProd.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_AddToCart_Bad_NotEnoughQuantity(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.AddToCart(1, prod.Id, storeId, 100);
            Assert.IsTrue(resProd.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_AddToCart_Bad_AddZero(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<Product> resProd = service.AddToCart(1, prod.Id, storeId, 0);
            Assert.IsTrue(resProd.ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ViewCart_Good_EmptyCart(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            Response<ShoppingCart> resSC = service.ViewCart(1);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(0, resSC.Value.ShoppingBags.Count);
            Assert.AreEqual(0.0, resSC.Value.Price);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ViewCart_Good_FullCart(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 10.0, 1, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Response<ShoppingCart> resSC = service.ViewCart(1);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(10.0, resSC.Value.Price);
            Assert.AreEqual(1, resSC.Value.ShoppingBags.Count);
            Assert.AreEqual(1, resSC.Value.ShoppingBags.Where(sb => sb.StoreId == storeId).First().Products.First().Quantity);
            AssertProductsEqual(resSC.Value.ShoppingBags.Where(sb => sb.StoreId == storeId).First().Products.First(), prod);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_EditCart_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 10.0, 10, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Response<ShoppingCart> resSC = service.EditCart(1, prod.Id, 5);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(50.0, resSC.Value.Price);
            Assert.AreEqual(5, resSC.Value.ShoppingBags.Where(sb => sb.StoreId == storeId).First().Products.First().Quantity);
            resSC = service.EditCart(1, prod.Id, 1);
            Assert.IsFalse(resSC.ErrorOccured);
            Assert.AreEqual(1, resSC.Value.ShoppingBags.Where(sb => sb.StoreId == storeId).First().Products.First().Quantity);
        }


        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_BuyCart_Good_MoreThenEnoughInStock(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 2, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.AddToCart(1, prod.Id, storeId, 10).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_BuyCart_Good_LastOneInStock(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 1, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.AddToCart(1, prod.Id, storeId, 10).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_BuyCart_Bad_NothingInCart(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_RemoveProductFromStore_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.RemoveProductFromStore(1, username, storeId, prod.Id).ErrorOccured);
            Assert.IsTrue(service.AddToCart(1, prod.Id, storeId, 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_RemoveProductFromStore_Bad(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.RemoveProductFromStore(1, username, storeId, 0).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductName_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductName(1, username, storeId, prod.Id, "newName").ErrorOccured);
            prod = service.AddToCart(1, prod.Id, storeId, 1).Value;
            Assert.AreEqual("newName", prod.Name);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductName_Bad_NoSuchProduct(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.ChangeProductName(1, username, storeId, 0, "newName").ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void Test_ChangeProductName_Bad_InvalidName(string username, string password, string newName)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductName(1, username, storeId, prod.Id, newName).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductPrice_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductPrice(1, username, storeId, prod.Id, 1711).ErrorOccured);
            prod = service.AddToCart(1, prod.Id, storeId, 1).Value;
            Assert.AreEqual(1711, prod.BasePrice);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductPrice_Bad_NoSuchProduct(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.ChangeProductPrice(1, username, storeId, 0, 1711).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, -1)]
        //[DataRow(username, password, 0)]
        public void Test_ChangeProductPrice_Bad_InvalidPrice(string username, string password, int price)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductPrice(1, username, storeId, prod.Id, price).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductQuantity_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductQuantity(1, username, storeId, prod.Id, 1711).ErrorOccured);
            Assert.IsFalse(service.AddToCart(1, prod.Id, storeId, 1711).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductQuantity_Bad_NoSuchProduct(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.ChangeProductQuantity(1, username, storeId, 0, 1711).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, -1)]
        public void Test_ChangeProductQuantity_Bad_InvalidQuantity(string username, string password, int quantity)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductQuantity(1, username, storeId, prod.Id, quantity).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductCategory_Good(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsFalse(service.ChangeProductCategory(1, username, storeId, prod.Id, "newName").ErrorOccured);
            prod = service.AddToCart(1, prod.Id, storeId, 1).Value;
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void Test_ChangeProductCategory_Bad_NoSuchProduct(string username, string password)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Assert.IsTrue(service.ChangeProductCategory(1, username, storeId, 0, "newName").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void Test_ChangeProductCategory_Bad_InvalidCategory(string username, string password, string cat)
        {
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 10, "cat1").Value;
            Assert.IsTrue(service.ChangeProductCategory(1, username, storeId, prod.Id, cat).ErrorOccured);
        }

        public bool BuyProduct_Thread(int userId, string user, string password, int productId, int storeId, int quantity)
        {
            Assert.IsFalse(service.Login(userId, user, password, DateTime.Now).ErrorOccured);
            bool ret = service.AddToCart(userId, productId, storeId, quantity).ErrorOccured;
            ret = ret & service.BuyCart(userId, cc, address, DateTime.Now).ErrorOccured;
            return ret;
        }

        [TestMethod]
        public void Test_BuyProductBad_BuylastAtTheSameTime()
        {
            bool res1 = false;
            bool res2 = false;

            service.EnterMarket(1, DateTime.Now);
            service.EnterMarket(2, DateTime.Now);
            service.EnterMarket(3, DateTime.Now);
            Assert.IsFalse(service.Register(1, "buyer1", "1", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(2, "buyer2", "2", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Register(3, "Owner", "own", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
            Assert.IsFalse(service.Login(3, "Owner", "own", DateTime.Now).ErrorOccured);

            int storeId = service.CreateNewStore(3, "Owner", "RandomStore", DateTime.Now).Value.StoreId;
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
        public void Test_AddProductPurchaseTerm_Good_Simple(string member, string type, string action, string value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm(type, action, value)(product.Id), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "t", ">", "10")]
        [DataRow("member1", "t", ">", "-2")]
        [DataRow("member1", "q", "$", "5")]
        [DataRow("member1", "h", "!=", "25:00")]
        [DataRow("member1", "d", "!=", "27/08/2020")]
        public void Test_AddProductPurchaseTerm_Bad_Simple_WrongParameters(string member, string type, string action, string value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsTrue(service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm(type, action, value)(product.Id), product.Id).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void Test_AddProductPurchaseTerm_Good_And(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductPurchaseTerm(1, member, store.StoreId, (makeAndPurchaseTerm(makeSimpleProductPurchaseTerm(l_type, l_action, l_value)(product.Id), makeSimpleProductPurchaseTerm(r_type, r_action, r_value)(product.Id))), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void Test_AddProductPurchaseTerm_Good_Or(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductPurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleProductPurchaseTerm(l_type, l_action, l_value)(product.Id), makeSimpleProductPurchaseTerm(r_type, r_action, r_value)(product.Id)), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10")]
        [DataRow("member1", "q", "<", "5")]
        [DataRow("member1", "h", "!=", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2023")]
        public void Test_AddCategoryPurchaseTerm_Good_Simple(string member, string type, string action, string value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm(type, action, value)("Category1"), "Category1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "t", ">", "10")]
        [DataRow("member1", "q", "<", "-2")]
        [DataRow("member1", "h", "$", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2020")]
        public void Test_AddCategoryPurchaseTerm_Bad_Simple_WrongParameters(string member, string type, string action, string value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsTrue(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm(type, action, value)("Category1"), "Category1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void Test_AddCategoryPurchaseTerm_Good_And(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeAndPurchaseTerm(makeSimpleCategoryPurchaseTerm(l_type, l_action, l_value)("Category1"), makeSimpleCategoryPurchaseTerm(r_type, r_action, r_value)("Category1")), "Category1").ErrorOccured);
        }


        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void Test_AddCategoryPurchaseTerm_Good_Or(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 20.4, 10, "Category1").Value;
            Assert.IsFalse(service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleCategoryPurchaseTerm(l_type, l_action, l_value)("Category1"), makeSimpleCategoryPurchaseTerm(r_type, r_action, r_value)("Category1")), "Category1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10")]
        [DataRow("member1", "q", "<", "5")]
        [DataRow("member1", "h", "!=", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2023")]
        public void Test_AddStorePurchaseTerm_Good_Simple(string member, string type, string action, string value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsFalse(service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm(type, action, value)).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "t", ">", "10")]
        [DataRow("member1", "q", "<", "-2")]
        [DataRow("member1", "h", "$", "01:00")]
        [DataRow("member1", "d", "!=", "27/08/2020")]
        public void Test_AddStorePurchaseTerm_Bad_WrongParameters(string member, string type, string action, string value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsTrue(service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm(type, action, value)).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void Test_AddStorePurchaseTerm_Good_And(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsFalse(service.AddStorePurchaseTerm(1, member, store.StoreId, makeAndPurchaseTerm(makeSimpleBagPurchaseTerm(l_type, l_action, l_value), makeSimpleBagPurchaseTerm(r_type, r_action, r_value))).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "p", ">", "10", "q", "<", "5")]
        [DataRow("member1", "h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("member1", "d", "!=", "20/01/2023", "q", "<=", "2")]
        public void Test_AddStorePurchaseTerm_Good_Or(string member, string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsFalse(service.AddStorePurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleBagPurchaseTerm(l_type, l_action, l_value), makeSimpleBagPurchaseTerm(r_type, r_action, r_value))).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", ">", 10)]
        [DataRow("member1", ">=", 13)]
        [DataRow("member1", "!=", 18)]
        public void Test_AddUserPurchaseTerm_Good_Simple(string member, string action, int age)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsFalse(service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(action, age)).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", "$", 10)]
        [DataRow("member1", ">=", -2)]
        public void Test_AddUserPurchaseTerm_Bad_Simple_WrongParameters(string member, string action, int age)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsTrue(service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(action, age)).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow("member1", ">", 10, "!=", 18)]
        [DataRow("member1", ">=", 13, ">", 10)]
        [DataRow("member1", "!=", 18, ">=", 16)]
        public void Test_AddUserPurchaseTerm_Good_And(string member, string l_action, int l_age, string r_action, int r_age)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsFalse(service.AddUserPurchaseTerm(1, member, store.StoreId, makeAndPurchaseTerm(makeSimpleUserPurchaseTerm(l_action, l_age), makeSimpleUserPurchaseTerm(r_action, r_age))).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("member1", ">", 10, "!=", 18)]
        [DataRow("member1", ">=", 13, ">", 10)]
        [DataRow("member1", "!=", 18, ">=", 16)]
        public void Test_AddUserPurchaseTerm_Good_Or(string member, string l_action, int l_age, string r_action, int r_age)
        {
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Assert.IsFalse(service.AddUserPurchaseTerm(1, member, store.StoreId, makeOrPurchaseTerm(makeSimpleUserPurchaseTerm(l_action, l_age), makeSimpleUserPurchaseTerm(r_action, r_age))).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddProductPurchaseTerm_GoodTerm()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("p", ">", "200")(p1.Id), p1.Id);
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("q", "<", "4")(p1.Id), p1.Id);
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("d", "=", DateTime.Now.ToString("dd/MM/yyyy"))(p1.Id), p1.Id);
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("h", ">", "00:01")(p1.Id), p1.Id);
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 2)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        [DataRow("=", 1)]
        public void Test_AddProductPurchaseTerm_Bad_BadQuantity(string action, int n)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("q", action, "2")(p1.Id), p1.Id);
            service.AddToCart(1, p1.Id, store.StoreId, n);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 2)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        [DataRow("=", 1)]
        public void Test_AddProductPurchaseTerm_Bad_BadPrice(string action, int n)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddProductPurchaseTerm(1, member, store.StoreId, makeSimpleProductPurchaseTerm("p", action, "200")(p1.Id), p1.Id);
            service.AddToCart(1, p1.Id, store.StoreId, n);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 4)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        public void Test_AddCategoryPurchaseTerm_Good(string action, int n)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Desc1", 100.0, 10, "Category1").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Desc2", 100.0, 10, "Category1").Value;
            service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm("q", action, "3")("Category1"), "Category1");
            service.AddToCart(1, p1.Id, store.StoreId, n - 1);
            service.AddToCart(1, p2.Id, store.StoreId, 1);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("=", 2)]
        [DataRow("<", 2)]
        [DataRow(">", 1)]
        public void Test_AddCategoryPurchaseTerm_Bad_BadQuantity_WithTwoProducts(string action, int n)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Desc1", 100.0, 10, "Category1").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Desc2", 100.0, 10, "Category1").Value;
            service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm("q", action, "3")("Category1"), "Category1");
            service.AddToCart(1, p1.Id, store.StoreId, n);
            service.AddToCart(1, p2.Id, store.StoreId, n);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(">", 2)]
        [DataRow("<", 2)]
        [DataRow("=", 3)]
        [DataRow("=", 1)]
        public void Test_AddCategoryPurchaseTerm_Bad_BadQuantity(string action, int n)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Desc1", 100.0, 10, "Category1").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Desc2", 100.0, 10, "Category1").Value;
            service.AddCategoryPurchaseTerm(1, member, store.StoreId, makeSimpleCategoryPurchaseTerm("q", action, "2")("Category1"), "Category1");
            if (n != 1)
            {
                service.AddToCart(1, p1.Id, store.StoreId, n - 1);
                service.AddToCart(1, p2.Id, store.StoreId, 1);
            }
            else
            {
                service.AddToCart(1, p1.Id, store.StoreId, 1);
            }
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddUserPurchaseTerm_Good()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(">", 18));
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddUserPurchaseTerm_Bad_BadAge()
        {
            string member = "member1";
            service.EnterMarket(1, DateTime.Now);
            service.Register(1, member, "password1", DateTime.Parse("Aug 30, 2016"));
            service.Login(1, member, "password1", DateTime.Now);
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            service.AddUserPurchaseTerm(1, member, store.StoreId, makeSimpleUserPurchaseTerm(">", 18));
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
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
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm(type, action, val));
            service.AddToCart(1, p1.Id, store.StoreId, 1);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddStorePurchaseTerm_BadHour()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm("h", "=", "04:04"));
            service.AddToCart(1, p1.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddStorePurchaseTerm_Bad_BadDate()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Prod1", "Desc1", 100.0, 100, "Cat1").Value;
            service.AddStorePurchaseTerm(1, member, store.StoreId, makeSimpleBagPurchaseTerm("d", "=", "30/08/2030"));
            service.AddToCart(1, p1.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
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
                                            ",\"productId\": " + id.ToString() + "}}, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: 3, \"productId\": " + id.ToString() + "}}";

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
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsFalse(service.AddProductDiscount(1, member, store.StoreId, discount(product.Id), product.Id).ErrorOccured);
        }

        [TestMethod]
        public void Test_AddProductDiscount_Bad_NoSuchProduct()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsTrue(service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(30)(product.Id + 1), product.Id + 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: -2, productId: 1}} ")]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: 0.5, productId: 1}} ")]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '$', value: 3, productId: 1}} ")]
        public void Test_AddProductDiscount_Bad_WrongParameters(string discount)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsTrue(service.AddProductDiscount(1, member, store.StoreId, discount, product.Id).ErrorOccured);
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
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "Category1").Value;
            Assert.IsTrue(service.AddProductDiscount(1, member, store.StoreId, discount(product.Id), product.Id).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: 5,category: 'cat1'}} ")]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'p',action: '<',value: 0.5,category: 'cat1'}} ")]
        public void Test_AddCategoryDiscount_Good(string discount)
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
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
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 10, "cat1");
            Assert.IsTrue(service.AddCategoryDiscount(1, member, store.StoreId, discount, "cat1").ErrorOccured);
        }

        [TestMethod]
        public void Test_BuyCart_ProductAndCategoryDiscounts()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddCategoryDiscount(1, member, store.StoreId, makeSimpleCategoryDiscount(50)("Cat1"), "Cat1");
            service.AddCategoryDiscount(1, member, store.StoreId, makeSimpleCategoryDiscount(10)("Cat2"), "Cat2");
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 615;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);
        }

        [TestMethod]
        public void Test_BuyCart_ProductAndStoreDiscounts()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddStoreDiscount(1, member, store.StoreId, makeSimpleStoreDiscount(10));
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 745;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);
        }

        [TestMethod]
        public void Test_BuyCart_ConditionalProductDiscounts()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeConditionalProductDiscount(10, 2)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeConditionalProductDiscount(20, 2)(p2.Id), p2.Id);
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 920;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);
        }

        [TestMethod]
        public void Test_BuyCart_CompositeProductDiscounts()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeAndproductDiscount(10, 20)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(10, 20)(p2.Id), p2.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(30, 40)(p3.Id), p3.Id);
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 720;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);
        }

        // NEW FOR VERSION 3

        [TestMethod]
        public void Test_Scenario1()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            string member3 = "Member3";
            Test_Login_Good(1, member1, "Password1");
            Test_Login_Good(2, member2, "Password2");
            Test_Login_Good(3, member3, "Password3");
            Store store = service.CreateNewStore(1, member1, "Store1", DateTime.Now).Value;
            service.NominateStoreOwner(1, member1, member2, store.StoreId, DateTime.Now);
            Product p1 = service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 100.0, 5, "Category1").Value;
            Product p2 = service.AddProduct(1, member1, store.StoreId, "Product2", "Description2", 10.0, 5, "Category2").Value;
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(10)("Category1"), "Category1");
            service.AddProductDiscount(2, member2, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            Assert.IsFalse(service.AddToCart(3, p1.Id, store.StoreId, 2).ErrorOccured); // 200 before discount, 180 after
            Assert.IsFalse(service.AddToCart(3, p2.Id, store.StoreId, 3).ErrorOccured); // 30 before discount, 24 after
            Assert.AreEqual(204, service.BuyCart(3, cc, address, DateTime.Now).Value);
            string review_string = "Good Product, that is!";
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
            Test_Login_Good(1, member1, "Password1");
            Test_Login_Good(2, member2, "Password2");
            Test_Login_Good(3, member3, "Password3");

            Store store = service.CreateNewStore(1, member1, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member1, store.StoreId, "Product1", "Description1", 100.0, 5, "Category1").Value;
            Product p2 = service.AddProduct(1, member1, store.StoreId, "Product2", "Description2", 10.0, 5, "Category2").Value;
            service.AddToCart(2, p1.Id, store.StoreId, 2);
            service.AddToCart(2, p2.Id, store.StoreId, 3);
            service.Logout(2, member2);
            service.ExitMarket(2);
            service.EnterMarket(55, DateTime.Now);
            service.Login(55, member2, "Password2", DateTime.Now);
            ShoppingCart cart = service.ViewCart(55).Value;
            Assert.AreEqual(1, cart.ShoppingBags.Count);
            Assert.AreEqual(2, cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.Count);
            Assert.AreEqual("Product1", cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.First().Name);
            Assert.AreEqual(2, cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.First().Quantity);
            Assert.AreEqual(p1.Id, cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.First().Id);

            Assert.AreEqual("Product2", cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.Last().Name);
            Assert.AreEqual(3, cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.Last().Quantity);
            Assert.AreEqual(p2.Id, cart.ShoppingBags.Where(sb => sb.StoreId == store.StoreId).First().Products.Last().Id);

            Assert.IsFalse(service.BuyCart(55, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_HoldedNotifications_NoHoldedNotifications()
        {
            Test_Register_Good(1, "Member2", "Password2");
            Test_Register_Good(2, "Member3", "Password3");
            Test_Register_Good(0, "Member1", "Password1");
            Response<KeyValuePair<Member, List<Notification>>> response1 = service.Login(0, "Member1", "Password1", DateTime.Now);

            Assert.AreEqual(0, response1.Value.Value.Count);
            Response<Store> resStore = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);

            Assert.IsFalse(resStore.ErrorOccured);
            Store store1 = resStore.Value;

            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId, DateTime.Now);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId, DateTime.Now);

            Response<KeyValuePair<Member, List<Notification>>> response2 = service.Login(1, "Member2", "Password2", DateTime.Now);

            Assert.IsFalse(response2.ErrorOccured);
            Assert.IsNotNull(response2.Value);
            Assert.IsNotNull(response2.Value.Value);
            Assert.AreEqual(0, response2.Value.Value.Count);

            Response<KeyValuePair<Member, List<Notification>>> response3 = service.Login(2, "Member3", "Password3", DateTime.Now);

            Assert.IsFalse(response3.ErrorOccured);
            Assert.IsNotNull(response3.Value);
            Assert.IsNotNull(response3.Value.Value);
            Assert.AreEqual(0, response3.Value.Value.Count);
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_CloseStore()
        {
            Test_Register_Good(1, "Member2", "Password2");
            Test_Register_Good(2, "Member3", "Password3");
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resStore = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
            Assert.IsFalse(resStore.ErrorOccured);
            Store store1 = resStore.Value;

            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId, DateTime.Now);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId, DateTime.Now);

            service.Login(1, "Member2", "Password2", DateTime.Now);
            service.Login(2, "Member3", "Password3", DateTime.Now);
            service.Logout(1, "Member2");
            service.Logout(2, "Member3");
            service.CloseStore(0, "Member1", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2", DateTime.Now);

            Assert.IsFalse(response4.ErrorOccured);
            Assert.AreEqual(1, response4.Value.Value.Count);
            //Assert.AreEqual("Member1", response4.Value.Value[0].Sender);
            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3", DateTime.Now);

            Assert.IsFalse(response5.ErrorOccured);
            Assert.AreEqual(1, response5.Value.Value.Count);
            //Assert.AreEqual("Member1", response5.Value.Value[0].Sender);
        }

        [TestMethod]
        public void Test_CloseStore_Good()
        {
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId);
            Assert.IsFalse(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_CloseStore_Bad_NoPermission()
        {
            Test_Login_Good(0, "Member1", "Password1");
            Test_Login_Good(1, "Member2", "Password2");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(1, "Member2", store1.StoreId);
            Assert.IsTrue(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_CloseStore_Bad_NoSuchStore()
        {
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
            Assert.IsFalse(resp1.ErrorOccured);
            Store store1 = resp1.Value;
            Assert.IsNotNull(store1);
            Response resp2 = service.CloseStore(0, "Member1", store1.StoreId + 1);
            Assert.IsTrue(resp2.ErrorOccured);
        }

        [TestMethod]
        public void Test_CloseStore_Bad_AlreadyClosed()
        {
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
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
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
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
            Test_Login_Good(0, "Member1", "Password1");
            Test_Login_Good(1, "Member2", "Password2");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
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
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
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
            Test_Login_Good(0, "Member1", "Password1");
            Response<Store> resp1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now);
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
            Test_Register_Good(1, "Member2", "Password2");
            Test_Register_Good(2, "Member3", "Password3");
            Test_Login_Good(0, "Member1", "Password1");
            Store store1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now).Value;

            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId, DateTime.Now);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId, DateTime.Now);

            service.CloseStore(0, "Member1", store1.StoreId);

            service.Login(1, "Member2", "Password2", DateTime.Now);
            service.Login(2, "Member3", "Password3", DateTime.Now);
            service.Logout(1, "Member2");
            service.Logout(2, "Member3");

            service.OpenStore(0, "Member1", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2", DateTime.Now);
            Assert.IsFalse(response4.ErrorOccured);
            Assert.AreEqual(1, response4.Value.Value.Count);
            //Assert.AreEqual("Member1", response4.Value.Value[0].Sender);

            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3", DateTime.Now);
            Assert.IsFalse(response5.ErrorOccured);
            Assert.AreEqual(1, response5.Value.Value.Count);
            //Assert.AreEqual("Member1", response5.Value.Value[0].Sender);
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_BuyingProductFromYourStore()
        {
            // Store owners need to get a notification when someone buys from them
            Test_Login_Good(0, "Member1", "Password1");
            Test_Register_Good(1, "Member2", "Password2");
            Test_Register_Good(2, "Member3", "Password3");
            Test_Register_Good(3, "Member4", "Password4");

            Store store1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now).Value;
            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId, DateTime.Now);
            service.NominateStoreManager(0, "Member1", "Member3", store1.StoreId, DateTime.Now);
            Product product1 = service.AddProduct(0, "Member1", store1.StoreId, "ProductName1", "Description1", 2.7, 10, "Category1").Value;
            service.Logout(0, "Member1");
            service.Login(3, "Member4", "Password4", DateTime.Now);
            service.AddToCart(3, product1.Id, store1.StoreId, 1);
            service.BuyCart(3, cc, address, DateTime.Now);
            Response<KeyValuePair<Member, List<Notification>>> response6 = service.Login(0, "Member1", "Password1", DateTime.Now);
            Assert.AreEqual(1, response6.Value.Value.Count);
            //Assert.AreEqual("Member4", response6.Value.Value[0].Sender);
            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2", DateTime.Now);
            Assert.AreEqual(1, response4.Value.Value.Count);
            //Assert.AreEqual("Member4", response4.Value.Value[0].Sender);
            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3", DateTime.Now);
            Assert.AreEqual(0, response5.Value.Value.Count);
            //Assert.AreEqual("Member4", response5.Value.Value[0].Sender);
        }

        [TestMethod]
        public void Test_HoldedNotifications_From_RemovingNomination()
        {
            // Store owners and managers need to get a notification when their nomination is removed
            Test_Register_Good(1, "Member2", "Password2");
            Test_Register_Good(2, "Member3", "Password3");
            Test_Login_Good(0, "Member1", "Password1");
            Test_Login_Good(3, "Member4", "Password4");
            Store store1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now).Value;
            service.NominateStoreOwner(0, "Member1", "Member4", store1.StoreId, DateTime.Now);
            service.NominateStoreOwner(3, "Member4", "Member2", store1.StoreId, DateTime.Now);
            Assert.AreEqual(1, service.TakeNotifications(0, "Member1").Value.Count);
            Assert.AreEqual(1, service.TakeNotifications(3, "Member4").Value.Count);
            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId, DateTime.Now);
            service.NominateStoreManager(3, "Member4", "Member3", store1.StoreId, DateTime.Now);
            service.Logout(3, "Member4");
            service.RemoveStoreOwnerNomination(0, "Member1", "Member4", store1.StoreId);

            Response<KeyValuePair<Member, List<Notification>>> response3 = service.Login(3, "Member4", "Password4", DateTime.Now);
            Assert.AreEqual(1, response3.Value.Value.Count);

            Response<KeyValuePair<Member, List<Notification>>> response4 = service.Login(1, "Member2", "Password2", DateTime.Now);
            Assert.AreEqual(1, response4.Value.Value.Count);

            Response<KeyValuePair<Member, List<Notification>>> response5 = service.Login(2, "Member3", "Password3", DateTime.Now);
            Assert.AreEqual(1, response5.Value.Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Good()
        {
            string FileName = "Test_InitializeFromFile_Good";
            StreamWriter file = File.CreateText(FileName);
            file.Write("enter-market(1,16/06/2022)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1,16/06/2022)\n" +
                "create-new-store(1,user1,store1,16/06/2022)");
            file.Flush();
            file.Close();
            Service service = new Service(externalSystem.Object, $"admin~admin~admin~22/08/1972\nss~{FileName}");
            Assert.IsTrue(service.WasInitializedWithFile);
            Assert.IsTrue(service.EnterMarket(1, DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsTrue(service.Login(1, "user1", "pass1", DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.GetAllStores(1).Value.Count == 1);
            Assert.IsTrue(service.GetAllStores(1).Value[0].Name.Equals("store1"));
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_IllegalOrderOfActions()
        {
            string FileName = "Test_InitializeFromFile_Bad_IllegalOrderOfActions";
            StreamWriter file = File.CreateText(FileName);
            file.Write("enter-market(1)\n" +
                "login(1,user1,pass1\n" +
                "create-new-store(1,user1,store1)");
            file.Flush();
            file.Close();
            Service service = new Service(externalSystem.Object, $"admin~admin~admin~22/08/1972\nss~{FileName}");
            // Expecting error: need to register before logging in
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1, DateTime.Now).ErrorOccured); // Make sure nothing happend after it failed
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("Aug 22, 1972")).ErrorOccured); // Make sure nothing happend after it failed
            Assert.IsFalse(service.Login(1, "user1", "pass1", DateTime.Now).ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_UnrecognizedCommand()
        {
            string FileName = "Test_InitializeFromFile_Bad_UnrecognizedCommand";
            StreamWriter file = File.CreateText(FileName);
            file.Write("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1,store1)\n" +
                "blah-blah(1,user1,pass1,store1)");
            file.Flush();
            file.Close();
            Service service = new Service(externalSystem.Object, $"admin~admin~admin~22/08/1972\nss~{FileName}");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1", DateTime.Now).ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_BadArgumentsForCommand_Overshoot()
        {
            string FileName = "Test_InitializeFromFile_Bad_BadArgumentsForCommand_Overshoot";
            StreamWriter file = File.CreateText(FileName);
            file.Write("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1,store1,failureInLife1)");
            file.Flush();
            file.Close();
            Service service = new Service(externalSystem.Object, $"admin~admin~admin~22/08/1972\nss~{FileName}");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1", DateTime.Now).ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_BadArgumentsForCommand_Undershoot()
        {
            string FileName = "Test_InitializeFromFile_Bad_BadArgumentsForCommand_Undershoot";
            StreamWriter file = File.CreateText(FileName);
            file.Write("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(1,user1)");
            file.Flush();
            file.Close();
            Service service = new Service(externalSystem.Object, $"admin~admin~admin~22/08/1972\nss~{FileName}");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1", DateTime.Now).ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_InitializeFromFile_Bad_BadArgumentsForCommand_WrongType()
        {
            string FileName = "Test_InitializeFromFile_Bad_BadArgumentsForCommand_WrongType";
            StreamWriter file = File.CreateText(FileName);
            file.Write("enter-market(1)\n" +
                "register(1,user1,pass1,22/08/1972)\n" +
                "login(1,user1,pass1)\n" +
                "create-new-store(FAILME,user1,store1)");
            file.Flush();
            file.Close();
            Service service = new Service(externalSystem.Object, $"admin~admin~admin~22/08/1972\nss~{FileName}");
            Assert.IsFalse(service.WasInitializedWithFile);
            Assert.IsFalse(service.EnterMarket(1, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.Register(1, "user1", "pass1", DateTime.Parse("22/08/1972")).ErrorOccured);
            Assert.IsFalse(service.Login(1, "user1", "pass1", DateTime.Now).ErrorOccured);
            Assert.AreEqual(0, service.GetAllStores(1).Value.Count);
        }

        [TestMethod]
        public void Test_AddActionToManager_Good()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            Test_Login_Good(1, member1, "password1");
            Test_Login_Good(2, member2, "password2");
            Store store = service.CreateNewStore(1, member1, "Store1", DateTime.Now).Value;
            service.NominateStoreManager(1, member1, member2, store.StoreId, DateTime.Now);
            Assert.IsTrue(service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
            Assert.IsFalse(service.AddActionToManager(1, member1, member2, store.StoreId, "AddProduct").ErrorOccured);
            Assert.IsFalse(service.AddProduct(2, member2, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddActionToManager_Bad_NoSuchAction()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            Test_Login_Good(1, member1, "password1");
            Test_Login_Good(2, member2, "password2");
            Store store = service.CreateNewStore(1, member1, "Store1", DateTime.Now).Value;
            service.NominateStoreManager(1, member1, member2, store.StoreId, DateTime.Now);
            Assert.IsTrue(service.AddActionToManager(1, member1, member2, store.StoreId, "BlahBlah").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddActionToManager_Bad_DoesntWorkOnNotManager()
        {
            string member1 = "Member1";
            string member2 = "Member2";
            Test_Login_Good(1, member1, "password1");
            Test_Login_Good(2, member2, "password2");
            Store store = service.CreateNewStore(1, member1, "Store1", DateTime.Now).Value;
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
            Test_Login_Good(1, member1, "password1");
            Test_Login_Good(2, member2, "password2");
            Test_Login_Good(3, member3, "password3");
            Store store = service.CreateNewStore(1, member1, "Store1", DateTime.Now).Value;
            service.NominateStoreManager(1, member1, member2, store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, member1, member3, store.StoreId, DateTime.Now);
            Assert.IsTrue(service.AddProduct(3, member3, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
            Assert.IsTrue(service.AddActionToManager(2, member2, member3, store.StoreId, "AddProduct").ErrorOccured);
            Assert.IsTrue(service.AddProduct(3, member3, store.StoreId, "Product1", "Description1", 10.0, 3, "Category1").ErrorOccured);
        }

        [TestMethod]
        public void Test_ExternalSystem_SupplyDeclines()
        {
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            bool SUPPLY_FLAG = false;
            bool CANCEL_SUPPLY_FLAG = false;
            bool PAY_FLAG = false;
            bool CANCEL_PAY_FLAG = false;
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5) => SUPPLY_FLAG = true).Returns(-1);
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Callback((int n1) => CANCEL_SUPPLY_FLAG = true).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5, string s6) => PAY_FLAG = true).Returns(150000);
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Callback((int n1) => CANCEL_PAY_FLAG = true).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Returns(true);
            service = new Service(externalSystem.Object);

            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 2, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
            Assert.IsTrue(SUPPLY_FLAG);
            Assert.IsFalse(CANCEL_SUPPLY_FLAG);
            Assert.IsTrue(!(CANCEL_PAY_FLAG ^ PAY_FLAG));
        }

        [TestMethod]
        public void Test_ExternalSystem_PayDeclines()
        {
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            bool SUPPLY_FLAG = false;
            bool CANCEL_SUPPLY_FLAG = false;
            bool PAY_FLAG = false;
            bool CANCEL_PAY_FLAG = false;
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5) => SUPPLY_FLAG = true).Returns(150000);
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Callback((int n1) => CANCEL_SUPPLY_FLAG = true).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5, string s6) => PAY_FLAG = true).Returns(-1);
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Callback((int n1) => CANCEL_PAY_FLAG = true).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Returns(true);
            service = new Service(externalSystem.Object);

            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 2, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Assert.IsTrue(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
            Assert.IsTrue(PAY_FLAG);
            Assert.IsFalse(CANCEL_PAY_FLAG);
            Assert.IsTrue(!(CANCEL_SUPPLY_FLAG ^ SUPPLY_FLAG));
        }

        [TestMethod]
        public void Test_ExternalSystem_Real_BuyCart()
        {
            IExternalSystem externalSystem = new ExternalSystem();
            service = new Service(externalSystem);
            Test_Login_Good(1, username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value.StoreId;
            Product prod = service.AddProduct(1, username, storeId, product, "Good", 1.0, 2, "cat1").Value;
            service.AddToCart(1, prod.Id, storeId, 1);
            Assert.AreEqual(!externalSystem.IsExternalSystemOnline(), service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_ExternalSystem_NoPaymentAndSupplyIfBuyingFails()
        {
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            bool SUPPLY_FLAG = false;
            bool CANCEL_SUPPLY_FLAG = false;
            bool PAY_FLAG = false;
            bool CANCEL_PAY_FLAG = false;
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5) => SUPPLY_FLAG = true).Returns(150000);
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Callback((int n1) => CANCEL_SUPPLY_FLAG = true).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5, string s6) => PAY_FLAG = true).Returns(8688);
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Callback((int n1) => CANCEL_PAY_FLAG = true).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Returns(true);
            service = new Service(externalSystem.Object);

            Test_Login_Good(1, username, password);
            Store store = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value;
            Product prod = service.AddProduct(1, username, store.StoreId, product, "Good", 1.0, 2, "cat1").Value;
            service.AddStorePurchaseTerm(1, username, store.StoreId, makeSimpleBagPurchaseTerm("h", "=", "04:04"));
            service.EnterMarket(2, DateTime.Now);
            service.Register(2, "member2", "password2", DateTime.Now);
            service.Login(2, "member2", "password2", DateTime.Now);
            service.AddToCart(2, prod.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(2, cc, address, DateTime.Now).ErrorOccured);
            Assert.IsTrue(!(PAY_FLAG ^ CANCEL_PAY_FLAG));
            Assert.IsTrue(!(CANCEL_SUPPLY_FLAG ^ SUPPLY_FLAG));
        }

        [TestMethod]
        public void Test_ExternalSystems_NoConnection()
        {
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            bool SUPPLY_FLAG = false;
            bool CANCEL_SUPPLY_FLAG = false;
            bool PAY_FLAG = false;
            bool CANCEL_PAY_FLAG = false;
            bool IS_ONLINE_FLAG = false;
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5) => { SUPPLY_FLAG = true; throw new Exception(); }).Returns(150000);
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Callback((int n1) => { CANCEL_SUPPLY_FLAG = true; throw new Exception(); }).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2, string s3, string s4, string s5, string s6) => { PAY_FLAG = true; throw new Exception(); }).Returns(8688);
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Callback((int n1) => { CANCEL_PAY_FLAG = true; throw new Exception(); }).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Callback(() => { IS_ONLINE_FLAG = true; throw new Exception(); }).Returns(true);
            service = new Service(externalSystem.Object);

            Test_Login_Good(1, username, password);
            Store store = service.CreateNewStore(1, username, "RandomStore", DateTime.Now).Value;
            Product prod = service.AddProduct(1, username, store.StoreId, product, "Good", 1.0, 2, "cat1").Value;
            service.EnterMarket(2, DateTime.Now);
            service.Register(2, "member2", "password2", DateTime.Now);
            service.Login(2, "member2", "password2", DateTime.Now);
            service.AddToCart(2, prod.Id, store.StoreId, 1);
            Assert.IsTrue(service.BuyCart(2, cc, address, DateTime.Now).ErrorOccured);
            Assert.IsTrue(IS_ONLINE_FLAG);
            Assert.IsTrue(!(PAY_FLAG ^ CANCEL_PAY_FLAG));
            Assert.IsTrue(!(CANCEL_SUPPLY_FLAG ^ SUPPLY_FLAG));
        }

        [TestMethod]
        public void Test_TakeNotifications_Success_Empty()
        {
            Test_Login_Good(1, "Member1", "Password1");
            Response<List<Notification>> resp = service.TakeNotifications(1, "Member1");
            Assert.IsFalse(resp.ErrorOccured);
            Assert.AreEqual(0, resp.Value.Count);
        }

        [TestMethod]
        public void Test_TakeNotifications_Success_NonEmpty()
        {
            // Store owners and managers need to get a notification when their nomination is removed
            Test_Login_Good(0, "Member1", "Password1");
            Test_Login_Good(1, "Member2", "Password2");
            Store store1 = service.CreateNewStore(0, "Member1", "Store1", DateTime.Now).Value;
            service.NominateStoreOwner(0, "Member1", "Member2", store1.StoreId, DateTime.Now);
            service.RemoveStoreOwnerNomination(0, "Member1", "Member2", store1.StoreId);

            Response<List<Notification>> resp = service.TakeNotifications(1, "Member2");
            Assert.IsFalse(resp.ErrorOccured);
            Assert.IsNotNull(resp.Value);
            Assert.AreEqual(1, resp.Value.Count);
            Response<List<Notification>> resp1 = service.TakeNotifications(1, "Member2");
            Assert.IsFalse(resp1.ErrorOccured);
            Assert.IsNotNull(resp1.Value);
            Assert.AreEqual(0, resp1.Value.Count);
        }

        [TestMethod]
        public void Test_CancelMember_Success()
        {
            service.EnterMarket(0, DateTime.Now);
            Assert.IsFalse(service.Login(0, "admin", "admin", DateTime.Now).ErrorOccured);
            Test_Register_Good(1, "member", "pass");
            Assert.IsFalse(service.CancelMember(0, "admin", "member").ErrorOccured);
            Assert.IsFalse(service.Register(1, "member", "pass", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
        }

        [TestMethod]
        public void Test_CancelMember_Failure_NoSuchMember()
        {
            service.EnterMarket(0, DateTime.Now);
            service.EnterMarket(1, DateTime.Now);
            Assert.IsFalse(service.Login(0, "admin", "admin", DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.CancelMember(0, "admin", "member").ErrorOccured);
            Assert.IsFalse(service.Register(1, "member", "pass", DateTime.Parse("Aug 22, 1972")).ErrorOccured);
        }

        [TestMethod]
        public void Test_CancelMember_Failure_NoPermissions()
        {
            Test_Login_Good(0, "member1", "pass1");
            Test_Register_Good(1, "member2", "pass2");
            service.CreateNewStore(0, "member1", "Store1", DateTime.Now);
            Assert.IsTrue(service.CancelMember(0, "member1", "member2").ErrorOccured);
        }

        [TestMethod]
        public void Test_GetMembersOnlineStats_Success()
        {
            service.EnterMarket(0, DateTime.Now);
            service.Login(0, "admin", "admin", DateTime.Now);
            Test_Register_Good(1, "member1", "pass1");
            Test_Register_Good(2, "member2", "pass2");
            Test_Login_Good(3, "member3", "pass3");
            Response<Dictionary<Member, bool>> resp = service.GetMembersOnlineStats(0, "admin");
            Assert.IsFalse(resp.ErrorOccured);
            Assert.AreEqual(4, resp.Value.Count);
            foreach (Member m in resp.Value.Keys)
            {
                if (m.Username.Equals("member3") || m.Username.Equals("admin"))
                {
                    Assert.IsTrue(resp.Value[m]);
                }
                else if (m.Username.Equals("member1") || m.Username.Equals("member2"))
                {
                    Assert.IsFalse(resp.Value[m]);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void Test_GetMembersOnlineStats_Failure_NoPermission()
        {
            Test_Login_Good(3, "member3", "pass3");
            Assert.IsTrue(service.GetMembersOnlineStats(3, "member3").ErrorOccured);
        }

        [TestMethod]
        public void Test_AddToCart_StoreUpdatesQuantity()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "mem2", "pass2");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "mem1", st.StoreId, "p1", "d1", 10.0, 5, "cat1").Value;
            Product userP = service.AddToCart(2, p.Id, st.StoreId, 4).Value;
            Assert.AreEqual(4, userP.Quantity);
            Assert.AreEqual(1, service.GetAllStores(1).Value[0].Products[0].Quantity);
        }

        [TestMethod]
        public void Test_EditCart_InBoundsOfStore()
        {
            Test_Login_Good(1, "mem", "pass");
            Store st = service.CreateNewStore(1, "mem", "S1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "mem", st.StoreId, "p1", "d1", 10.0, 3, "cat1").Value;
            Test_Login_Good(2, "buyer", "pass");
            service.AddToCart(2, p.Id, st.StoreId, 2);
            Assert.IsTrue(service.EditCart(2, p.Id, 50).ErrorOccured);
            Assert.AreEqual(1, service.GetAllStores(2).Value[0].Products[0].Quantity);
            Response<ShoppingCart> resp = service.EditCart(2, p.Id, 3);
            Assert.IsFalse(resp.ErrorOccured);
            Assert.AreEqual(30.0, resp.Value.Price);
            Assert.AreEqual(0, service.GetAllStores(2).Value[0].Products[0].Quantity);
        }

        [TestMethod]
        public void Test_EditCart_ReturnsToStore()
        {
            Test_Login_Good(1, "mem", "pass");
            Store st = service.CreateNewStore(1, "mem", "S1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "mem", st.StoreId, "p1", "d1", 10.0, 3, "cat1").Value;
            Test_Login_Good(2, "buyer", "pass");
            service.AddToCart(2, p.Id, st.StoreId, 2);
            Response<ShoppingCart> resp = service.EditCart(2, p.Id, 0);
            Assert.IsFalse(resp.ErrorOccured);
            Assert.AreEqual(0, resp.Value.Price);
            Assert.AreEqual(3, service.GetAllStores(2).Value[0].Products[0].Quantity);
        }

        [TestMethod]
        public void Test_CreateNewStore_NoSuchFounder()
        {
            Test_Login_Good(1, "mem", "pass");
            Response<Store> resp = service.CreateNewStore(1, "mem1", "s1", DateTime.Now);
            Assert.IsTrue(resp.ErrorOccured);
        }

        [TestMethod]
        public void Test_StoreOwnerNominationAfterRemoval()
        {
            Test_Login_Good(1, "mem", "pass");
            Store st = service.CreateNewStore(1, "mem", "s1", DateTime.Now).Value;
            Test_Login_Good(2, "mem1", "pass");
            Assert.IsFalse(service.NominateStoreOwner(1, "mem", "mem1", st.StoreId, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.RemoveStoreOwnerNomination(1, "mem", "mem1", st.StoreId).ErrorOccured);
            Assert.IsFalse(service.NominateStoreOwner(1, "mem", "mem1", st.StoreId, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_RemoveStoreOwnerNomination_NotTheNominator()
        {
            Test_Login_Good(1, "mem", "pass");
            Test_Login_Good(2, "mem1", "pass");
            Test_Login_Good(3, "mem2", "pass");
            Store st = service.CreateNewStore(1, "mem", "s1", DateTime.Now).Value;
            Assert.IsFalse(service.NominateStoreOwner(1, "mem", "mem1", st.StoreId, DateTime.Now).ErrorOccured);
            Response<StoreOwner> resp1 = service.NominateStoreOwner(1, "mem", "mem2", st.StoreId, DateTime.Now);
            Assert.IsFalse(resp1.ErrorOccured);
            Assert.IsNull(resp1.Value);
            Response<StoreOwner> resp2 = service.NominateStoreOwner(2, "mem1", "mem2", st.StoreId, DateTime.Now);
            Assert.IsFalse(resp2.ErrorOccured);
            Assert.IsNotNull(resp2.Value);
            Response<Member> resp3 = service.RemoveStoreOwnerNomination(2, "mem1", "mem2", st.StoreId);
            Assert.IsTrue(resp3.ErrorOccured);
            Response<Member> resp4 = service.RemoveStoreOwnerNomination(1, "mem", "mem2", st.StoreId);
            Assert.IsFalse(resp4.ErrorOccured);
        }

        [TestMethod]
        public void Test_DailyIncomeMarketManager_Success()
        {
            service = new Service(externalSystem.Object);
            Test_Login_Good(1, "mem1", "pass1");
            service.EnterMarket(2, DateTime.Now);
            service.Login(2, "admin", "admin", DateTime.Now);
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "mem1", st.StoreId, "p1", "d1", 100.0, 5, "cat1").Value;
            service.AddToCart(1, p.Id, st.StoreId, 3);
            service.BuyCart(1, cc, address, DateTime.Now);
            service.AddToCart(1, p.Id, st.StoreId, 1);
            service.BuyCart(1, cc, address, DateTime.Parse("Aug 22, 1972"));
            Assert.AreEqual(300.0, service.GetDailyIncomeMarketManager(2, "admin").Value);
        }

        [TestMethod]
        public void Test_DailyIncomeMarketManager_MultipleStores()
        {
            service = new Service(externalSystem.Object);
            Test_Login_Good(1, "mem1", "pass1");
            service.EnterMarket(2, DateTime.Now);
            service.Login(2, "admin", "admin", DateTime.Now);
            Test_Login_Good(3, "mem2", "pass2");

            Store st1 = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, "mem1", st1.StoreId, "p1", "d1", 100.0, 5, "cat1").Value;

            Store st2 = service.CreateNewStore(3, "mem2", "s2", DateTime.Now).Value;
            Product p2 = service.AddProduct(3, "mem2", st2.StoreId, "p2", "d2", 50.0, 5, "cat2").Value;

            Assert.IsFalse(service.AddToCart(1, p1.Id, st1.StoreId, 3).ErrorOccured);
            Assert.IsFalse(service.AddToCart(1, p2.Id, st2.StoreId, 1).ErrorOccured);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);

            Assert.IsFalse(service.AddToCart(1, p1.Id, st1.StoreId, 1).ErrorOccured);
            Assert.IsFalse(service.AddToCart(1, p2.Id, st2.StoreId, 1).ErrorOccured);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Parse("Aug 22, 1972")).ErrorOccured);

            Assert.AreEqual(350.0, service.GetDailyIncomeMarketManager(2, "admin").Value);
        }

        [TestMethod]
        public void Test_DailyIncomeMarketManager_Failure_NoPermission()
        {
            service = new Service(externalSystem.Object);
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "Ron", "Ron");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "mem1", st.StoreId, "p1", "d1", 100.0, 5, "cat1").Value;
            service.AddToCart(1, p.Id, st.StoreId, 3);
            service.BuyCart(1, cc, address, DateTime.Now);
            service.AddToCart(1, p.Id, st.StoreId, 1);
            service.BuyCart(1, cc, address, DateTime.Parse("Aug 22, 1972"));
            Assert.IsTrue(service.GetDailyIncomeMarketManager(2, "Ron").ErrorOccured);
        }

        [TestMethod]
        public void Test_DailyIncomeMarketManager_SuccessWithDiscounts()
        {
            service = new Service(externalSystem.Object);
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            service.EnterMarket(2, DateTime.Now);
            service.Login(2, "admin", "admin", DateTime.Now);
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeAndproductDiscount(10, 20)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(10, 20)(p2.Id), p2.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(30, 40)(p3.Id), p3.Id);
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 720;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);
            Assert.AreEqual(expected_price, service.GetDailyIncomeMarketManager(2, "admin").Value);
        }

        [TestMethod]
        public void Test_DailyIncomeStoreOwner_Success()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "ron", "ron");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, "mem1", st.StoreId, "p1", "d1", 100.0, 5, "cat1").Value;
            Product p2 = service.AddProduct(1, "mem1", st.StoreId, "p2", "d2", 50.0, 5, "cat2").Value;

            service.AddToCart(2, p1.Id, st.StoreId, 3);
            service.AddToCart(2, p2.Id, st.StoreId, 1);
            service.BuyCart(2, cc, address, DateTime.Now);
            service.AddToCart(2, p1.Id, st.StoreId, 1);
            service.AddToCart(2, p2.Id, st.StoreId, 1);
            service.BuyCart(2, cc, address, DateTime.Parse("Aug 22, 1972"));

            Assert.AreEqual(350.0, service.GetDailyIncomeStore(1, "mem1", st.StoreId).Value);
        }

        [TestMethod]
        public void Test_DailyIncomeStoreOwner_MultipleUsers()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "ron", "ron");
            Test_Login_Good(3, "nir", "nir");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, "mem1", st.StoreId, "p1", "d1", 100.0, 5, "cat1").Value;
            Product p2 = service.AddProduct(1, "mem1", st.StoreId, "p2", "d2", 50.0, 5, "cat2").Value;

            service.AddToCart(2, p1.Id, st.StoreId, 2);
            service.AddToCart(2, p2.Id, st.StoreId, 1);
            service.BuyCart(2, cc, address, DateTime.Now);

            service.AddToCart(3, p1.Id, st.StoreId, 1);
            service.AddToCart(3, p2.Id, st.StoreId, 2);
            service.BuyCart(3, cc, address, DateTime.Now);

            service.AddToCart(2, p1.Id, st.StoreId, 1);
            service.AddToCart(2, p2.Id, st.StoreId, 1);
            service.BuyCart(2, cc, address, DateTime.Parse("Aug 22, 1972"));

            service.AddToCart(3, p1.Id, st.StoreId, 1);
            service.AddToCart(3, p2.Id, st.StoreId, 1);
            service.BuyCart(3, cc, address, DateTime.Parse("Aug 22, 1972"));

            Assert.AreEqual(450.0, service.GetDailyIncomeStore(1, "mem1", st.StoreId).Value);
        }

        [TestMethod]
        public void Test_DailyIncomeStoreOwner_Failure_NoPermissions()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Test_Login_Good(2, "ron", "ron");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, "mem1", st.StoreId, "p1", "d1", 100.0, 5, "cat1").Value;
            Product p2 = service.AddProduct(1, "mem1", st.StoreId, "p2", "d2", 50.0, 5, "cat2").Value;

            service.AddToCart(2, p1.Id, st.StoreId, 3);
            service.AddToCart(2, p2.Id, st.StoreId, 1);
            service.BuyCart(2, cc, address, DateTime.Now);
            service.AddToCart(2, p1.Id, st.StoreId, 1);
            service.AddToCart(2, p2.Id, st.StoreId, 1);
            service.BuyCart(2, cc, address, DateTime.Parse("Aug 22, 1972"));

            Assert.IsTrue(service.GetDailyIncomeStore(2, "ron", st.StoreId).ErrorOccured);
        }

        [TestMethod]
        public void Test_DailyIncomeStoreOwner_SuccessWithDiscounts()
        {
            string member = "member1";
            Test_Login_Good(1, member, "password1");
            Store store = service.CreateNewStore(1, member, "Store1", DateTime.Now).Value;
            Product p1 = service.AddProduct(1, member, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member, store.StoreId, makeAndproductDiscount(10, 20)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(10, 20)(p2.Id), p2.Id);
            service.AddProductDiscount(1, member, store.StoreId, makeXorproductDiscount(30, 40)(p3.Id), p3.Id);
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 720;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);
            Assert.AreEqual(expected_price, service.GetDailyIncomeStore(1, member, store.StoreId).Value);
        }

        [TestMethod]
        public void Test_RejectStoreOwnerNomination_Success()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Test_Login_Good(2, "mem2", "pass2");
            Test_Login_Good(3, "mem3", "pass3");
            Assert.IsFalse(service.NominateStoreOwner(1, "mem1", "mem2", st.StoreId, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.NominateStoreOwner(1, "mem1", "mem3", st.StoreId, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.RejectStoreOwnerNomination(2, "mem2", "mem3", st.StoreId).ErrorOccured);
            Assert.AreEqual(0, service.GetMemberPermissions(3, "mem3").Value.Count);
            Assert.IsFalse(service.NominateStoreOwner(1, "mem1", "mem3", st.StoreId, DateTime.Now).ErrorOccured);
            Assert.IsFalse(service.NominateStoreOwner(2, "mem2", "mem3", st.StoreId, DateTime.Now).ErrorOccured);
            Assert.AreEqual(1, service.GetMemberPermissions(3, "mem3").Value.Count);
        }

        [TestMethod]
        public void Test_RejectStoreOwnerNomination_Failure_NotVotingOn()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Test_Login_Good(2, "mem2", "pass2");
            Test_Login_Good(3, "mem3", "pass3");
            Assert.IsFalse(service.NominateStoreOwner(1, "mem1", "mem2", st.StoreId, DateTime.Now).ErrorOccured);
            Assert.IsTrue(service.RejectStoreOwnerNomination(2, "mem2", "mem3", st.StoreId).ErrorOccured);
            Assert.AreEqual(0, service.GetMemberPermissions(3, "mem3").Value.Count);
        }

        [TestMethod]
        public void Test_RejectStoreOwnerNomination_Failure_NotStoreOwner()
        {
            Test_Login_Good(1, "mem1", "pass1");
            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Now).Value;
            Test_Login_Good(2, "mem2", "pass2");
            Test_Login_Good(3, "mem3", "pass3");
            Assert.IsTrue(service.RejectStoreOwnerNomination(2, "mem2", "mem3", st.StoreId).ErrorOccured);
            Assert.AreEqual(0, service.GetMemberPermissions(3, "mem3").Value.Count);
        }

        private void ValidateDailyRangeInformation(List<StatisticsInformation> input, 
            DateTime begin,
            DateTime end,
            int EXPECTED_GUEST_COUNT, 
            int EXPECTED_MEMBER_COUNT, 
            int EXPECTED_MANAGER_COUNT, 
            int EXPECTED_OWNER_COUNT, 
            int EXPECTED_MARKET_COUNT)
        {
            int CURR_GUEST_COUNT = 0;
            int CURR_MEMBER_COUNT = 0;
            int CURR_MANAGER_COUNT = 0;
            int CURR_OWNER_COUNT = 0;
            int CURR_MARKET_COUNT = 0;

            Dictionary<string, StatisticsInformation> data = input.ToDictionary(si => si.Date);

            foreach (string date in data.Keys)
            {
                DateTime currentDate = DateTime.Parse(date);
                Assert.IsTrue(currentDate >= begin && currentDate <= end);
                StatisticsInformation si = data[date];
                Assert.AreEqual(date, si.Date);
                Assert.IsTrue(si.Guests != 0 || si.Members != 0 || si.StoreManagers != 0 || si.StoreOwners != 0 || si.MarketManagers != 0);
                CURR_GUEST_COUNT += si.Guests;
                CURR_MEMBER_COUNT += si.Members;
                CURR_MANAGER_COUNT += si.StoreManagers;
                CURR_OWNER_COUNT += si.StoreOwners;
                CURR_MARKET_COUNT += si.MarketManagers;
            }
            Assert.AreEqual(CURR_GUEST_COUNT, EXPECTED_GUEST_COUNT);
            Assert.AreEqual(CURR_MEMBER_COUNT, EXPECTED_MEMBER_COUNT);
            Assert.AreEqual(CURR_MANAGER_COUNT, EXPECTED_MANAGER_COUNT);
            Assert.AreEqual(CURR_OWNER_COUNT, EXPECTED_OWNER_COUNT);
            Assert.AreEqual(CURR_MARKET_COUNT, EXPECTED_MARKET_COUNT);
        }

        [DataTestMethod]
        [DataRow("Aug 22, 1980", "Aug 21, 1990", 1, 0, 0, 0, 0)]
        [DataRow("Aug 22, 1980", "Aug 22, 1990", 2, 0, 0, 0, 0)]
        [DataRow("Aug 22, 1980", "May 22, 2022", 3, 0, 0, 0, 0)]
        [DataRow("Aug 22, 1980", "Jun 14, 2022", 3, 1, 0, 1, 0)]
        [DataRow("Aug 22, 1980", "Jun 15, 2022", 3, 2, 1, 1, 0)]
        [DataRow("Aug 22, 1980", "Jun 16, 2022", 3, 2, 1, 1, 1)]

        [DataRow("Aug 22, 1990", "Aug 22, 1990", 1, 0, 0, 0, 0)]
        [DataRow("Aug 22, 1990", "May 22, 2022", 2, 0, 0, 0, 0)]
        [DataRow("Aug 22, 1990", "Jun 14, 2022", 2, 1, 0, 1, 0)]
        [DataRow("Aug 22, 1990", "Jun 15, 2022", 2, 2, 1, 1, 0)]
        [DataRow("Aug 22, 1990", "Jun 16, 2022", 2, 2, 1, 1, 1)]

        [DataRow("May 21, 2022", "May 22, 2022", 1, 0, 0, 0, 0)]
        [DataRow("May 22, 2022", "Jun 14, 2022", 1, 1, 0, 1, 0)]
        [DataRow("May 22, 2022", "Jun 15, 2022", 1, 2, 1, 1, 0)]
        [DataRow("May 22, 2022", "Jun 16, 2022", 1, 2, 1, 1, 1)]

        [DataRow("Jun 14, 2022", "Jun 14, 2022", 0, 1, 0, 1, 0)]
        [DataRow("Jun 14, 2022", "Jun 15, 2022", 0, 2, 1, 1, 0)]
        [DataRow("Jun 14, 2022", "Jun 16, 2022", 0, 2, 1, 1, 1)]

        [DataRow("Jun 15, 2022", "Jun 15, 2022", 0, 1, 1, 0, 0)]
        [DataRow("Jun 15, 2022", "Jun 16, 2022", 0, 1, 1, 0, 1)]

        [DataRow("Jun 16, 2022", "Jun 16, 2022", 0, 0, 0, 0, 1)]
        public void Test_MarketManagerDailyRangeInformation_Success(string begin, string end, int guest, int member, int manager, int owner, int market)
        {
            // Information:
            // 22/08/1980 - Guest
            // 22/08/1990 - Guest
            // 22/05/22 - Guest
            // 14/06/22 - Store owner
            // 14/06/22 - Member
            // 15/06/22 - Member
            // 15/06/22 - Store manager
            // 16/06/22 - Market manager
            service.EnterMarket(1, DateTime.Parse("May 22, 2022"));
            service.EnterMarket(2, DateTime.Parse("Aug 22, 1980"));
            service.EnterMarket(3, DateTime.Parse("Aug 22, 1990"));

            service.Register(1, "mem1", "pass1", DateTime.Parse("Aug 22, 1972"));
            service.Register(3, "mem2", "pass2", DateTime.Parse("Aug 22, 1972"));

            service.Login(1, "mem1", "pass1", DateTime.Parse("Jun 15, 2022"));
            service.Login(2, "admin", "admin", DateTime.Parse("Jun 16, 2022"));
            service.Login(3, "mem2", "pass2", DateTime.Parse("Jun 14, 2022"));

            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Parse("Jun 14, 2022")).Value;
            service.NominateStoreManager(1, "mem1", "mem2", st.StoreId, DateTime.Parse("Jun 15, 2022"));

            Response<List<StatisticsInformation>> resp = service.MarketManagerDailyRangeInformation(2, "admin", DateTime.Parse(begin), DateTime.Parse(end));
            Assert.IsFalse(resp.ErrorOccured);
            Assert.IsNotNull(resp.Value);
            ValidateDailyRangeInformation(resp.Value, DateTime.Parse(begin), DateTime.Parse(end), guest, member, manager, owner, market);
        }

        public void Test_MarketManagerDailyInformation_Failure_NotMarketManager()
        {
            service = new Service(externalSystem.Object, "admin~NOTYOU~admin~22/08/1972");

            service.EnterMarket(1, DateTime.Parse("May 22, 2022"));
            service.EnterMarket(2, DateTime.Parse("Aug 22, 1980"));
            service.EnterMarket(3, DateTime.Parse("Aug 22, 1990"));

            service.Register(1, "mem1", "pass1", DateTime.Parse("Aug 22, 1972"));
            service.Register(2, "admin", "admin", DateTime.Parse("Aug 22, 1972"));
            service.Register(3, "mem2", "pass2", DateTime.Parse("Aug 22, 1972"));

            service.Login(1, "mem1", "pass1", DateTime.Parse("Jun 15, 2022"));
            service.Login(2, "admin", "admin", DateTime.Parse("Jun 16, 2022"));
            service.Login(3, "mem2", "pass2", DateTime.Parse("Jun 14, 2022"));

            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Parse("Jun 14, 2022")).Value;
            service.NominateStoreManager(1, "mem1", "mem2", st.StoreId, DateTime.Parse("Jun 15, 2022"));

            Response<List<StatisticsInformation>> resp = service.MarketManagerDailyRangeInformation(2, "admin", DateTime.Parse("Jun 14, 2022"), DateTime.Parse("Jun 15, 2022"));
            Assert.IsTrue(resp.ErrorOccured);
            Assert.IsNotNull(resp.Value);
        }

        [DataTestMethod]
        [DataRow("Aug 22, 1970", "Aug 21, 1980")]
        [DataRow("Jun 17, 2022", "Jun 17, 2022")]
        public void Test_MarketManagerDailyInformation_Empty(string begin, string end)
        {
            // Information:
            // 22/08/1980 - Guest
            // 22/08/1990 - Guest
            // 22/05/22 - Guest
            // 14/06/22 - Store owner
            // 14/06/22 - Member
            // 15/06/22 - Member
            // 15/06/22 - Store manager
            // 16/06/22 - Market manager

            service.EnterMarket(1, DateTime.Parse("May 22, 2022"));
            service.EnterMarket(2, DateTime.Parse("Aug 22, 1980"));
            service.EnterMarket(3, DateTime.Parse("Aug 22, 1990"));

            service.Register(1, "mem1", "pass1", DateTime.Parse("Aug 22, 1972"));
            service.Register(3, "mem2", "pass2", DateTime.Parse("Aug 22, 1972"));

            service.Login(1, "mem1", "pass1", DateTime.Parse("Jun 15, 2022"));
            service.Login(2, "admin", "admin", DateTime.Parse("Jun 16, 2022"));
            service.Login(3, "mem2", "pass2", DateTime.Parse("Jun 14, 2022"));

            Store st = service.CreateNewStore(1, "mem1", "s1", DateTime.Parse("Jun 14, 2022")).Value;
            service.NominateStoreManager(1, "mem1", "mem2", st.StoreId, DateTime.Parse("Jun 15, 2022"));

            Response<List<StatisticsInformation>> resp = service.MarketManagerDailyRangeInformation(2, "admin", DateTime.Parse(begin), DateTime.Parse(end));
            Assert.IsFalse(resp.ErrorOccured);
            Assert.IsNotNull(resp.Value);
            ValidateDailyRangeInformation(resp.Value, DateTime.Parse(begin), DateTime.Parse(end), 0, 0, 0, 0, 0);
        }

        [TestMethod]
        public void Test_GetStorePurhcaseHistory_Success()
        {
            string member1 = "member1";
            Test_Login_Good(1, member1, "password1");
            DateTime PURCHASE_DATE = DateTime.Now;

            Store store = service.CreateNewStore(1, member1, "Store1", PURCHASE_DATE).Value;
            Product p1 = service.AddProduct(1, member1, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member1, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member1, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member1, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member1, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(50)("Cat1"), "Cat1");
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(10)("Cat2"), "Cat2");
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 615;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, PURCHASE_DATE).Value);

            Response<List<Order>> adminResponse = service.GetStorePurchaseHistory(1, member1, store.StoreId);
            Assert.IsFalse(adminResponse.ErrorOccured);
            List<Order> orderHistory = adminResponse.Value;
            Assert.IsNotNull(orderHistory);
            Assert.AreEqual(1, orderHistory.Count);
            Assert.AreEqual(PURCHASE_DATE.ToShortDateString(), orderHistory[0].Date);
            Assert.AreEqual(store.StoreId, orderHistory[0].StoreId);
            Assert.AreEqual("member1", orderHistory[0].BuyerName);
            Assert.AreEqual(address, orderHistory[0].Address);
            Assert.AreEqual(expected_price, orderHistory[0].Price);
            Assert.AreEqual(3, orderHistory[0].Products.Count);
            Assert.IsTrue(orderHistory[0].Products.Select(p => p.Id).Contains(p1.Id));
            Assert.IsTrue(orderHistory[0].Products.Select(p => p.Id).Contains(p2.Id));
            Assert.IsTrue(orderHistory[0].Products.Select(p => p.Id).Contains(p3.Id));

            Test_Login_Good(2, "admin", "admin");
            List<Order> orderHistory1 = service.GetStorePurchaseHistory(2, "admin", store.StoreId).Value;
            Assert.IsNotNull(orderHistory1);
            Assert.AreEqual(1, orderHistory1.Count);
            Assert.AreEqual(PURCHASE_DATE.ToShortDateString(), orderHistory1[0].Date);
            Assert.AreEqual(store.StoreId, orderHistory1[0].StoreId);
            Assert.AreEqual("member1", orderHistory1[0].BuyerName);
            Assert.AreEqual(address, orderHistory1[0].Address);
            Assert.AreEqual(expected_price, orderHistory1[0].Price);
            Assert.AreEqual(3, orderHistory1[0].Products.Count);
            Assert.IsTrue(orderHistory1[0].Products.Select(p => p.Id).Contains(p1.Id));
            Assert.IsTrue(orderHistory1[0].Products.Select(p => p.Id).Contains(p2.Id));
            Assert.IsTrue(orderHistory1[0].Products.Select(p => p.Id).Contains(p3.Id));
        }

        [TestMethod]
        public void Test_GetStorePurhcaseHistory_Success_MultipleOrders_MultipleStores()
        {
            string member1 = "member1";
            Test_Login_Good(1, member1, "password1");
            DateTime PURCHASE_DATE = DateTime.Now;

            Store store = service.CreateNewStore(1, member1, "Store1", PURCHASE_DATE).Value;
            Product p1 = service.AddProduct(1, member1, store.StoreId, "Product1", "Description1", 100.0, 6, "no_cat").Value;
            Product p2 = service.AddProduct(1, member1, store.StoreId, "Product2", "Description2", 200.0, 4, "Cat1").Value;
            Product p3 = service.AddProduct(1, member1, store.StoreId, "Product3", "Description3", 50.0, 10, "Cat2").Value;
            service.AddProductDiscount(1, member1, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member1, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(50)("Cat1"), "Cat1");
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(10)("Cat2"), "Cat2");


            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 615;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, PURCHASE_DATE).Value);

            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            Assert.IsFalse(service.BuyCart(1, cc, address, PURCHASE_DATE).ErrorOccured);

            // NOT INCLUDED IN HISTORY, NOT THE SAME STORE!
            Store store2 = service.CreateNewStore(1, member1, "Store2", PURCHASE_DATE).Value;
            Product p4 = service.AddProduct(1, member1, store2.StoreId, "Product4", "Description4", 100.0, 3, "no_cat").Value;
            Product p5 = service.AddProduct(1, member1, store2.StoreId, "Product5", "Description5", 200.0, 2, "Cat1").Value;
            Product p6 = service.AddProduct(1, member1, store2.StoreId, "Product6", "Description6", 50.0, 5, "Cat2").Value;
            service.AddToCart(1, p4.Id, store2.StoreId, 3);
            service.AddToCart(1, p5.Id, store2.StoreId, 2);
            service.AddToCart(1, p6.Id, store2.StoreId, 5);
            Assert.IsFalse(service.BuyCart(1, cc, address, DateTime.Now).ErrorOccured);
            // NOT INCLUDED IN HISTORY, NOT THE SAME STORE!


            Response<List<Order>> adminResponse = service.GetStorePurchaseHistory(1, member1, store.StoreId);
            Assert.IsFalse(adminResponse.ErrorOccured);
            List<Order> orderHistory = adminResponse.Value;
            Assert.IsNotNull(orderHistory);
            Assert.AreEqual(2, orderHistory.Count);

            Test_Login_Good(2, "admin", "admin");
            List<Order> orderHistory1 = service.GetStorePurchaseHistory(2, "admin", store.StoreId).Value;
            Assert.IsNotNull(orderHistory1);
            Assert.AreEqual(2, orderHistory.Count);
        }

        [TestMethod]
        public void Test_GetStorePurhcaseHistory_Failure_NoPermissions()
        {
            string member1 = "member1";
            Test_Login_Good(1, member1, "password1");
            DateTime PURCHASE_DATE = DateTime.Now;

            Store store = service.CreateNewStore(1, member1, "Store1", PURCHASE_DATE).Value;
            Product p1 = service.AddProduct(1, member1, store.StoreId, "Product1", "Description1", 100.0, 3, "no_cat").Value;
            Product p2 = service.AddProduct(1, member1, store.StoreId, "Product2", "Description2", 200.0, 2, "Cat1").Value;
            Product p3 = service.AddProduct(1, member1, store.StoreId, "Product3", "Description3", 50.0, 5, "Cat2").Value;
            service.AddProductDiscount(1, member1, store.StoreId, makeSimpleProductDiscount(10)(p1.Id), p1.Id);
            service.AddProductDiscount(1, member1, store.StoreId, makeSimpleProductDiscount(20)(p2.Id), p2.Id);
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(50)("Cat1"), "Cat1");
            service.AddCategoryDiscount(1, member1, store.StoreId, makeSimpleCategoryDiscount(10)("Cat2"), "Cat2");
            service.AddToCart(1, p1.Id, store.StoreId, 3);
            service.AddToCart(1, p2.Id, store.StoreId, 2);
            service.AddToCart(1, p3.Id, store.StoreId, 5);
            double expected_price = 615;
            Assert.AreEqual(expected_price, service.BuyCart(1, cc, address, DateTime.Now).Value);

            Test_Login_Good(2, "member2", "pass");
            Response<List<Order>> response = service.GetStorePurchaseHistory(2, "member2", store.StoreId);
            Assert.IsTrue(response.ErrorOccured);
        }

        [TestMethod]
        public void Test_OfferBid_Success_AndCheckOwnersAndManagersReceiveNotification_AndCanCheckBidsStatus()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);
        }

        [TestMethod]
        public void Test_CantBuyWithoutAcceptedBidFromAllOwners()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;
            Assert.IsTrue(service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).ErrorOccured);

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);

            Assert.IsTrue(service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_ReceiveNotificationForDecliningBid_Holded()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;
            service.Logout(4, "member4");
            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, false).ErrorOccured);

            Assert.AreEqual(1, service.Login(4, "member4", "pass4", DateTime.Now).Value.Value.Count);
        }


        [TestMethod]
        public void Test_ReceiveNotificationForDecliningBid()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, false).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(4, "member4").Value.Count);
        }

        [TestMethod]
        public void Test_ReceiveNotificationForAcceptingBid_Holded()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(0, service.TakeNotifications(4, "member4").Value.Count);

            service.Logout(4, "member4");
            Assert.IsFalse(service.VoteForBid(2, "member2", store.StoreId, bidId, true).ErrorOccured);

            Assert.AreEqual(1, service.Login(4, "member4", "pass4", DateTime.Now).Value.Value.Count);
        }

        [TestMethod]
        public void Test_ReceiveNotificationForAcceptingBid()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(0, service.TakeNotifications(4, "member4").Value.Count);
            Assert.IsFalse(service.VoteForBid(2, "member2", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(4, "member4").Value.Count);
        }

        [TestMethod]
        public void Test_CanBuyAfterAccepting()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(0, service.TakeNotifications(4, "member4").Value.Count);
            Assert.IsFalse(service.VoteForBid(2, "member2", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(4, "member4").Value.Count);
            Assert.AreEqual(8.0, service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).Value);
        }

        [TestMethod]
        public void Test_CantBuyAfterDeclining()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(0, service.TakeNotifications(4, "member4").Value.Count);
            Assert.IsFalse(service.VoteForBid(2, "member2", store.StoreId, bidId, false).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(4, "member4").Value.Count);
            Assert.IsTrue(service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).ErrorOccured);
        }


        [TestMethod]
        public void Test_CantBuyTwiceAfterBidAccepting()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(0, service.TakeNotifications(4, "member4").Value.Count);
            Assert.IsFalse(service.VoteForBid(2, "member2", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(4, "member4").Value.Count);
            Assert.AreEqual(8.0, service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).Value);
            Assert.IsTrue(service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).ErrorOccured);
        }

        [TestMethod]
        public void Test_CanBuyAfterPriceOfferFromOwner()
        {
            for (int i = 1; i < 5; i++)
            {
                Test_Login_Good(i, $"member{i}", $"pass{i}");
            }
            Store store = service.CreateNewStore(1, "member1", "Store1", DateTime.Now).Value;
            Product product = service.AddProduct(1, "member1", store.StoreId, "Product1", "Description1", 15.0, 3, "Category1").Value;
            service.NominateStoreOwner(1, "member1", "member2", store.StoreId, DateTime.Now);
            service.NominateStoreManager(1, "member1", "member3", store.StoreId, DateTime.Now);

            service.OfferBid(4, "member4", store.StoreId, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, service.TakeNotifications(i, $"member{i}").Value.Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = service.GetBidsStatus(1, "member1", store.StoreId).Value;
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.StoreId, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.IsTrue(service.GetBidsStatus(3, "member3", store.StoreId).ErrorOccured);

            int bidId = bid.BidId;

            Assert.IsFalse(service.VoteForBid(1, "member1", store.StoreId, bidId, true).ErrorOccured);
            Assert.AreEqual(0, service.TakeNotifications(4, "member4").Value.Count);
            Assert.IsFalse(service.CounterBid(2, "member2", store.StoreId, bidId, 10.0).ErrorOccured);
            Assert.AreEqual(1, service.TakeNotifications(4, "member4").Value.Count);
            Assert.AreEqual(10.0, service.BuyBidProduct(4, "member4", store.StoreId, bidId, cc, address, DateTime.Now).Value);
        }

        [TestMethod]
        public void Test_PersistenceOnUserCart()
        {
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            Test_Login_Good(1, "member1", "pass1");
            Test_Login_Good(2, "member2", "pass2");
            Store st = service.CreateNewStore(1, "member1", "S1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "member1", st.StoreId, "Prod1", "Desc1", 10.0, 5, "Cat1").Value;
            service.AddToCart(2, p.Id, st.StoreId, 3);
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            service.EnterMarket(2, DateTime.Now);
            service.Login(2, "member2", "pass2", DateTime.Now);
            ShoppingCart sc = service.ViewCart(2).Value;
            Assert.IsNotNull(sc);
            Assert.AreEqual(1, sc.ShoppingBags.Count);
            Assert.AreEqual(1, sc.ShoppingBags.First().Products.Count);
            Assert.AreEqual(30.0, sc.Price);
        }

        [TestMethod]
        public void Test_PersistenceOnStore()
        {
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            Test_Login_Good(1, "member1", "pass1");
            Store st = service.CreateNewStore(1, "member1", "S1", DateTime.Now).Value;
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            service.EnterMarket(1, DateTime.Now);
            service.Login(1, "member1", "pass1", DateTime.Now);
            List<Store> stores = service.GetAllStores(1).Value;
            Assert.AreEqual(1, stores.Count);
            Assert.AreEqual(st.StoreId, stores.First().StoreId);
            Assert.AreEqual(st.Name, stores.First().Name);
        }

        [TestMethod]
        public void Test_PersistenceOnProduct()
        {
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            Test_Login_Good(1, "member1", "pass1");
            Store st = service.CreateNewStore(1, "member1", "S1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "member1", st.StoreId, "Prod1", "Desc1", 10.0, 5, "Cat1").Value;
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            service.EnterMarket(1, DateTime.Now);
            service.Login(1, "member1", "pass1", DateTime.Now);
            List<Store> stores = service.GetAllStores(1).Value;
            Assert.AreEqual(1, stores.Count);
            Assert.AreEqual(st.StoreId, stores.First().StoreId);
            Assert.AreEqual(st.Name, stores.First().Name);
            Assert.AreEqual(p.Id, stores.First().Products.First().Id);
        }

        [TestMethod]
        public void Test_PersistenceOnStoreOwnerVoting()
        {
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            Test_Login_Good(1, "member1", "pass1");
            Test_Login_Good(2, "member2", "pass2");
            Test_Login_Good(3, "member3", "pass3");
            Store st = service.CreateNewStore(1, "member1", "pass1", DateTime.Now).Value;
            service.NominateStoreOwner(1, "member1", "member2", st.StoreId, DateTime.Now);
            service.NominateStoreOwner(1, "member1", "member3", st.StoreId, DateTime.Now);
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            service.EnterMarket(2, DateTime.Now);
            service.Login(2, "member2", "pass2", DateTime.Now);
            Response<StoreOwner> rst = service.NominateStoreOwner(2, "member2", "member3", st.StoreId, DateTime.Now);
            Assert.IsFalse(rst.ErrorOccured);
            Assert.IsNotNull(rst.Value);
        }

        [TestMethod]
        public void Test_PersistenceOnBidding()
        {
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            Test_Login_Good(1, "member1", "pass1");
            Test_Login_Good(2, "member2", "pass2");
            Test_Login_Good(3, "member3", "pass3");
            Store st = service.CreateNewStore(1, "member1", "pass1", DateTime.Now).Value;
            Product p = service.AddProduct(1, "member1", st.StoreId, "P1", "D1", 10.0, 5, "Cat1").Value;
            service.NominateStoreOwner(1, "member1", "member2", st.StoreId, DateTime.Now);
            Bid b = service.OfferBid(3, "member3", st.StoreId, p.Id, 7.0).Value;
            service.VoteForBid(2, "member2", st.StoreId, b.BidId, true);
            service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\ndb");
            service.EnterMarket(1, DateTime.Now);
            service.Login(1, "member1", "pass1", DateTime.Now);
            Response<Bid> rb = service.VoteForBid(1, "member1", st.StoreId, p.Id, true);
            Assert.IsFalse(rb.ErrorOccured);
            Assert.IsNotNull(rb.Value);
        }
    }
}
