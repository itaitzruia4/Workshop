using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using Xunit;

namespace Tests.IntegrationTests
{
    internal class SystemTests
    {
        private static string credentials = "Good";

        public IService InitSystem()
        {
            return new Service();
        }

        public void TestSystemInitiation()
        {
            IService service = InitSystem();
            Assert.NotNull(service);
            //Assert.NotNull(srv.getSystemmanager()); //Further tests will come later on
            //Assert.NotNull(srv.getExternalConnections());
        }

        public void TestEnterMarket_Good()
        {
            IService service = InitSystem();
            Response<User> ru = service.EnterMarket();
            Assert.False(ru.ErrorOccured);
            Assert.NotNull(ru.Value);
        }

        public void TestEnterMarket_Bad()
        {
            IService service = InitSystem();
            Assert.False(service.EnterMarket().ErrorOccured);
            Assert.True(service.EnterMarket().ErrorOccured);
        }

        public void TestExitMarket_Good()
        {
            IService service = InitSystem();
            service.EnterMarket();
            Assert.False(service.ExitMarket().ErrorOccured);
        }

        public void TestExitMarket_Bad1()
        {
            IService service = InitSystem();
            Assert.True(service.ExitMarket().ErrorOccured);
        }

        public void TestExitMarket_Bad2()
        {
            IService service = InitSystem();
            service.EnterMarket();
            Assert.False(service.ExitMarket().ErrorOccured);
            Assert.True(service.ExitMarket().ErrorOccured);
        }

        public void TestRegister_Good()
        {
            IService service = InitSystem();
            service.EnterMarket();
            Assert.False(service.Register(credentials, credentials).ErrorOccured);
        }

        public void TestRegister_Bad1()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            Assert.True(service.Register("TestRegister_Bad1", credentials).ErrorOccured);
        }

        public void TestRegister_Bad2()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            Assert.True(service.Register(credentials, "TestRegister_Bad2").ErrorOccured);
        }

        public void TestRegister_Bad3()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            Assert.True(service.Register(credentials, credentials).ErrorOccured);
        }

        public void TestLogin_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            Response<Member> rMember = service.Login(credentials, credentials);
            Assert.False(rMember.ErrorOccured);
            Assert.NotNull(rMember.Value);
        }

        public void TestLogin_Bad1()
        {
            IService service = InitSystem();
            Assert.True(service.Login("Fake1", "Fake2").ErrorOccured);
        }

        public void TestLogin_Bad2()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            Assert.True(service.Login(credentials, "Fake2").ErrorOccured);
        }

        public void TestLogin_Bad3()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            Assert.True(service.Login("Fake1", credentials).ErrorOccured);
        }

        public void TestLogout_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            Assert.False(service.Logout(credentials).ErrorOccured);
        }

        public void TestLogout_Bad1()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            Assert.False(service.Logout(credentials).ErrorOccured);
            Assert.True(service.Logout(credentials).ErrorOccured);
        }

        public void TestLogout_Bad2()
        {
            IService service = InitSystem();
            Assert.True(service.Logout("TestLogout_Bad2").ErrorOccured);
        }

        public void TestLogout_Bad3()
        {
            IService service = InitSystem();
            Assert.True(service.Logout(null).ErrorOccured);
        }

        public void TestAddProduct_Good()
        {
            IService service = InitSystem();
            int storeId = 0;
            storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.False(service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1).ErrorOccured);
        }

        public void TestAddProduct_Bad()
        {
            IService service = InitSystem();
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.False(service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1).ErrorOccured);
            Assert.True(service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1).ErrorOccured);
        }

        public void TestNominateStoreOwner_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            Assert.False(service.NominateStoreOwner(credentials, "Who?", storeId).ErrorOccured);
        }

        public void TestNominateStoreOwner_Bad()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            service.NominateStoreOwner(credentials, "Who?", storeId);
            Assert.True(service.NominateStoreOwner(credentials, "Who?", storeId).ErrorOccured);
        }

        public void TestNominateStoreManager_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            Assert.False(service.NominateStoreManager(credentials, "Who?", storeId).ErrorOccured);
        }

        public void TestNominateStoreManager_Bad()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.Register("Who?", "Who?");
            service.NominateStoreManager(credentials, "Who?", storeId);
            Assert.True(service.NominateStoreManager(credentials, "Who?", storeId).ErrorOccured);
        }

        public void TestGetWorkersInformation_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.False(service.GetWorkersInformation(credentials, storeId).ErrorOccured);
        }

        public void TestGetWorkersInformation_Bad()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            service.Register("Who?", "Who?");
            service.Login("Who?", "Who?");
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.True(service.GetWorkersInformation("Who?", storeId).ErrorOccured);
        }

        public void TestCloseStore_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.False(service.CloseStore(credentials, storeId).ErrorOccured);
        }

        public void TestCloseStore_Bad()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            service.Register("Who?", "Who?");
            service.Login("Who?", "Who?");
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            Assert.False(service.CloseStore("Who?", storeId).ErrorOccured);
        }

        public void TestCreateNewStore_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            Response<int> res = service.CreateNewStore(credentials, credentials);
            Assert.False(res.ErrorOccured);
            Assert.True(res.Value >= 0);
        }

        public void TestCreateNewStore_Bad()
        {
            IService service = InitSystem();
            Assert.False(service.CreateNewStore(credentials, credentials).ErrorOccured);
        }
        public void TestReviewProduct_Good()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1);
            Assert.False(service.ReviewProduct(credentials, 0, "Blank").ErrorOccured);
        }

        public void TestReviewProduct_Bad1()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1);
            service.Logout(credentials);
            Assert.True(service.ReviewProduct(credentials, 0, "Blank").ErrorOccured);
        }

        public void TestReviewProduct_Bad2()
        {
            IService service = InitSystem();
            service.Register(credentials, credentials);
            service.Login(credentials, credentials);
            int storeId = service.CreateNewStore(credentials, credentials).Value;
            service.AddProduct(credentials, storeId, 0, credentials, credentials, 1, 1);
            Assert.True(service.ReviewProduct(credentials, -1, "Blank").ErrorOccured);
        }
    }
}
