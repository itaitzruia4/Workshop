using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.ServiceLayer;

namespace Tests.IntegrationTests.DomainLayer.MarketPackage
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

        public Func<int, string> makeConditionalProductDiscount(double percent, int qunatity)
        {
            Func<int, string> func = id => "{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: "+percent.ToString()+", productId: "+id+" }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '>', value: " + qunatity +", productId: " + id.ToString() + "}}";
            return func;
        }

        public Func<string, string> makeSimpleCategoryDiscount(double percent)
        {
            Func<string, string> func = category => "{\"tag\": \"SimpleDiscount\",\"priceAction\":" +
                "{\"tag\": \"CategoryPriceActionSimple\",\"percentage\":" +
                percent.ToString() + ", \"category\": \"" + category + "\"}}";
            return func;
        }

        public string makeSimpleStoreDiscount(double percent)
        {
            return "{tag: 'SimpleDiscount', priceAction: { tag: 'StorePriceActionSimple', percentage: " + percent.ToString() +"}}";
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

        public Func<int, string> makeOrproductDiscount(double lPercent, double rPercent)
        {
            Func<int, string> func = id => "{ \"tag\": \"OrDiscount\",\"lhs\": {\"tag\": \"SimpleDiscount\"," +
                                            "\"priceAction\": {\"tag\": \"ProductPriceActionSimple\"," +
                                            "\"percentage\": " + lPercent.ToString() + ", \"productId\": " + id.ToString() +
                                            "}},\"rhs\": {\"tag\": \"SimpleDiscount\", \"priceAction\": { " +
                                            "\"tag\": \"ProductPriceActionSimple\", \"percentage\": " + rPercent.ToString() +
                                            ",\"productId\": " + id.ToString() + "}}}";

            return func;
        }

        public Func<int, string> makeXorproductDiscount(double lPercent, double rPercent)
        {
            Func<int, string> func = id => "{ \"tag\": \"XorDiscount\",\"lhs\": {\"tag\": \"SimpleDiscount\"," +
                                            "\"priceAction\": {\"tag\": \"ProductPriceActionSimple\"," +
                                            "\"percentage\": " + lPercent.ToString() + ", \"productId\": " + id.ToString() +
                                            "}},\"rhs\": {\"tag\": \"SimpleDiscount\", \"priceAction\": { " +
                                            "\"tag\": \"ProductPriceActionSimple\", \"percentage\": " + rPercent.ToString() +
                                            ",\"productId\": " + id.ToString() + "}}, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: 3, productId: 1}}";

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

        [DataTestMethod]
        [DataRow(30, 30.5)]
        [DataRow(30.5, 30)]
        [DataRow(0.5, 100)]
        [DataRow(100, 0.5)]
        public void TestAddProductDiscount_Good_Or(double lPercent, double rPercent)
        {
            TestAddProductDiscount_Good(makeOrproductDiscount(lPercent, rPercent));
        }

        [DataTestMethod]
        [DataRow(30, 30.5)]
        [DataRow(30.5, 30)]
        [DataRow(0.5, 100)]
        [DataRow(100, 0.5)]
        public void TestAddProductDiscount_Good_Xor(double lPercent, double rPercent)
        {
            TestAddProductDiscount_Good(makeXorproductDiscount(lPercent, rPercent));
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
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: -2, productId: 1}} ")]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '<', value: 0.5, productId: 1}} ")]
        [DataRow("{tag: 'ConditionalDiscount', priceAction: { tag: 'ProductPriceActionSimple', percentage: 10, productId: 1 }, discountTerm: {tag: 'ProductDiscountSimpleTerm', type: 'q', action: '$', value: 3, productId: 1}} ")]
        public void TestAddProductDiscount_bad_WrongParameters(string discount)
        {
            Assert.ThrowsException<Exception>(() => discountPolicy.AddProductDiscount(discount, 1));
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
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'p',action: '<',value: 0.5,category: 'cat1'}} ")]
        public void TestAddCategoryDiscount_Good(string discount)
        {
            discountPolicy.AddCategoryDiscount(discount, "cat1");
        }

        [DataTestMethod]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: -2,category: 'cat1'}} ")]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '<',value: 0.5,category: 'cat1'}} ")]
        [DataRow("{tag: 'ConditionalDiscount',priceAction: {tag: 'CategoryPriceActionSimple',percentage: 10,category: 'cat1'},discountTerm: {tag: 'CategoryDiscountSimpleTerm',type: 'q',action: '$',value: 3,category: 'cat1'}} ")]
        public void TestAddCategoryDiscount_bad_WrongParameters(string discount)
        {
            Assert.ThrowsException<Exception>(() => discountPolicy.AddCategoryDiscount(discount, "cat1"));
        }

        [TestMethod]
        public void TestCalculatePrice_simpleCategoryAndProductsDiscounts()
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_discounts = new string[] { makeSimpleProductDiscount(10)(1), makeSimpleProductDiscount(20)(2) };
            string[] category_discounts = new string[] { makeSimpleCategoryDiscount(50)("Cat1"), makeSimpleCategoryDiscount(10)("Cat2") };
            string[] store_discounts = new string[] { };
            double expected_price = 335;
            for (int i = 1; i <= product_discounts.Length; i++)
            {
                discountPolicy.AddProductDiscount(product_discounts[i-1], i);
            }
            for(int i = 0; i < category_discounts.Length; i++)
            {
                if (i % 2 == 0)
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat1");
                else
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat2");
            }
            for (int i = 0; i < store_discounts.Length; i++)
            {
                discountPolicy.AddStoreDiscount(store_discounts[i]);
            }
            double actual = discountPolicy.CalculateDiscount(shoppingBag);
            Assert.AreEqual(expected_price, actual);
        }

        [TestMethod]
        public void TestCalculatePrice_simpleStoreAndProductsDiscounts()
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_discounts = new string[] { makeSimpleProductDiscount(10)(1), makeSimpleProductDiscount(20)(2) };
            string[] category_discounts = new string[] {};
            string[] store_discounts = new string[] { makeSimpleStoreDiscount(10)};
            double expected_price = 205;
            for (int i = 1; i <= product_discounts.Length; i++)
            {
                discountPolicy.AddProductDiscount(product_discounts[i - 1], i);
            }
            for (int i = 0; i < category_discounts.Length; i++)
            {
                if (i % 2 == 0)
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat1");
                else
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat2");
            }
            for (int i = 0; i < store_discounts.Length; i++)
            {
                discountPolicy.AddStoreDiscount(store_discounts[i]);
            }
            double actual = discountPolicy.CalculateDiscount(shoppingBag);
            Assert.AreEqual(expected_price, actual);
        }

        [TestMethod]
        public void TestCalculatePrice_ConditionalProductDiscounts()
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_discounts = new string[] { makeConditionalProductDiscount(10, 2)(1), makeConditionalProductDiscount(20, 2)(2) };
            string[] category_discounts = new string[] { };
            string[] store_discounts = new string[] { };
            double expected_price = 30;
            for (int i = 1; i <= product_discounts.Length; i++)
            {
                discountPolicy.AddProductDiscount(product_discounts[i - 1], i);
            }
            for (int i = 0; i < category_discounts.Length; i++)
            {
                if (i % 2 == 0)
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat1");
                else
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat2");
            }
            for (int i = 0; i < store_discounts.Length; i++)
            {
                discountPolicy.AddStoreDiscount(store_discounts[i]);
            }
            double actual = discountPolicy.CalculateDiscount(shoppingBag);
            Assert.AreEqual(expected_price, actual);
        }

        [TestMethod]
        public void TestCalculatePrice_CompositeProductDiscounts()
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_discounts = new string[] { makeAndproductDiscount(10, 20)(1), makeXorproductDiscount(10, 20)(2), makeXorproductDiscount(30, 40)(3) };
            string[] category_discounts = new string[] { };
            string[] store_discounts = new string[] { };
            double expected_price = 270;
            for (int i = 1; i <= product_discounts.Length; i++)
            {
                discountPolicy.AddProductDiscount(product_discounts[i - 1], i);
            }
            for (int i = 0; i < category_discounts.Length; i++)
            {
                if (i % 2 == 0)
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat1");
                else
                    discountPolicy.AddCategoryDiscount(category_discounts[i], "Cat2");
            }
            for (int i = 0; i < store_discounts.Length; i++)
            {
                discountPolicy.AddStoreDiscount(store_discounts[i]);
            }
            double actual = discountPolicy.CalculateDiscount(shoppingBag);
            Assert.AreEqual(expected_price, actual);
        }
    }
}
