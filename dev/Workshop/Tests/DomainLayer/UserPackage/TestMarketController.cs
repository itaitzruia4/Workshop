using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Security;

namespace Tests
{
    [TestClass]
    public class TestMarketController
    {
        private ISecurityHandler security;
        private MarketController marketController;

        [TestInitialize]
        public void Setup()
        {
            var securityMock = new Mock<ISecurityHandler>();
            securityMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            security = securityMock.Object;

            marketController = new MarketController(security);
        }

        [TestMethod]
        public void Temp_Success()
        {
            bool IHateMyLife = true;
        }
    }
}
