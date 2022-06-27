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
using DALObject = Workshop.DataLayer.DALObject;
using DiscountPolicyDAL = Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy;
using DiscountDAL = Workshop.DataLayer.DataObjects.Market.Discounts.Discount;
using ProductDiscountDAL = Workshop.DataLayer.DataObjects.Market.Discounts.ProductDiscount;
using CategoryDiscountDAL = Workshop.DataLayer.DataObjects.Market.Discounts.CategoryDiscount;
using StoreDAL = Workshop.DataLayer.DataObjects.Market.Store;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.MarketPackage
{
    public class DiscountPolicy : IPersistentObject<DiscountPolicyDAL>
    {
        private Dictionary<int, Discount> products_discounts;
        private Dictionary<string, Discount> category_discounts;
        private Discount store_discount;
        private Store store;
        private DiscountPolicyDAL discountPolicyDAL;

        private const string AND_DISCOUNT_TAG = "AndDiscount";
        private const string OR_DISCOUNT_TAG = "OrDiscount";
        private const string XOR_DISCOUNT_TAG = "XorDiscount";
        private const string SIMPLE_DISCOUNT_TAG = "SimpleDiscount";
        private const string COND_DISCOUNT_TAG = "ConditionalDiscount";
        private const string PRICE_ACT_COMP_TAG = "PriceActionComposite";
        private const string PRODUCT_PRICE_ACT_TAG = "ProductPriceActionSimple";
        private const string CATEGORY_PRICE_ACT_TAG = "CategoryPriceActionSimple";
        private const string STORE_PRICE_ACT_TAG = "StorePriceActionSimple"; 
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
            this.discountPolicyDAL = new DiscountPolicyDAL(new List<ProductDiscountDAL>(), new List<CategoryDiscountDAL>(), null);
            DataHandler.getDBHandler().save(discountPolicyDAL);
        }

        public DiscountPolicy(DiscountPolicyDAL discountPolicyDAL, Store store)
        {
            this.products_discounts = new Dictionary<int, Discount>();
            this.category_discounts = new Dictionary<string, Discount>();
            this.store_discount = null;
            this.store = store;
            this.discountPolicyDAL = null;
            foreach(ProductDiscountDAL productDiscount in discountPolicyDAL.products_discounts)
                AddProductDiscount(productDiscount.Discount.discountJson, productDiscount.ProductId, productDiscount.Discount);
            foreach(CategoryDiscountDAL categoryDiscount in discountPolicyDAL.category_discounts)
                AddCategoryDiscount(categoryDiscount.Discount.discountJson, categoryDiscount.Name, categoryDiscount.Discount);
            if(discountPolicyDAL.store_discount != null)
                AddStoreDiscount(discountPolicyDAL.store_discount.discountJson);
            this.discountPolicyDAL = discountPolicyDAL;
        }

        public DiscountPolicyDAL ToDAL()
        {
            return this.discountPolicyDAL;
        }

        public void AddProductDiscount(string json_discount, int product_id)
        {
            AddProductDiscount(json_discount, product_id, null);
        }

        private void AddProductDiscount(string json_discount, int product_id, DiscountDAL discountDAL)
        {
            Discount discount = ParseDiscount(json_discount);
            discount.DALDiscount = discountDAL;
            if (!products_discounts.ContainsKey(product_id))
            {
                products_discounts.Add(product_id, discount);
                if (this.discountPolicyDAL != null)
                {
                    products_discounts[product_id].DALDiscount = new DiscountDAL(json_discount);
                    DataHandler.getDBHandler().save(products_discounts[product_id].DALDiscount);
                    this.discountPolicyDAL.products_discounts.Add(new ProductDiscountDAL(product_id, discount.ToDAL()));
                    DataHandler.getDBHandler().update(discountPolicyDAL);
                }
            }
            else
            {
                products_discounts[product_id] = new OrDiscount(products_discounts[product_id], discount);
                if (this.discountPolicyDAL != null)
                {
                    products_discounts[product_id].DALDiscount = new DiscountDAL(json_discount);
                    DataHandler.getDBHandler().save(products_discounts[product_id].DALDiscount);
                    ProductDiscountDAL dal_discount = this.discountPolicyDAL.products_discounts.Find(d => d.ProductId == product_id);
                    dal_discount.Discount = products_discounts[product_id].ToDAL();
                    DataHandler.getDBHandler().update(dal_discount);
                }
            }
        }

        public void AddCategoryDiscount(string json_discount, string category_name)
        {
            AddCategoryDiscount(json_discount, category_name, null);
        }

        private void AddCategoryDiscount(string json_discount, string category_name, DiscountDAL discountDAL)
        {
            Discount discount = ParseDiscount(json_discount);
            discount.DALDiscount = discountDAL;
            if (!category_discounts.ContainsKey(category_name))
            {
                category_discounts.Add(category_name, discount);
                if (this.discountPolicyDAL != null)
                {
                    category_discounts[category_name].DALDiscount = new DiscountDAL(json_discount);
                    DataHandler.getDBHandler().save(category_discounts[category_name].DALDiscount);
                    this.discountPolicyDAL.category_discounts.Add(new CategoryDiscountDAL(category_name, discount.ToDAL()));
                    DataHandler.getDBHandler().update(discountPolicyDAL);
                }
            }
            else
            {
                category_discounts[category_name] = new OrDiscount(category_discounts[category_name], discount);
                if (this.discountPolicyDAL != null)
                {
                    category_discounts[category_name].DALDiscount = new DiscountDAL(json_discount);
                    DataHandler.getDBHandler().save(category_discounts[category_name].DALDiscount);
                    CategoryDiscountDAL dal_discount = this.discountPolicyDAL.category_discounts.Find(d => d.Name == category_name);
                    dal_discount.Discount = category_discounts[category_name].ToDAL();
                    DataHandler.getDBHandler().update(dal_discount);
                }
            }
        }

        public void AddStoreDiscount(string json_discount)
        {
            AddStoreDiscount(json_discount, null);
        }

        public void AddStoreDiscount(string json_discount, DiscountDAL discountDAL)
        {
            Discount discount = ParseDiscount(json_discount);
            discount.DALDiscount = discountDAL;
            if (store_discount == null)
            {
                store_discount = discount;
            }
            else
            {
                store_discount = new OrDiscount(store_discount, discount);
            }
            if (this.discountPolicyDAL != null)
            {
                store_discount.DALDiscount = new DiscountDAL(json_discount);
                DataHandler.getDBHandler().save(store_discount.DALDiscount);
                this.discountPolicyDAL.store_discount = store_discount.ToDAL();
                DataHandler.getDBHandler().update(discountPolicyDAL);
            }
        }

        private Discount ParseDiscount(string json_discount)
        {
            dynamic discount_data = JObject.Parse(json_discount);
            Discount discount = ParseDiscount(discount_data);
            /*if (this.discountPolicyDAL != null)
            {
                discount.DALDiscount = new DiscountDAL(json_discount);
                DataHandler.getDBHandler().save(discount.DALDiscount);
            }*/
            return discount;
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
            return new AndDiscount(ParseDiscount(data.lhs), ParseDiscount(data.rhs));
        }

        private Discount ParseOrDiscount(dynamic data)
        {
            return new OrDiscount(ParseDiscount(data.lhs), ParseDiscount(data.rhs));
        }

        private Discount ParseXorDiscount(dynamic data)
        {
            return new XorDiscount(ParseDiscount(data.lhs), ParseDiscount(data.rhs), ParseDiscountTerm(data.discountTerm));
        }

        private Discount ParseSimpleDiscount(dynamic data)
        {
            return new SimpleDiscount(ParsePriceAction(data.priceAction));
        }

        private Discount ParseConditionalDiscount(dynamic data)
        {
            return new ConditionalDiscount(ParsePriceAction(data.priceAction), ParseDiscountTerm(data.discountTerm));
        }

        private PriceAction ParsePriceAction(dynamic data)
        {
            string tag = data.tag;
            if (tag.Equals(PRICE_ACT_COMP_TAG))
                return ParsePriceActionComposite(data);
            if (tag.Equals(PRODUCT_PRICE_ACT_TAG))
                return ParseProductPriceActionSimple(data);
            if (tag.Equals(CATEGORY_PRICE_ACT_TAG))
                return ParseCategoryPriceActionSimple(data);
            if (tag.Equals(STORE_PRICE_ACT_TAG))
                return ParseStorePriceActionSimple(data);
            throw new Exception("Unknown OfferedPrice action tag: " + tag);
        }

        private PriceAction ParsePriceActionComposite(dynamic data)
        {
            string action = data.value;
            if (action.Equals("sum"))
                return new SumComposite(ParsePriceAction(data.lhs), ParsePriceAction(data.rhs));
            if (action.Equals("max"))
                return new MaxComposite(ParsePriceAction(data.lhs), ParsePriceAction(data.rhs));
            throw new Exception("Unknown OfferedPrice action: " + action);
        }

        private PriceAction ParseProductPriceActionSimple(dynamic data)
        {
            double percentage;
            int product_id;
            try
            {
                percentage = double.Parse(((string)data.percentage).Trim('{','}'));
                try
                {
                    product_id = int.Parse(((string)data.productId).Trim('{', '}'));
                    
                }
                catch (Exception)
                {
                    throw new Exception("Invalid Product id: " + data.productId);
                }
            }
            catch (Exception) { 
                throw new Exception("Invalid percentage: " + data.percentage);
            }

            if (percentage <= 0 || percentage > 100)
                throw new Exception("Discount percentage must be between 0 and 100");

            if (!store.ProductExists(product_id))
                throw new Exception("Product id: " + product_id + "does not exist in store: " + store.GetId());
            return new PriceActionSimple(percentage, (ProductDTO product) => { return product.Id == product_id; });
        }

        private PriceAction ParseCategoryPriceActionSimple(dynamic data)
        {
            double percentage = 0;
            try
            {
                percentage = double.Parse(((string)data.percentage).Trim('{', '}'));
            }
            catch (Exception)
            {
                throw new Exception("Invalid percentage: " + data.percentage);
            }
            string category = data.category;
            if (percentage <= 0 || percentage > 100)
                throw new Exception("Discount percentage must be between 0 and 100");
            if (category == null || category.Equals(""))
                throw new Exception("Discount category must be non-empty.");

            return new PriceActionSimple(percentage, (ProductDTO product) => { return category.Equals(product.Category); });
        }
        private PriceAction ParseStorePriceActionSimple(dynamic data)
        {
            double percentage;
            try
            {
                percentage = double.Parse(((string)data.percentage).Trim('{', '}'));
            }
            catch (Exception)
            {
                throw new Exception("Invalid Product id: " + data.category);
            }
            if (percentage <= 0 || percentage > 100)
                throw new Exception("Discount percentage must be between 0 and 100");

            return new PriceActionSimple(percentage, (ProductDTO product) => { return true; });
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

        public double CalculateDiscount(ShoppingBagDTO shoppingBag)
        {
            double totalDiscount = 0.0;
            foreach (ProductDTO prod in shoppingBag.products)
            {
                if (products_discounts.ContainsKey(prod.Id))
                {
                    totalDiscount += products_discounts[prod.Id].CalculateDiscountValue(shoppingBag);
                }
            }
            foreach (string category in category_discounts.Keys)
            {
                totalDiscount += category_discounts[category].CalculateDiscountValue(shoppingBag);
            }
            if (store_discount != null)
                totalDiscount += store_discount.CalculateDiscountValue(shoppingBag);
            return totalDiscount;
        }

        private Term ParseDiscountCompositeTerm(dynamic data)
        {
            string term = data.value;
            if (term.Equals("and"))
                return new AndTerm(ParseDiscountTerm(data.lhs), ParseDiscountTerm(data.rhs));
            if (term.Equals("or"))
                return new OrTerm(ParseDiscountTerm(data.lhs), ParseDiscountTerm(data.rhs));
            if (term.Equals("xor"))
                return new XorTerm(ParseDiscountTerm(data.lhs), ParseDiscountTerm(data.rhs));
            throw new Exception("Unknown discount term action: " + term);
        }

        private Term ParseProductDiscountSimpleTerm(dynamic data)
        {
            SimpleTerm.TermSimple filter;
            string action = ((string)data.action).Trim('{', '}');
            string type = ((string)data.type).Trim('{', '}');
            double value;
            int product_id;
            try
            {
                value = double.Parse(((string)data.value).Trim('{', '}'));
                try
                {
                    product_id = int.Parse(((string)data.productId).Trim('{', '}'));
                    
                }
                catch (Exception)
                {
                    throw new Exception("Invalid Product id: " + data.productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid term value: " + data.value);
            }
            if (value <= 0)
                throw new Exception("Discount term value must be above 0.");

            if (!store.ProductExists(product_id))
                throw new Exception("Product id: " + product_id + "does not exist in store: " + store.GetId());
            // The term is related to OfferedPrice of the Product
            if (type.Equals("p"))
            {
                if (action.Equals("<"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price < value;
                    };
                else if (action.Equals(">"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price > value;
                    };
                else if (action.Equals("="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price == value;
                    };
                else if (action.Equals(">="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price >= value;
                    };
                else if (action.Equals("<="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price <= value;
                    };
                else
                    throw new Exception("Unknown term action: " + action);
            }
            // The term is related to quantity of the Product
            else if (type.Equals("q"))
            {
                if (value - (int)(value) > 0)
                    throw new Exception("Quantity Discount term value must an integer, not double.");
                if (action.Equals("<"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                quantity += product.Quantity;
                        }
                        return quantity < value;
                    };
                else if (action.Equals(">"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                quantity += product.Quantity;
                        }
                        return quantity > value;
                    };
                else if (action.Equals("="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                quantity += product.Quantity;
                        }
                        return quantity == value;
                    };
                else if (action.Equals(">="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                quantity += product.Quantity;
                        }
                        return quantity >= value;
                    };
                else if (action.Equals("<="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Id == product_id)
                                quantity += product.Quantity;
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

        private Term ParseCategoryDiscountSimpleTerm(dynamic data)
        {
            SimpleTerm.TermSimple filter;
            string action = ((string)data.action).Trim('{', '}');
            double value;
            string type = ((string)data.type).Trim('{', '}');
            string category = ((string)data.category).Trim('{', '}');
            try
            {
                value = double.Parse(((string)data.value).Trim('{', '}'));
            }
            catch (Exception)
            {
                throw new Exception("Invalid term value: " + data.value);
            }
            if (value <= 0)
                throw new Exception("Discount term value must be above 0.");

            if (category == null || category.Equals(""))
                throw new Exception("Discount term category must be non-empty.");

            // The term is related to OfferedPrice of the Product
            if (type.Equals("p"))
            {
                if (action.Equals("<"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price < value;
                    };
                else if (action.Equals(">"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price > value;
                    };
                else if (action.Equals("="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price == value;
                    };
                else if (action.Equals(">="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price >= value;
                    };
                else if (action.Equals("<="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                total_price += product.Price * product.Quantity;
                        }
                        return total_price <= value;
                    };
                else
                    throw new Exception("Unknown term action: " + action);
            }
            // The term is related to quantity of the Product
            else if (type.Equals("q"))
            {
                if (value - (int)(value) > 0)
                    throw new Exception("Quantity Discount term value must an integer, not double.");
                if (action.Equals("<"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                quantity += product.Quantity;
                        }
                        return quantity < value;
                    };
                else if (action.Equals(">"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                quantity += product.Quantity;
                        }
                        return quantity > value;
                    };
                else if (action.Equals("="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                quantity += product.Quantity;
                        }
                        return quantity == value;
                    };
                else if (action.Equals(">="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                quantity += product.Quantity;
                        }
                        return quantity >= value;
                    };
                else if (action.Equals("<="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double quantity = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            if (product.Category == category)
                                quantity += product.Quantity;
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

        private Term ParseBagDiscountSimpleTerm(dynamic data)
        {
            SimpleTerm.TermSimple filter;
            string action = data.action.Trim('{', '}');
            double value = double.Parse(((string)data.value).Trim('{', '}'));
            string type = ((string)data.type).Trim('{', '}');
            try
            {
                value = double.Parse(((string)data.value).Trim('{', '}'));
            }
            catch (Exception)
            {
                throw new Exception("Invalid term value: " + data.value);
            }
            if (value <= 0)
                throw new Exception("Discount term value must be above 0.");

            // The term is related to OfferedPrice of the Product
            if (type.Equals("p"))
            {
                if (action.Equals("<"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            total_price += product.Price * product.Quantity;
                        }
                        return total_price < value;
                    };
                else if (action.Equals(">"))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            total_price += product.Price * product.Quantity;
                        }
                        return total_price > value;
                    };
                else if (action.Equals("="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            total_price += product.Price * product.Quantity;
                        }
                        return total_price == value;
                    };
                else if (action.Equals(">="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            total_price += product.Price * product.Quantity;
                        }
                        return total_price >= value;
                    };
                else if (action.Equals("<="))
                    filter = (ShoppingBagDTO shopping_bag, int age) => {
                        double total_price = 0;
                        foreach (ProductDTO product in shopping_bag.products)
                        {
                            total_price += product.Price * product.Quantity;
                        }
                        return total_price <= value;
                    };
                else
                    throw new Exception("Unknown term action: " + action);
            }
            // The term is related to quantity of the Product
            else if (type.Equals("q"))
            {
                if (value - (int)(value) > 0)
                    throw new Exception("Quantity Discount term value must an integer, not double.");
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
    }
}
