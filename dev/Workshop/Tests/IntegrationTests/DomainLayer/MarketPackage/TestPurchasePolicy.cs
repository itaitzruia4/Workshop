using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Tests.UnitTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestPurchasePolicy
    {
        PurchasePolicy purchasePolicy;
        Store store;
        [TestInitialize]
        public void InitSystem()
        {
            store = new Store(1, "Store1", new Workshop.DomainLayer.UserPackage.Permissions.Member("member", "pass", DateTime.ParseExact("22/08/1972", "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            purchasePolicy = new PurchasePolicy(store);
        }

        public Func<int, string> makeSimpleProductPurchaseTerm(string type, string action, string value)
        {
            Func<int, string> func = id => "{ tag: 'ProductPurchaseSimpleTerm', type: '" + type + "', action: '" + action + "', value: '" + value + "', productId: " + id + "}";
            return func;
        }

        public Func<string, string> makeSimpleCategoryPurchaseTerm(string type, string action, string value)
        {
            Func<string, string> func = category => "{ tag: 'CategoryPurchaseSimpleTerm', type: '" + type + "', action: '" + action + "', value: '" + value + "', category: '" + category + "'}";
            return func;
        }

        public string makeSimpleBagPurchaseTerm(string type, string action, string value)
        {
            return "{ tag: 'BagPurchaseSimpleTerm', type: '" + type + "', action: '" + action + "', value: '" + value + "'}";
        }

        public string makeSimpleUserPurchaseTerm(string action, int age)
        {
            return "{ tag: 'UserPurchaseSimpleTerm', action: '" + action + "', age: '" + age + "'}";
        }

        public string makeAndPurchaseTerm(string l_term, string r_term)
        {
            return "{ tag: 'PurchaseCompositeTerm', value: 'and', lhs: " + l_term + ", rhs: " + r_term + "}";
        }

        public string makeOrPurchaseTerm(string l_term, string r_term)
        {
            return "{ tag: 'PurchaseCompositeTerm', value: 'or', lhs: " + l_term + ", rhs: " + r_term + "}";
        }


        [DataTestMethod]
        [DataRow("p", ">", "10")]
        [DataRow("q", "<", "5")]
        [DataRow("h", "!=", "01:00")]
        [DataRow("d", "!=", "27/08/2023")]
        public void TestAddProductPurchaseTerm_Good_Simple(string type, string action, string value)
        {
            purchasePolicy.AddProductTerm(makeSimpleProductPurchaseTerm(type, action, value)(1), 1);
        }

        [DataTestMethod]
        [DataRow("t", ">", "10")]
        [DataRow("t", ">", "-2")]
        [DataRow("q", "$", "5")]
        [DataRow("h", "!=", "25:00")]
        [DataRow("d", "!=", "27/08/2020")]
        public void TestAddProductPurchaseTerm_Bad_Simple_WrongParameters(string type, string action, string value)
        {
            Assert.ThrowsException<Exception>(() => purchasePolicy.AddProductTerm(makeSimpleProductPurchaseTerm(type, action, value)(1), 1));
        }



        [DataTestMethod]
        [DataRow("p", ">", "10", "q", "<", "5")]
        [DataRow("h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddProductPurchaseTerm_Good_And(string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            purchasePolicy.AddProductTerm((makeAndPurchaseTerm(makeSimpleProductPurchaseTerm(l_type, l_action, l_value)(1), makeSimpleProductPurchaseTerm(r_type, r_action, r_value)(1))), 1);
        }

        [DataTestMethod]
        [DataRow("p", ">", "10", "q", "<", "5")]
        [DataRow("h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddProductPurchaseTerm_Good_Or(string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            purchasePolicy.AddProductTerm((makeOrPurchaseTerm(makeSimpleProductPurchaseTerm(l_type, l_action, l_value)(1), makeSimpleProductPurchaseTerm(r_type, r_action, r_value)(1))), 1);
        }

        [DataTestMethod]
        [DataRow("p", ">", "10")]
        [DataRow("q", "<", "5")]
        [DataRow("h", "!=", "01:00")]
        [DataRow("d", "!=", "27/08/2023")]
        public void TestAddCategoryPurchaseTerm_Good_Simple(string type, string action, string value)
        {
            purchasePolicy.AddCategoryTerm(makeSimpleCategoryPurchaseTerm(type, action, value)("Cat1"), "Cat1");
        }

        [DataTestMethod]
        [DataRow("t", ">", "10")]
        [DataRow("q", "<", "-2")]
        [DataRow("h", "$", "01:00")]
        [DataRow("d", "!=", "27/08/2020")]
        public void TestAddCategoryPurchaseTerm_Bad_Simple_WrongParameters(string type, string action, string value)
        {
            Assert.ThrowsException<Exception>(() => purchasePolicy.AddCategoryTerm(makeSimpleCategoryPurchaseTerm(type, action, value)("Cat1"), "Cat1"));
        }


        [DataTestMethod]
        [DataRow("p", ">", "10", "q", "<", "5")]
        [DataRow("h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddCategoryPurchaseTerm_Good_And(string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            purchasePolicy.AddCategoryTerm((makeAndPurchaseTerm(makeSimpleCategoryPurchaseTerm(l_type, l_action, l_value)("Cat1"), makeSimpleCategoryPurchaseTerm(r_type, r_action, r_value)("Cat1"))), "Cat1");
        }


        [DataTestMethod]
        [DataRow("p", ">", "10", "q", "<", "5")]
        [DataRow("h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddCategoryPurchaseTerm_Good_Or(string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            purchasePolicy.AddCategoryTerm((makeOrPurchaseTerm(makeSimpleCategoryPurchaseTerm(l_type, l_action, l_value)("Cat1"), makeSimpleCategoryPurchaseTerm(r_type, r_action, r_value)("Cat1"))), "Cat1");
        }

        [DataTestMethod]
        [DataRow("p", ">", "10")]
        [DataRow("q", "<", "5")]
        [DataRow("h", "!=", "01:00")]
        [DataRow("d", "!=", "27/08/2023")]
        public void TestAddCategoryBagTerm_Good_Simple(string type, string action, string value)
        {
            purchasePolicy.AddStoreTerm(makeSimpleBagPurchaseTerm(type, action, value));
        }

        [DataTestMethod]
        [DataRow("t", ">", "10")]
        [DataRow("q", "<", "-2")]
        [DataRow("h", "$", "01:00")]
        [DataRow("d", "!=", "27/08/2020")]
        public void TestAddCategoryBagTerm_Bad_WrongParameters(string type, string action, string value)
        {
            Assert.ThrowsException<Exception>(() => purchasePolicy.AddStoreTerm(makeSimpleBagPurchaseTerm(type, action, value)));
        }


        [DataTestMethod]
        [DataRow("p", ">", "10", "q", "<", "5")]
        [DataRow("h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddBagPurchaseTerm_Good_And(string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            purchasePolicy.AddStoreTerm(makeAndPurchaseTerm(makeSimpleBagPurchaseTerm(l_type, l_action, l_value), makeSimpleBagPurchaseTerm(r_type, r_action, r_value)));
        }


        [DataTestMethod]
        [DataRow("p", ">", "10", "q", "<", "5")]
        [DataRow("h", ">", "01:00", "d", "<", "27/08/2023")]
        [DataRow("d", "!=", "20/01/2023", "q", "<=", "2")]
        public void TestAddBagPurchaseTerm_Good_Or(string l_type, string l_action, string l_value, string r_type, string r_action, string r_value)
        {
            purchasePolicy.AddStoreTerm(makeOrPurchaseTerm(makeSimpleBagPurchaseTerm(l_type, l_action, l_value), makeSimpleBagPurchaseTerm(r_type, r_action, r_value)));
        }

        [DataTestMethod]
        [DataRow("$", 10)]
        [DataRow(">=", -2)]
        public void TestAddUserTerm_Good_Simple(string action, int age)
        {
            Assert.ThrowsException<Exception>(() => purchasePolicy.AddUserTerm(makeSimpleUserPurchaseTerm(action, age)));
        }

        [DataTestMethod]
        [DataRow(">", 10)]
        [DataRow(">=", 13)]
        [DataRow("!=", 18)]
        public void TestAddUserTerm_Bad_Simple_WrongParameters(string action, int age)
        {
            purchasePolicy.AddUserTerm(makeSimpleUserPurchaseTerm(action, age));
        }


        [DataTestMethod]
        [DataRow(">", 10, "!=", 18)]
        [DataRow(">=", 13, ">", 10)]
        [DataRow("!=", 18, ">=", 16)]
        public void TestAddBagPurchaseTerm_Good_And(string l_action, int l_age, string r_action, int r_age)
        {
            purchasePolicy.AddUserTerm(makeAndPurchaseTerm(makeSimpleUserPurchaseTerm(l_action, l_age), makeSimpleUserPurchaseTerm(r_action, r_age)));
        }


        [DataTestMethod]
        [DataRow(">", 10, "!=", 18)]
        [DataRow(">=", 13, ">", 10)]
        [DataRow("!=", 18, ">=", 16)]
        public void TestAddBagPurchaseTerm_Good_Or(string l_action, int l_age, string r_action, int r_age)
        {
            purchasePolicy.AddUserTerm(makeOrPurchaseTerm(makeSimpleUserPurchaseTerm(l_action, l_age), makeSimpleUserPurchaseTerm(r_action, r_age)));
        }

        [DataTestMethod]
        [DataRow(18)]
        [DataRow(20)]
        public void TestCanPurchase_true(int age)
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_terms = new string[] { makeSimpleProductPurchaseTerm("p", ">", "200")(1), makeSimpleProductPurchaseTerm("q", "=", "2")(2) };
            string[] category_terms = new string[] { makeSimpleCategoryPurchaseTerm("q", "<", "3")("Cat1"), makeSimpleCategoryPurchaseTerm("q", ">", "4")("Cat2") };
            string[] store_terms = new string[] { makeSimpleBagPurchaseTerm("h", ">", "01:00") };
            string[] user_terms = new string[] { makeSimpleUserPurchaseTerm(">=", 18) };
            for (int i = 1; i <= product_terms.Length; i++)
            {
                purchasePolicy.AddProductTerm(product_terms[i - 1], i);
            }
            for (int i = 0; i < category_terms.Length; i++)
            {
                if (i % 2 == 0)
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat1");
                else
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat2");
            }
            for (int i = 0; i < store_terms.Length; i++)
            {
                purchasePolicy.AddStoreTerm(store_terms[i]);
            }
            for (int i = 0; i < user_terms.Length; i++)
            {
                purchasePolicy.AddUserTerm(user_terms[i]);
            }
            Assert.IsTrue(purchasePolicy.CanPurchase(shoppingBag, age));
        }

        [DataTestMethod]
        [DataRow(10)]
        [DataRow(17)]
        [DataRow(18)]
        public void TestCanPurchase_false_BadAge(int age)
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_terms = new string[] { makeSimpleProductPurchaseTerm("p", ">", "200")(1), makeSimpleProductPurchaseTerm("q", "=", "2")(2) };
            string[] category_terms = new string[] { makeSimpleCategoryPurchaseTerm("q", "<", "3")("Cat1"), makeSimpleCategoryPurchaseTerm("q", ">", "4")("Cat2") };
            string[] store_terms = new string[] { makeSimpleBagPurchaseTerm("h", ">", "01:00") };
            string[] user_terms = new string[] { makeSimpleUserPurchaseTerm(">", 18) };
            for (int i = 1; i <= product_terms.Length; i++)
            {
                purchasePolicy.AddProductTerm(product_terms[i - 1], i);
            }
            for (int i = 0; i < category_terms.Length; i++)
            {
                if (i % 2 == 0)
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat1");
                else
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat2");
            }
            for (int i = 0; i < store_terms.Length; i++)
            {
                purchasePolicy.AddStoreTerm(store_terms[i]);
            }
            for (int i = 0; i < user_terms.Length; i++)
            {
                purchasePolicy.AddUserTerm(user_terms[i]);
            }
            Assert.IsFalse(purchasePolicy.CanPurchase(shoppingBag, age));
        }

        [DataTestMethod]
        [DataRow(18)]
        [DataRow(20)]
        public void TestCanPurchase_false_BadQuantity(int age)
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 3, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_terms = new string[] { makeSimpleProductPurchaseTerm("p", ">", "200")(1), makeSimpleProductPurchaseTerm("q", "=", "2")(2) };
            string[] category_terms = new string[] { makeSimpleCategoryPurchaseTerm("q", "<", "3")("Cat1"), makeSimpleCategoryPurchaseTerm("q", ">", "4")("Cat2") };
            string[] store_terms = new string[] { makeSimpleBagPurchaseTerm("h", ">", "01:00") };
            string[] user_terms = new string[] { makeSimpleUserPurchaseTerm(">=", 18) };
            for (int i = 1; i <= product_terms.Length; i++)
            {
                purchasePolicy.AddProductTerm(product_terms[i - 1], i);
            }
            for (int i = 0; i < category_terms.Length; i++)
            {
                if (i % 2 == 0)
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat1");
                else
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat2");
            }
            for (int i = 0; i < store_terms.Length; i++)
            {
                purchasePolicy.AddStoreTerm(store_terms[i]);
            }
            for (int i = 0; i < user_terms.Length; i++)
            {
                purchasePolicy.AddUserTerm(user_terms[i]);
            }
            Assert.IsFalse(purchasePolicy.CanPurchase(shoppingBag, age));
        }

        [DataTestMethod]
        [DataRow(18)]
        [DataRow(20)]
        public void TestCanPurchase_false_BadPrice(int age)
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 2, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_terms = new string[] { makeSimpleProductPurchaseTerm("p", ">", "200")(1), makeSimpleProductPurchaseTerm("q", "=", "2")(2) };
            string[] category_terms = new string[] { makeSimpleCategoryPurchaseTerm("q", "<", "3")("Cat1"), makeSimpleCategoryPurchaseTerm("q", ">", "4")("Cat2") };
            string[] store_terms = new string[] { makeSimpleBagPurchaseTerm("h", ">", "01:00") };
            string[] user_terms = new string[] { makeSimpleUserPurchaseTerm(">=", 18) };
            for (int i = 1; i <= product_terms.Length; i++)
            {
                purchasePolicy.AddProductTerm(product_terms[i - 1], i);
            }
            for (int i = 0; i < category_terms.Length; i++)
            {
                if (i % 2 == 0)
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat1");
                else
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat2");
            }
            for (int i = 0; i < store_terms.Length; i++)
            {
                purchasePolicy.AddStoreTerm(store_terms[i]);
            }
            for (int i = 0; i < user_terms.Length; i++)
            {
                purchasePolicy.AddUserTerm(user_terms[i]);
            }
            Assert.IsFalse(purchasePolicy.CanPurchase(shoppingBag, age));
        }

        [DataTestMethod]
        [DataRow(18)]
        [DataRow(20)]
        public void TestCanPurchase_false_BadHour(int age)
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_terms = new string[] { makeSimpleProductPurchaseTerm("p", ">", "200")(1), makeSimpleProductPurchaseTerm("q", "=", "2")(2) };
            string[] category_terms = new string[] { makeSimpleCategoryPurchaseTerm("q", "<", "3")("Cat1"), makeSimpleCategoryPurchaseTerm("q", ">", "4")("Cat2") };
            string[] store_terms = new string[] { makeSimpleBagPurchaseTerm("h", "=", "04:04") };
            string[] user_terms = new string[] { makeSimpleUserPurchaseTerm(">=", 18) };
            for (int i = 1; i <= product_terms.Length; i++)
            {
                purchasePolicy.AddProductTerm(product_terms[i - 1], i);
            }
            for (int i = 0; i < category_terms.Length; i++)
            {
                if (i % 2 == 0)
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat1");
                else
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat2");
            }
            for (int i = 0; i < store_terms.Length; i++)
            {
                purchasePolicy.AddStoreTerm(store_terms[i]);
            }
            for (int i = 0; i < user_terms.Length; i++)
            {
                purchasePolicy.AddUserTerm(user_terms[i]);
            }
            Assert.IsFalse(purchasePolicy.CanPurchase(shoppingBag, age));
        }

        [DataTestMethod]
        [DataRow(18)]
        [DataRow(20)]
        public void TestCanPurchase_false_BadDate(int age)
        {
            ShoppingBagDTO shoppingBag = new ShoppingBagDTO(1, new List<ProductDTO>(new ProductDTO[] { new ProductDTO(1, "", "", 100.0, 3, null, 1), new ProductDTO(2, "", "", 200.0, 2, "Cat1", 1), new ProductDTO(3, "", "", 50.0, 5, "Cat2", 1) }));
            string[] product_terms = new string[] { makeSimpleProductPurchaseTerm("p", ">", "200")(1), makeSimpleProductPurchaseTerm("q", "=", "2")(2) };
            string[] category_terms = new string[] { makeSimpleCategoryPurchaseTerm("q", "<", "3")("Cat1"), makeSimpleCategoryPurchaseTerm("q", ">", "4")("Cat2") };
            string[] store_terms = new string[] { makeSimpleBagPurchaseTerm("h", ">", "01:00"), makeSimpleBagPurchaseTerm("d", "=", "12/12/2030") };
            string[] user_terms = new string[] { makeSimpleUserPurchaseTerm(">=", 18) };
            for (int i = 1; i <= product_terms.Length; i++)
            {
                purchasePolicy.AddProductTerm(product_terms[i - 1], i);
            }
            for (int i = 0; i < category_terms.Length; i++)
            {
                if (i % 2 == 0)
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat1");
                else
                    purchasePolicy.AddCategoryTerm(category_terms[i], "Cat2");
            }
            for (int i = 0; i < store_terms.Length; i++)
            {
                purchasePolicy.AddStoreTerm(store_terms[i]);
            }
            for (int i = 0; i < user_terms.Length; i++)
            {
                purchasePolicy.AddUserTerm(user_terms[i]);
            }
            Assert.IsFalse(purchasePolicy.CanPurchase(shoppingBag, age));
        }
    }
}
