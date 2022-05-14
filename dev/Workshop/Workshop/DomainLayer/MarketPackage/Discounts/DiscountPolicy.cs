using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.Discounts;
using Workshop.DomainLayer.MarketPackage.Terms;
using Workshop.DomainLayer.UserPackage.Shopping;
using static Workshop.DomainLayer.MarketPackage.Terms.Term;

namespace Workshop.DomainLayer.MarketPackage
{
    public class DiscountPolicy
    {
        private Dictionary<int, Discount> products_discounts;
        private Dictionary<string, Discount> category_discounts;
        private Discount store_discount;
        private Store store;

        private const string AND_DISCOUNT_TAG = "AndDiscount";
        private const string OR_DISCOUNT_TAG = "OrDiscount";
        private const string XOR_DISCOUNT_TAG = "XorDiscount";
        private const string SIMPLE_DISCOUNT_TAG = "SimpleDiscount";
        private const string COND_DISCOUNT_TAG = "ConditionalDiscount";
        private const string PRICE_ACT_COMP_TAG = "PriceActionComposite";
        private const string PRODUCT_PRICE_ACT_TAG = "ProductPriceActionSimple";
        private const string CATEGORY_PRICE_ACT_TAG = "CategoryPriceActionSimple";
        private const string DISCOUNT_COMP_TERM_TAG = "DiscountCompositeTerm";
        private const string PRODUCT_SIMPLE_TERM_TAG = "ProductDiscountSimpleTerm";
        private const string CATEGORY_SIMPLE_TERM_TAG = "CategoryDiscountSimpleTerm";
        private const string BAG_SIMPLE_TERM_TAG = "BagDiscountSimpleTerm";

        public DiscountPolicy(Store store)
        {
            products_discounts = new Dictionary<int, Discount>();
            category_discounts = new Dictionary<string, Discount>();
            store_discount = null;
            this.store = store;
        }

        public void AddProductDiscount(string json_discount, int product_id)
        {
            Discount discount = ParseDiscount(json_discount);
            if (!products_discounts.ContainsKey(product_id))
                products_discounts.Add(product_id, discount);
            else
                products_discounts[product_id] = new OrDiscount(products_discounts[product_id], discount);
        }

        public void AddCategoryDiscount(string json_discount, string category_name)
        {
            Discount discount = ParseDiscount(json_discount);
            if (!category_discounts.ContainsKey(category_name))
                category_discounts.Add(category_name, discount);
            else
                category_discounts[category_name] = new OrDiscount(category_discounts[category_name], discount);
        }

        public void AddStoreDiscount(string json_discount)
        {
            Discount discount = ParseDiscount(json_discount);
            if (store_discount == null)
                store_discount = discount;
            else
                store_discount = new OrDiscount(store_discount, discount);
        }

        private Discount ParseDiscount(string json_discount)
        {
            dynamic discount_data = JObject.Parse(json_discount);
            return ParseDiscount(discount_data);
        }

        private Discount ParseDiscount(dynamic data)
        {
            string tag = data.tag;
            if (tag.Equals(AND_DISCOUNT_TAG))
                return ParseAndDiscount(data);
            if (tag.Equals(OR_DISCOUNT_TAG))
                return ParseOrDiscount(data);
            if (tag.Equals(XOR_DISCOUNT_TAG))
                return ParseXorDiscount(data);
            if (tag.Equals(SIMPLE_DISCOUNT_TAG))
                return ParseSimpleDiscount(data);
            if (tag.Equals(COND_DISCOUNT_TAG))
                return ParseConditionalDiscount(data);
            throw new Exception("Unknown discount tag: " + tag);
        }

        private Discount ParseAndDiscount(dynamic data)
        {
            return new AndDiscount(ParseDiscount(data.left), ParseDiscount(data.right));
        }

        private Discount ParseOrDiscount(dynamic data)
        {
            return new OrDiscount(ParseDiscount(data.left), ParseDiscount(data.right));
        }

        private Discount ParseXorDiscount(dynamic data)
        {
            return new XorDiscount(ParseDiscount(data.left), ParseDiscount(data.right), ParseDiscountTerm(data.discountTerm));
        }

        private Discount ParseSimpleDiscount(dynamic data)
        {
            return new SimpleDiscount(ParsePriceAction(data.priceAction));
        }

        private Discount ParseConditionalDiscount(dynamic data)
        {
            return new ConditionalDiscount(ParsePriceAction(data.priceAction), ParseDiscountTerm(data.discountTerm));
        }

        private Discount ParsePriceAction(dynamic data)
        {
            string tag = data.tag;
            if (tag.Equals(PRICE_ACT_COMP_TAG))
                return ParsePriceActionComposite(data);
            if (tag.Equals(PRODUCT_PRICE_ACT_TAG))
                return ParseProductPriceActionSimple(data);
            if (tag.Equals(CATEGORY_PRICE_ACT_TAG))
                return ParseCategoryPriceActionSimple(data);
            throw new Exception("Unknown price action tag: " + tag);
        }

        private PriceAction ParsePriceActionComposite(dynamic data)
        {
            string action = data.value;
            if (action.Equals("sum"))
                return new SumComposite(ParsePriceAction(data.left), ParsePriceAction(data.right));
            if (action.Equals("max"))
                return new MaxComposite(ParsePriceAction(data.left), ParsePriceAction(data.right));
            throw new Exception("Unknown price action: " + action);
        }

        private PriceAction ParseProductPriceActionSimple(dynamic data)
        {
            try
            {
                double percentage = double.Parse(data.percentage);
                try
                {
                    int product_id = double.Parse(data.key);
                    if (percentage <= 0 || percentage > 100)
                        throw new Exception("Discount percentage must be between 0 and 100");

                    if (!store.ProductExists(product_id))
                        throw new Exception("Product id: " + product_id + "does not exist in store: " + store.GetId());
                    return new PriceActionSimple(percentage, (ProductDTO product) => { return product.Id == product_id; });
                }
                catch (Exception)
                {
                    throw new Exception("Invalid product id: " + data.key);
                }
            }
            catch (Exception) { 
                throw new Exception("Invalid percentage: " + data.percentage);
            } 
        }

        private PriceAction ParseCategoryPriceActionSimple(dynamic data)
        {
            
            try
            {
                double percentage = double.Parse(data.percentage);
                string category = data.key;
                if (percentage <= 0 || percentage > 100)
                    throw new Exception("Discount percentage must be between 0 and 100");
                if (category == null || category.Equals(""))
                    throw new Exception("Discount category must be non-empty.");

                return new PriceActionSimple(percentage, (ProductDTO product) => { return category.Equals(product.Category); });
            }
            catch (Exception)
            {
                throw new Exception("Invalid product id: " + data.key);
            }
        }

        private Term ParseDiscountTerm(dynamic data)
        {
            string tag = data.tag;
            if (tag.Equals(DISCOUNT_COMP_TERM_TAG))
                return ParseDiscountCompositeTerm(data);
            if (tag.Equals(PRODUCT_SIMPLE_TERM_TAG))
                return ParseProductDiscountSimpleTerm(data);
            if (tag.Equals(CATEGORY_SIMPLE_TERM_TAG))
                return ParseCategoryDiscountSimpleTerm(data);
            if (tag.Equals(BAG_SIMPLE_TERM_TAG))
                return ParseBagDiscountSimpleTerm(data);
            throw new Exception("Unknown discount term tag: " + tag);
        }

        internal int CalculateDiscount(ShoppingBagDTO shoppingBag)
        {
            throw new NotImplementedException();
        }

        private Term ParseDiscountCompositeTerm(dynamic data)
        {
            string term = data.value;
            if (term.Equals("and"))
                return new AndTerm(ParseDiscountTerm(data.left), ParseDiscountTerm(data.right));
            if (term.Equals("or"))
                return new OrTerm(ParseDiscountTerm(data.left), ParseDiscountTerm(data.right));
            if (term.Equals("xor"))
                return new XorTerm(ParseDiscountTerm(data.left), ParseDiscountTerm(data.right));
            throw new Exception("Unknown discount term action: " + term);
        }

        private Term ParseProductDiscountSimpleTerm(dynamic data)
        {
            try
            {
                SimpleTerm.TermSimple filter;
                string action = data.action;
                double value = double.Parse(data.value);
                string type = data.type;
                try
                {
                    int product_id = double.Parse(data.key);
                    if (value <= 0)
                        throw new Exception("Discount term value must be above 0.");

                    if (!store.ProductExists(product_id))
                        throw new Exception("Product id: " + product_id + "does not exist in store: " + store.GetId());
                    // The term is related to price of the product
                    if (type.Equals("p"))
                    {
                        if (action.Equals("<"))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double total_price = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        total_price += product.Price;
                                }
                                return total_price < value;
                            };
                        else if (action.Equals(">"))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double total_price = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        total_price += product.Price;
                                }
                                return total_price > value;
                            };
                        else if (action.Equals("="))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double total_price = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        total_price += product.Price;
                                }
                                return total_price == value;
                            };
                        else if (action.Equals(">="))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double total_price = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        total_price += product.Price;
                                }
                                return total_price >= value;
                            };
                        else if (action.Equals("<="))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double total_price = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        total_price += product.Price;
                                }
                                return total_price <= value;
                            };
                        else
                            throw new Exception("Unknown term action: " + action);
                    }
                    // The term is related to quantity of the product
                    else if (type.Equals("q"))
                    {
                        if (action.Equals("<"))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double quantity = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        quantity++;
                                }
                                return quantity < value;
                            };
                        else if (action.Equals(">"))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double quantity = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        quantity++;
                                }
                                return quantity > value;
                            };
                        else if (action.Equals("="))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double quantity = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        quantity++;
                                }
                                return quantity == value;
                            };
                        else if (action.Equals(">="))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double quantity = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        quantity++;
                                }
                                return quantity >= value;
                            };
                        else if (action.Equals("<="))
                            filter = (ShoppingBagDTO shopping_bag, int age) => {
                                double quantity = 0;
                                foreach (ProductDTO product in shopping_bag.products)
                                {
                                    if (product.Id == product_id)
                                        quantity++;
                                }
                                return quantity <= value;
                            };
                        else
                            throw new Exception("Unknown term action: " + action);
                    }
                    else
                        throw new Exception("Unknown term type: " + type);

                    return new Terms.SimpleTerm(filter);
                }
                catch (Exception)
                {
                    throw new Exception("Invalid product id: " + data.key);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid term value: " + data.value);
            }
        }

        private Term ParseCategoryDiscountSimpleTerm(dynamic data)
        {
            try
            {
                SimpleTerm.TermSimple filter;
                string action = data.action;
                double value = double.Parse(data.value);
                string type = data.type;
                string category = data.key;
                if (value <= 0)
                    throw new Exception("Discount term value must be above 0.");

                if (category == null || category.Equals(""))
                    throw new Exception("Discount term category must be non-empty.");

                // The term is related to price of the product
                if (type.Equals("p"))
                {
                    if (action.Equals("<"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    total_price += product.Price;
                            }
                            return total_price < value;
                        };
                    else if (action.Equals(">"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    total_price += product.Price;
                            }
                            return total_price > value;
                        };
                    else if (action.Equals("="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    total_price += product.Price;
                            }
                            return total_price == value;
                        };
                    else if (action.Equals(">="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    total_price += product.Price;
                            }
                            return total_price >= value;
                        };
                    else if (action.Equals("<="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    total_price += product.Price;
                            }
                            return total_price <= value;
                        };
                    else
                        throw new Exception("Unknown term action: " + action);
                }
                // The term is related to quantity of the product
                else if (type.Equals("q"))
                {
                    if (action.Equals("<"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double quantity = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    quantity++;
                            }
                            return quantity < value;
                        };
                    else if (action.Equals(">"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double quantity = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    quantity++;
                            }
                            return quantity > value;
                        };
                    else if (action.Equals("="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double quantity = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    quantity++;
                            }
                            return quantity == value;
                        };
                    else if (action.Equals(">="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double quantity = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    quantity++;
                            }
                            return quantity >= value;
                        };
                    else if (action.Equals("<="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double quantity = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                if (product.Category == category)
                                    quantity++;
                            }
                            return quantity <= value;
                        };
                    else
                        throw new Exception("Unknown term action: " + action);
                }
                else
                    throw new Exception("Unknown term type: " + type);

                return new Terms.SimpleTerm(filter);
            }
            catch (Exception)
            {
                throw new Exception("Invalid term value: " + data.value);
            }
        }

        private Term ParseBagDiscountSimpleTerm(dynamic data)
        {
            try
            {
                SimpleTerm.TermSimple filter;
                string action = data.action;
                double value = double.Parse(data.value);
                string type = data.type;
                if (value <= 0)
                    throw new Exception("Discount term value must be above 0.");

                // The term is related to price of the product
                if (type.Equals("p"))
                {
                    if (action.Equals("<"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                total_price += product.Price;
                            }
                            return total_price < value;
                        };
                    else if (action.Equals(">"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                total_price += product.Price;
                            }
                            return total_price > value;
                        };
                    else if (action.Equals("="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                total_price += product.Price;
                            }
                            return total_price == value;
                        };
                    else if (action.Equals(">="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                total_price += product.Price;
                            }
                            return total_price >= value;
                        };
                    else if (action.Equals("<="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            double total_price = 0;
                            foreach (ProductDTO product in shopping_bag.products)
                            {
                                total_price += product.Price;
                            }
                            return total_price <= value;
                        };
                    else
                        throw new Exception("Unknown term action: " + action);
                }
                // The term is related to quantity of the product
                else if (type.Equals("q"))
                {
                    if (action.Equals("<"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            return shopping_bag.products.Count() < value;
                        };
                    else if (action.Equals(">"))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            return shopping_bag.products.Count() > value;
                        };
                    else if (action.Equals("="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            return shopping_bag.products.Count() == value;
                        };
                    else if (action.Equals(">="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            return shopping_bag.products.Count() >= value;
                        };
                    else if (action.Equals("<="))
                        filter = (ShoppingBagDTO shopping_bag, int age) => {
                            return shopping_bag.products.Count() <= value;
                        };
                    else
                        throw new Exception("Unknown term action: " + action);
                }
                else
                    throw new Exception("Unknown term type: " + type);

                return new Terms.SimpleTerm(filter);
            }
            catch (Exception)
            {
                throw new Exception("Invalid term value: " + data.value);
            }
        }
    }
}
