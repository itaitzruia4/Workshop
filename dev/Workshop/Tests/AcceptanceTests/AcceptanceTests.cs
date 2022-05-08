using System;
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
            Response<User> ru = service.EnterMarket();
            Assert.IsFalse(ru.ErrorOccured);
            Assert.IsNotNull(ru.Value);
        }

        [TestMethod]
        public void TestEnterMarket_Bad()
        {
            Assert.IsFalse(service.EnterMarket().ErrorOccured);
            Assert.IsTrue(service.EnterMarket().ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Good()
        {
            service.EnterMarket();
            Assert.IsFalse(service.ExitMarket().ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Bad1()
        {
            Assert.IsTrue(service.ExitMarket().ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Bad2()
        {
            service.EnterMarket();
            Assert.IsFalse(service.ExitMarket().ErrorOccured);
            Assert.IsTrue(service.ExitMarket().ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestRegister_Good(string username, string password)
        {
            service.EnterMarket();
            Assert.IsFalse(service.Register(username, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        [DataRow("TestRegister_Bad", password)]
        [DataRow(username, "TestRegister_Bad")]
        public void TestRegister_Bad(string username, string password)
        {
            service.Register(username, password);
            Assert.IsTrue(service.Register(username, password).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogin_Good(string username, string password)
        {
            service.EnterMarket();
            service.Register(username, password);
            Response<Member> rMember = service.Login(username, password);
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
            Assert.IsTrue(service.Login(username, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, username)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestLogin_Bad_WrongPassword(string username, string password, string wrongPassword)
        {
            service.Register(username, password);
            Assert.IsTrue(service.Login(username, wrongPassword).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Fake")]
        [DataRow(username, password, password)]
        [DataRow(username, password, "")]
        [DataRow(username, password, null)]
        public void TestLogin_Bad_WrongUsername(string username, string password, string wrongUsername)
        {
            service.Register(username, password);
            Assert.IsTrue(service.Login(wrongUsername, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogout_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            Assert.IsFalse(service.Logout(username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogout_Bad_LogoutTwice(string username, string password)
        {
            TestLogin_Good(username, password);
            Assert.IsFalse(service.Logout(username).ErrorOccured);
            Assert.IsTrue(service.Logout(username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username)]
        [DataRow(null)]
        [DataRow("")]
        public void TestLogout_Bad_NoSuchUser(string username)
        {
            Assert.IsTrue(service.Logout(username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogout_Bad_NotLoggedIn(string username, string password)
        {
            TestRegister_Good(username, password);
            Assert.IsTrue(service.Logout(null).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddProduct_Good(string username, string password)
        {
            int storeId = 0;
            TestLogin_Good(username, password);
            storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.AddProduct(username, storeId, 0, "TestAddProduct", "Good", 1.0, 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddProduct_Bad_AddTwice(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.AddProduct(username, storeId, 0, "TestAddProduct", "Bad", 1, 1).ErrorOccured);
            Assert.IsTrue(service.AddProduct(username, storeId, 0, "TestAddProduct", "Bad", 1, 1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int TestNominateStoreOwner_Good(string username, string password, string nominated)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
            Assert.IsFalse(service.NominateStoreOwner(username, nominated, storeId).ErrorOccured);
            return storeId;
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public void TestNominateStoreOwner_Bad_NominateTwice(string username, string password, string nominated)
        {
            int storeId = TestNominateStoreOwner_Good(username, password, nominated);
            Assert.IsTrue(service.NominateStoreOwner(username, nominated, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestNominateStoreOwner_Bad_NominateUsrself(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.NominateStoreOwner(username, username, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void TestNominateStoreOwner_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
            TestLogin_Good(nominator, nominator);
            Assert.IsFalse(service.NominateStoreOwner(nominator, nominated, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int TestNominateStoreManager_Good(string username, string password, string nominated)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
            Assert.IsFalse(service.NominateStoreManager(username, nominated, storeId).ErrorOccured);
            return storeId;
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public void TestNominateStoreManager_Bad_NominateTwice(string username, string password, string nominated)
        {
            int storeId = TestNominateStoreManager_Good(username, password, nominated);
            Assert.IsTrue(service.NominateStoreManager(username, nominated, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestNominateStoreManager_Bad_NominateUsrself(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.NominateStoreManager(username, username, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void TestNominateStoreManager_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
            TestLogin_Good(nominator, nominator);
            Assert.IsFalse(service.NominateStoreManager(nominator, nominated, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestGetWorkersInformation_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.GetWorkersInformation(username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void TestGetWorkersInformation_Bad(string username, string password, string npUser)
        {
            TestLogin_Good(username, password);
            TestLogin_Good(npUser, npUser);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.GetWorkersInformation(npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestCloseStore_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.CloseStore(username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void TestCloseStore_Bad(string username, string password, string npUser)
        {
            TestLogin_Good(username, password);
            TestLogin_Good(npUser, npUser);
            int storeId = service.CreateNewStore(username, username).Value.StoreId;
            Assert.IsFalse(service.CloseStore(npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public int TestCreateNewStore_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            Response<Store> res = service.CreateNewStore(username, "RandomStore");
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
            Assert.IsTrue(service.CreateNewStore(username, password).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            service.AddProduct(password, storeId, 0, "TestReviewProduct", "Good", 1, 1);
            Assert.IsFalse(service.ReviewProduct(username, 0, "Blank").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_userLoggeedOut(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            service.AddProduct(password, storeId, 0, "TestReviewProduct", "Good", 1, 1);
            service.Logout(username);
            Assert.IsTrue(service.ReviewProduct(username, 0, "Blank").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_noSuchProduct(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(username, "RandomStore").Value.StoreId;
            service.AddProduct(password, storeId, 0, "TestReviewProduct", "Good", 1, 1);
            Assert.IsTrue(service.ReviewProduct(username, 2, "Blank").ErrorOccured);
        }
    }
}
