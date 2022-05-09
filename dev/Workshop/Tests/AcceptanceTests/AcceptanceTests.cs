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
        public void TestExitMarket_Bad1()
        {
            Assert.IsTrue(service.ExitMarket(1).ErrorOccured);
        }

        [TestMethod]
        public void TestExitMarket_Bad2()
        {
            service.EnterMarket(1);
            Assert.IsFalse(service.ExitMarket(1).ErrorOccured);
            Assert.IsTrue(service.ExitMarket(1).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestRegister_Good(string username, string password)
        {
            service.EnterMarket(1);
            Assert.IsFalse(service.Register(1, username, password).ErrorOccured);
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
        [DataRow(username, password)]
        public void TestLogin_Good(string username, string password)
        {
            service.EnterMarket(1);
            service.Register(1, username, password);
            Response<Member> rMember = service.Login(1, username, password);
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
        [DataRow(username, password)]
        public void TestLogout_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            Assert.IsFalse(service.Logout(1, username).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestLogout_Bad_LogoutTwice(string username, string password)
        {
            TestLogin_Good(username, password);
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
            TestRegister_Good(username, password);
            Assert.IsTrue(service.Logout(1, null).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddProduct_Good(string username, string password)
        {
            int storeId = 0;
            TestLogin_Good(username, password);
            storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.AddProduct(1, username, storeId, 0, "TestAddProduct", "Good", 1.0, 1, "cat1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestAddProduct_Bad_AddTwice(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.AddProduct(1, username, storeId, 0, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
            Assert.IsTrue(service.AddProduct(1, username, storeId, 0, "TestAddProduct", "Bad", 1, 1, "cat1").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int TestNominateStoreOwner_Good(string username, string password, string nominated)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
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
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.NominateStoreOwner(1, username, username, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void TestNominateStoreOwner_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
            TestLogin_Good(nominator, nominator);
            Assert.IsFalse(service.NominateStoreOwner(1, nominator, nominated, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random")]
        public int TestNominateStoreManager_Good(string username, string password, string nominated)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
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
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.NominateStoreManager(1, username, username, storeId).ErrorOccured);
        }


        [DataTestMethod]
        [DataRow(username, password, "Random1", "Random2")]
        public void TestNominateStoreManager_Bad_NominateNoPermission(string username, string password, string nominated, string nominator)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            TestLogin_Good(nominated, nominated);
            TestLogin_Good(nominator, nominator);
            Assert.IsFalse(service.NominateStoreManager(1, nominator, nominated, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestGetWorkersInformation_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.GetWorkersInformation(1, username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void TestGetWorkersInformation_Bad(string username, string password, string npUser)
        {
            TestLogin_Good(username, password);
            TestLogin_Good(npUser, npUser);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsTrue(service.GetWorkersInformation(1, npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestCloseStore_Good(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            Assert.IsFalse(service.CloseStore(1, username, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password, "Who?")]
        public void TestCloseStore_Bad(string username, string password, string npUser)
        {
            TestLogin_Good(username, password);
            TestLogin_Good(npUser, npUser);
            int storeId = service.CreateNewStore(1, username, username).Value.StoreId;
            Assert.IsFalse(service.CloseStore(1, npUser, storeId).ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public int TestCreateNewStore_Good(string username, string password)
        {
            TestLogin_Good(username, password);
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
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, 0, "TestReviewProduct", "Good", 1, 1, "cat1");
            Assert.IsFalse(service.ReviewProduct(1, username, 0, "Blank").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_userLoggeedOut(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, 0, "TestReviewProduct", "Good", 1, 1, "cat1");
            service.Logout(1, username);
            Assert.IsTrue(service.ReviewProduct(1, username, 0, "Blank").ErrorOccured);
        }

        [DataTestMethod]
        [DataRow(username, password)]
        public void TestReviewProduct_Bad_noSuchProduct(string username, string password)
        {
            TestLogin_Good(username, password);
            int storeId = service.CreateNewStore(1, username, "RandomStore").Value.StoreId;
            service.AddProduct(1, password, storeId, 0, "TestReviewProduct", "Good", 1, 1, "cat1");
            Assert.IsTrue(service.ReviewProduct(1, username, 2, "Blank").ErrorOccured);
        }
    }
}
