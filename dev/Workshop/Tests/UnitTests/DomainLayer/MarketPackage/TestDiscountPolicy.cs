using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Workshop.DomainLayer.MarketPackage;
using Workshop.ServiceLayer;

namespace Tests.UnitTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestDiscountPolicy
    {

        DiscountPolicy discountPolicy;

        [TestInitialize]
        public void InitSystem()
        {
            var storeMock = new Mock<Store>(1, "Store1" );
            storeMock.Setup(x => x.ProductExists(It.IsAny<int>())).Returns(true);
            discountPolicy = new DiscountPolicy(storeMock.Object);
        }

        public Func<int, string> makeSimpleProductDiscount(double percent)
        {
            Func<int, string> func = id => "{\"tag\": \"SimpleDiscount\",\"priceAction\":" +
                "{\"tag\": \"ProductPriceActionSimple\",\"percentage\":" +
                percent.ToString() + ", \"productId\": " + id.ToString() + "}}";
            return func;
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

        [DataTestMethod]
        [DataRow(30)]
        [DataRow(30.5)]
        [DataRow(0.5)]
        [DataRow(100)]
        public void TestAddProductDiscount_Good_Simple(double percent)
        {
            TestAddProductDiscount_Good(makeSimpleProductDiscount(percent));
        }

        [DataTestMethod]
        [DataRow(30, 30.5)]
        [DataRow(30.5, 30)]
        [DataRow(0.5, 100)]
        [DataRow(100, 0.5)]
        public void TestAddProductDiscount_Good_And(double lPercent, double rPercent)
        {
            TestAddProductDiscount_Good(makeAndproductDiscount(lPercent, rPercent));
        }

        public void TestAddProductDiscount_Good(Func<int, string> discount)
        {
            discountPolicy.AddProductDiscount(discount(1), 1);
        }

        [DataTestMethod]
        [DataRow(30)]
        public void TestAddProductDiscount_Bad_NoSuchProduct(double discount)
        {
            var storeMock = new Mock<Store>(1, "Store1" );
            storeMock.Setup(s => s.ProductExists(It.IsAny<int>())).Returns(false);
            discountPolicy = new DiscountPolicy(storeMock.Object);
            Assert.ThrowsException<Exception>(() => discountPolicy.AddProductDiscount(makeSimpleProductDiscount(discount)(0), 0));
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-1.5)]
        [DataRow(200)]
        public void TestAddProductDiscount_bad_BadDiscount_simple(double percent)
        {
            TestAddProductDiscount_bad_BadDiscount(makeSimpleProductDiscount(percent));
        }

        public void TestAddProductDiscount_bad_BadDiscount(Func<int, string> discount)
        {
            Assert.ThrowsException<Exception>(() => discountPolicy.AddProductDiscount(discount(1), 1));
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: 5,category: 'cat1'}} ")]
        public void TestAddCategoryDiscount_Good(string discount)
        {
            discountPolicy.AddCategoryDiscount(discount, "cat1");
        }

        [DataTestMethod]
        [DataRow("{\"tag\": \"CategoryDiscountSimpleTerm\",\"type\": \"q\",\"action\": \"<\",\"value\": -2,\"category\": \"cat1\"} ")]
        [DataRow("{\"tag\": \"CategoryDiscountSimpleTerm\",\"type\": \"q\",\"action\": \"<\",\"value\": 0.5,\"category\": \"cat1\"} ")]
        public void TestAddCategoryDiscount_bad_WrongParameters(string discount)
        {
            Assert.ThrowsException<Exception>(() => discountPolicy.AddCategoryDiscount(discount, "cat1"));
        }
    }
}
