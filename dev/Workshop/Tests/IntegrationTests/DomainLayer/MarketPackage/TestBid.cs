using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.MarketPackage.Biding;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;
using SystemAdminDTO = Workshop.ServiceLayer.ServiceObjects.SystemAdminDTO;
using IExternalSystem = Workshop.ServiceLayer.IExternalSystem;
using ExternalSystem = Workshop.ServiceLayer.ExternalSystem;
namespace Tests.IntegrationTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestBid
    {
        private UserController userController;
        private MarketController marketController;
        private SupplyAddress address = new SupplyAddress("Ronmi", "Mayor 1", "Ashkelon", "Israel", "784112");
        private CreditCard cc = new CreditCard("001122334455667788", "11", "26", "LeBron Michal", "555", "208143751");
        private Store store;
        private Product product;

        [TestInitialize]
        public void Setup()
        {
            ISecurityHandler security = new HashSecurityHandler();
            IReviewHandler review = new ReviewHandler();
            SystemAdminDTO adminDTO = new SystemAdminDTO("adm", "adm", "14/06/2022");
            List<SystemAdminDTO> adms = new List<SystemAdminDTO>();
            adms.Add(adminDTO);
            userController = new UserController(security, review, adms);
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Random().Next(10000, 100000));
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Random().Next(10000, 100000));
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Returns(true);

            marketController = new MarketController(userController, externalSystem.Object);
            marketController.InitializeSystem();

            for (int i = 1; i < 5; i++)
            {
                userController.EnterMarket(i, DateTime.Now);
                userController.Register(i, $"member{i}", $"pass{i}", DateTime.Parse("Aug 22, 1972"));
                userController.Login(i, $"member{i}", $"pass{i}", DateTime.Now);
            }
            store = marketController.CreateNewStore(1, "member1", "Store1", DateTime.Now);
            product = marketController.AddProductToStore(1, "member1", store.GetId(), "Product1", "Description1", 15.0, 3, "Category1");
            marketController.NominateStoreOwner(1, "member1", "member2", store.id, DateTime.Now);
            marketController.NominateStoreManager(1, "member1", "member3", store.id, DateTime.Now);
        }

        [TestMethod]
        public int Test_OfferBid_Success_AndCheckOwnersAndManagersReceiveNotification_AndCanCheckBidsStatus()
        {
            marketController.OfferBid(4, "member4", store.id, product.Id, 8.0);

            // Make sure the store owners and managers were notified

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(1, userController.TakeNotifications(i, $"member{i}").Count);
            }

            // Make sure the bid was added
            List<Bid> storeBids = marketController.GetBidsStatus(1, "member1", store.id);
            Assert.IsNotNull(storeBids);
            Assert.AreEqual(1, storeBids.Count);
            Bid bid = storeBids[0];

            Assert.AreEqual(8.0, bid.OfferedPrice);
            Assert.AreEqual("member4", bid.OfferingMembername);
            Assert.AreEqual(product.Id, bid.Product.Id);
            Assert.AreEqual(store.id, bid.StoreId);
            Assert.AreEqual(0, bid.OwnerVotes.Count);

            Assert.ThrowsException<ArgumentException>(() => marketController.GetBidsStatus(3, "member3", store.id));
            return bid.BidId;
        }

        [TestMethod]
        public void Test_CantBuyWithoutAcceptedBidFromAllOwners()
        {
            int bidId = Test_OfferBid_Success_AndCheckOwnersAndManagersReceiveNotification_AndCanCheckBidsStatus();
            Assert.ThrowsException<ArgumentException>(() => marketController.BuyBidProduct(4, "member4", store.id, bidId, cc, address, DateTime.Now));

            marketController.VoteForBid(1, "member1", store.id, bidId, true);
            Assert.ThrowsException<ArgumentException>(() => marketController.BuyBidProduct(4, "member4", store.id, bidId, cc, address, DateTime.Now));
        }

        [TestMethod]
        public void Test_ReceiveNotificationForDecliningBid()
        {
            int bidId = Test_OfferBid_Success_AndCheckOwnersAndManagersReceiveNotification_AndCanCheckBidsStatus();
            marketController.VoteForBid(1, "member1", store.id, bidId, false);
            Assert.AreEqual(1, userController.TakeNotifications(4, "member4").Count);
        }

        [TestMethod]
        public int Test_ReceiveNotificationForAcceptingBid()
        {
            int bidId = Test_OfferBid_Success_AndCheckOwnersAndManagersReceiveNotification_AndCanCheckBidsStatus();
            marketController.VoteForBid(1, "member1", store.id, bidId, true);
            Assert.AreEqual(0, userController.TakeNotifications(4, "member4").Count);
            marketController.VoteForBid(2, "member2", store.id, bidId, true);
            Assert.AreEqual(1, userController.TakeNotifications(4, "member4").Count);
            return bidId;
        }

        [TestMethod]
        public int Test_CanBuyAfterAccepting()
        {
            int bidId = Test_ReceiveNotificationForAcceptingBid();
            Assert.AreEqual(8.0, marketController.BuyBidProduct(4, "member4", store.id, bidId, cc, address, DateTime.Now));
            return bidId;
        }

        [TestMethod]
        public void Test_CantBuyTwice()
        {
            int bidId = Test_CanBuyAfterAccepting();
            Assert.ThrowsException<KeyNotFoundException>(() => marketController.BuyBidProduct(4, "member4", store.id, bidId, cc, address, DateTime.Now));
        }
    }
}
