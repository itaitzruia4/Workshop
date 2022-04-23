using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;

namespace Tests.DomainLayer.UserPackage
{
    [TestClass]
    public class RolesTests
    {
        private ISecurityHandler security;
        private IUserController userController;
        private MarketController marketController;
        private Member member;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            security = securityMock.Object;

            userController = new UserController(security);
            userController.EnterMarket();
            userController.Register("nirdan", "12345");
            this.member = userController.Login("nirdan", "12345");
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
        public void GetStoreOrdersListFail()
        {
            int storeId = 1;
            member.AddRole(new StoreManager(storeId));
            Assert.IsFalse(userController.IsAuthorized(member.Username, storeId, Workshop.DomainLayer.UserPackage.Permissions.Action.GetStoreOrdersList));
        }
    }
}
