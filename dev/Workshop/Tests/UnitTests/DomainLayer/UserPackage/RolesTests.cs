using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;

namespace Tests.UnitTests.DomainLayer.UserPackage
{
    [TestClass]
    public class RolesTests
    {
        private IUserController userController;
        private Member member;

        [TestInitialize]
        public void Setup()
        {
            // TODO convert to mocks and move the tests to either TestUserController or TestMarketController
            ISecurityHandler security;
            IReviewHandler review;

            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            security = securityMock.Object;

            var reviewMock = new Mock<IReviewHandler>();
            reviewMock.Setup(x => x.AddReview(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()));
            review = reviewMock.Object;

            userController = new UserController(security, review);
            userController.EnterMarket(1);
            userController.Register(1, "nirdan", "12345", DateTime.Parse("Aug 22, 1972"));
            this.member = userController.Login(1, "nirdan", "12345");
        }

        [TestMethod]
        public void GetStoreOrdersListSuccess1()
        {
            int storeId = 1;
            member.AddRole(new StoreOwner(storeId));
            Assert.IsTrue(userController.IsAuthorized(member.Username, storeId, Workshop.DomainLayer.UserPackage.Permissions.Action.GetStoreOrdersList));
        }

        [TestMethod]
        public void GetStoreOrdersListSuccess2()
        {
            int storeId = 1;
            member.AddRole(new StoreFounder(storeId));
            Assert.IsTrue(userController.IsAuthorized(member.Username, storeId, Workshop.DomainLayer.UserPackage.Permissions.Action.GetStoreOrdersList));
        }

        [TestMethod]
        public void GetStoreOrdersListFail_NoAuthorization()
        {
            int storeId = 1;
            member.AddRole(new StoreManager(storeId));
            Assert.IsFalse(userController.IsAuthorized(member.Username, storeId, Workshop.DomainLayer.UserPackage.Permissions.Action.GetStoreOrdersList));
        }
    }
}
