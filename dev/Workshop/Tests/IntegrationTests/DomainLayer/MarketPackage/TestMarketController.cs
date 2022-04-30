using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;

namespace Tests.IntegrationTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestMarketController
    {
        private UserController userController;
        private MarketController marketController;
        [TestInitialize]
        public void Setup()
        {
            ISecurityHandler security = new HashSecurityHandler();
            IReviewHandler review = new ReviewHandler();
            IPaymentExternalService paymentExternalService = new ProxyPaymentExternalService(null);
            IMarketPaymentService paymentService = new PaymentAdapter(paymentExternalService);

            ISupplyExternalService supplyExternalService = new ProxySupplyExternalService(null);
            IMarketSupplyService supplyService = new SupplyAdapter(supplyExternalService);

            userController = new UserController(security, review);
            marketController = new MarketController(userController, paymentService, supplyService);
            userController.InitializeSystem();
            marketController.InitializeSystem();

            userController.EnterMarket();

            userController.Register("member1", "pass1");
            userController.Login("member1", "pass1");
            marketController.CreateNewStore("member1", "shop1");

            userController.ExitMarket();
        }
        [TestMethod]
        public void TestGetWorkersInformation_Success()
        {
            CollectionAssert.AreEqual(userController.GetWorkers(1), marketController.GetWorkersInformation("member1", 1));
        }

        [TestMethod]
        public void TestGetWorkersInformation_Failure_NoPermission()
        {
            Assert.ThrowsException<MemberAccessException>(() => marketController.GetWorkersInformation("Notallowed Cohen", 1));
        }

        [TestMethod]
        public void TestCreateNewStore_Success()
        {
            int result = marketController.CreateNewStore("User1", "Cool store123");
            Assert.AreEqual(result, 5);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("User1", "")]
        public void TestCreateNewStore_Failure(string username, string storeName)
        {
            Assert.ThrowsException<ArgumentException>(() => marketController.CreateNewStore(username, storeName));
        }

    }
}
