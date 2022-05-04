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

        private static string credentials = "Good";

        [TestInitialize]
        public void InitSystem()
        {
            service = new Service();
        }
        
        [TestMethod]
        public void TestSystemInitiation()
        {
            Assert.IsNotNull(service);
            //Assert.NotNull(srv.getSystemmanager()); //Further tests will come later on
            //Assert.NotNull(srv.getExternalConnections());
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
        public void TestExitMarket_Bad_NotEntered()
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

        [TestMethod]
        public void TestRegister_Good()
        {
            
            service.EnterMarket();
            Assert.IsFalse(service.Register(credentials, credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestRegister_Bad1()
        {
            
            service.Register(credentials, credentials);
            Assert.IsTrue(service.Register("TestRegister_Bad1", credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestRegister_Bad2()
        {
            
            service.Register(credentials, credentials);
            Assert.IsTrue(service.Register(credentials, "TestRegister_Bad2").ErrorOccured);
        }

        [TestMethod]
        public void TestRegister_Bad3()
        {
            
            service.Register(credentials, credentials);
            Assert.IsTrue(service.Register(credentials, credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestLogin_Good()
        {
            
            service.Register(credentials, credentials);
            Response<Member> rMember = service.Login(credentials, credentials);
            Assert.IsFalse(rMember.ErrorOccured);
            Assert.IsNotNull(rMember.Value);
        }

        [TestMethod]
        public void TestLogin_Bad1()
        {
            
            Assert.IsTrue(service.Login("Fake1", "Fake2").ErrorOccured);
        }

        [TestMethod]
        public void TestLogin_Bad2()
        {
            
            service.Register(credentials, credentials);
            Assert.IsTrue(service.Login(credentials, "Fake2").ErrorOccured);
        }

        [TestMethod]
        public void TestLogin_Bad3()
        {
            
            service.Register(credentials, credentials);
            Assert.IsTrue(service.Login("Fake1", credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestLogout_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            Assert.IsFalse(service.Logout(credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestLogout_Bad1()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            Assert.IsFalse(service.Logout(credentials).ErrorOccured);
            Assert.IsTrue(service.Logout(credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestLogout_Bad2()
        {
            
            Assert.IsTrue(service.Logout("TestLogout_Bad2").ErrorOccured);
        }

        [TestMethod]
        public void TestLogout_Bad3()
        {
            
            Assert.IsTrue(service.Logout(null).ErrorOccured);
        }

        /*
         * Commenting out tests that don't compile, until presler will fix them

        [TestMethod]
        public void TestAddProduct_Good()
        {
            
            int storeId = 0;
            storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.IsFalse(service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1).ErrorOccured);
        }

        [TestMethod]
        public void TestAddProduct_Bad()
        {
            
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.IsFalse(service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1).ErrorOccured);
            Assert.IsTrue(service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1).ErrorOccured);
        }

        [TestMethod]
        public void TestNominateStoreOwner_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            Assert.IsFalse(service.NominateStoreOwner(credentials, "Who?", storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestNominateStoreOwner_Bad()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            service.NominateStoreOwner(credentials, "Who?", storeId);
            Assert.IsTrue(service.NominateStoreOwner(credentials, "Who?", storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestNominateStoreManager_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            Assert.IsFalse(service.NominateStoreManager(credentials, "Who?", storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestNominateStoreManager_Bad()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            service.NominateStoreManager(credentials, "Who?", storeId);
            Assert.IsTrue(service.NominateStoreManager(credentials, "Who?", storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestGetWorkersInformation_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.IsFalse(service.GetWorkersInformation(credentials, storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestGetWorkersInformation_Bad()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            service.Register("Who?", "Who?");
            service.Login("Who?", "Who?");
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.IsTrue(service.GetWorkersInformation("Who?", storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestCloseStore_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.IsFalse(service.CloseStore(credentials, storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestCloseStore_Bad()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            service.Register("Who?", "Who?");
            service.Login("Who?", "Who?");
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.IsFalse(service.CloseStore("Who?", storeId).ErrorOccured);
        }

        [TestMethod]
        public void TestCreateNewStore_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            Response<int> res = service.CreateNewStore(credentials, credentials);
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(res.Value >= 0);
        }

        [TestMethod]
        public void TestCreateNewStore_Bad()
        {
            
            Assert.IsFalse(service.CreateNewStore(credentials, credentials).ErrorOccured);
        }

        [TestMethod]
        public void TestReviewProduct_Good()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1);
            Assert.IsFalse(service.ReviewProduct(credentials, 0, "Blank").ErrorOccured);
        }

        [TestMethod]
        public void TestReviewProduct_Bad1()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1);
            service.Logout(credentials);
            Assert.IsTrue(service.ReviewProduct(credentials, 0, "Blank").ErrorOccured);
        }

        [TestMethod]
        public void TestReviewProduct_Bad2()
        {
            
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1);
            Assert.IsTrue(service.ReviewProduct(credentials, -1, "Blank").ErrorOccured);
        }
        */
        
    }
}
