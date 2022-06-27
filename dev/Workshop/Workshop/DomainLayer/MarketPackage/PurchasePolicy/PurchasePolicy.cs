using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.Terms;
using Workshop.DomainLayer.UserPackage.Shopping;
using DALObject = Workshop.DataLayer.DALObject;
using PurchasePolicyDAL = Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy;
using DALTerm = Workshop.DataLayer.DataObjects.Market.Purchases.Term;
using DALProductTerm = Workshop.DataLayer.DataObjects.Market.Purchases.ProductTerm;
using DALCategoryTerm = Workshop.DataLayer.DataObjects.Market.Purchases.CategoryTerm;
using DataHandler = Workshop.DataLayer.DataHandler;


namespace Workshop.DomainLayer.MarketPackage
{
    public class PurchasePolicy : IPersistentObject<PurchasePolicyDAL>
    {
        private Dictionary<int, Term> products_terms;
        private Dictionary<string, Term> category_terms;
        private Term user_terms;
        private Term store_terms;
        private PurchasePolicyDAL purchasePolicyDAL;

        private const string PURCHASE_COMP_TERM_TAG = "PurchaseCompositeTerm";
        private const string PRODUCT_SIMPLE_TERM_TAG = "ProductPurchaseSimpleTerm";
        private const string CATEGORY_SIMPLE_TERM_TAG = "CategoryPurchaseSimpleTerm";
        private const string BAG_SIMPLE_TERM_TAG = "BagPurchaseSimpleTerm";
        private const string USER_SIMPLE_TERM_TAG = "UserPurchaseSimpleTerm";

        private Store store;
        public PurchasePolicy(Store store)
        {
            this.store = store;
            this.products_terms = new Dictionary<int, Term>();
            this.category_terms = new Dictionary<string, Term>();
            this.user_terms = null;
            this.store_terms = null;
            this.purchasePolicyDAL = new PurchasePolicyDAL(new List<DALProductTerm>(), new List<DALCategoryTerm>(), null, null);
            DataHandler.getDBHandler().save(purchasePolicyDAL);
        }

        public PurchasePolicy(PurchasePolicyDAL purchasePolicyDAL, Store store)
        {
            this.store = store;
            this.products_terms = new Dictionary<int, Term>();
            this.category_terms = new Dictionary<string, Term>();
            this.user_terms = null;
            this.store_terms = null;
            this.purchasePolicyDAL = null;
            foreach (DALProductTerm product_term in purchasePolicyDAL.products_terms)
            {
                AddProductTerm(product_term.Term.TermJson, product_term.ProductId, null);
            }
            foreach (DALCategoryTerm category_term in purchasePolicyDAL.category_terms)
            {
                AddCategoryTerm(category_term.Term.TermJson, category_term.CategoryName, null);
            }
            if(purchasePolicyDAL.user_terms != null)
                AddUserTerm(purchasePolicyDAL.user_terms.TermJson);
            if(purchasePolicyDAL.store_terms != null)
                AddStoreTerm(purchasePolicyDAL.store_terms.TermJson);
            this.purchasePolicyDAL = purchasePolicyDAL;
        }

        public PurchasePolicyDAL ToDAL()
        {
            return this.purchasePolicyDAL;
        }

        public bool CanPurchase(ShoppingBagDTO shoppingBag, int age)
        {
            foreach(ProductDTO product in shoppingBag.products)
            {
                if(products_terms.ContainsKey(product.Id))
                {
                    if(!products_terms[product.Id].IsEligible(shoppingBag))
                        return false;
                }
                if(product.Category != null && category_terms.ContainsKey(product.Category))
                {
                    if (!category_terms[product.Category].IsEligible(shoppingBag))
                        return false;
                }   
            }
            if (user_terms != null && !user_terms.IsEligible(shoppingBag, age))
                return false;
            if (store_terms != null && !store_terms.IsEligible(shoppingBag))
                return false;
            return true;
        }

        public void AddProductTerm(string json_term, int product_id)
        { 
            AddProductTerm(json_term, product_id, null);
        }

        private void AddProductTerm(string json_term, int product_id, DALTerm dalterm)
        {
            Term term = ParseTerm(json_term);
            term.DALTerm = dalterm;
            if (!products_terms.ContainsKey(product_id))
            {
                products_terms.Add(product_id, term);
                if (this.purchasePolicyDAL != null)
                {
                    products_terms[product_id].DALTerm = new DALTerm(json_term);
                    DataHandler.getDBHandler().save(products_terms[product_id].DALTerm);
                    this.purchasePolicyDAL.products_terms.Add(new DALProductTerm(product_id, term.ToDAL()));
                    DataHandler.getDBHandler().update(purchasePolicyDAL);
                }
            }
            else
            {
                products_terms[product_id] = new OrTerm(products_terms[product_id], term);
                if (this.purchasePolicyDAL != null)
                {
                    products_terms[product_id].DALTerm = new DALTerm(json_term);
                    DataHandler.getDBHandler().save(products_terms[product_id].DALTerm);
                    DALProductTerm dal_term = this.purchasePolicyDAL.products_terms.Find(d => d.ProductId == product_id);
                    dal_term.Term = products_terms[product_id].ToDAL();
                    DataHandler.getDBHandler().update(dal_term);
                }
            }
        }

        public void AddCategoryTerm(string json_term, string category_name)
        { 
            AddCategoryTerm(json_term, category_name, null);
        }

        private void AddCategoryTerm(string json_term, string category_name, DALTerm dalterm)
        {
            Term term = ParseTerm(json_term);
            term.DALTerm = dalterm;
            if (!category_terms.ContainsKey(category_name))
            {
                category_terms.Add(category_name, term);
                if (this.purchasePolicyDAL != null)
                {
                    category_terms[category_name].DALTerm = new DALTerm(json_term);
                    DataHandler.getDBHandler().save(category_terms[category_name].DALTerm);
                    this.purchasePolicyDAL.category_terms.Add(new DALCategoryTerm(category_name, term.ToDAL()));
                    DataHandler.getDBHandler().update(purchasePolicyDAL);
                }
            }
            else
            {
                category_terms[category_name] = new OrTerm(category_terms[category_name], term);
                if (this.purchasePolicyDAL != null)
                {
                    category_terms[category_name].DALTerm = new DALTerm(json_term);
                    DataHandler.getDBHandler().save(category_terms[category_name].DALTerm);
                    DALCategoryTerm dal_term = this.purchasePolicyDAL.category_terms.Find(d => d.CategoryName == category_name);
                    dal_term.Term = category_terms[category_name].ToDAL();
                    DataHandler.getDBHandler().update(dal_term);
                }
            }
        }

        public void AddStoreTerm(string json_term)
        { 
            AddStoreTerm(json_term, null);
        }

        private void AddStoreTerm(string json_term, DALTerm dalterm)
        {
            Term term = ParseTerm(json_term);
            term.DALTerm = dalterm;
            if (store_terms == null)
                store_terms = term;
            else
                store_terms = new AndTerm(store_terms, term);
            if (this.purchasePolicyDAL != null)
            {
                store_terms.DALTerm = new DALTerm(json_term);
                DataHandler.getDBHandler().save(store_terms.DALTerm);
                this.purchasePolicyDAL.store_terms = store_terms.ToDAL();
                DataHandler.getDBHandler().update(purchasePolicyDAL);
            }
        }

        public void AddUserTerm(string json_term)
        {
            AddUserTerm(json_term, null);
        }

        public void AddUserTerm(string json_term, DALTerm dalterm)
        {
            Term term = ParseTerm(json_term);
            term.DALTerm = dalterm;
            if (user_terms == null)
                user_terms = term;
            else
                user_terms = new OrTerm(user_terms, term);
            if (this.purchasePolicyDAL != null)
            {
                user_terms.DALTerm = new DALTerm(json_term);
                DataHandler.getDBHandler().save(user_terms.DALTerm);
                this.purchasePolicyDAL.user_terms = user_terms.ToDAL();
                DataHandler.getDBHandler().update(purchasePolicyDAL);
            }
        }

        private Term ParseTerm(string json_discount)
        {
            dynamic discount_data = JObject.Parse(json_discount);
            return ParseTerm(discount_data);
        }

        private Term ParseTerm(dynamic data)
        {
            string tag = data.tag;
            if (tag.Equals(PURCHASE_COMP_TERM_TAG))
                return ParseCompositeTerm(data);
            if (tag.Equals(PRODUCT_SIMPLE_TERM_TAG))
                return ParseProductTerm(data);
            if (tag.Equals(CATEGORY_SIMPLE_TERM_TAG))
                return ParseCategoryTerm(data);
            if (tag.Equals(BAG_SIMPLE_TERM_TAG))
                return ParseBagTerm(data);
            if (tag.Equals(USER_SIMPLE_TERM_TAG))
                return ParseUserTerm(data);
            throw new Exception("Unknown term tag: " + tag);
        }
        private Term ParseCompositeTerm(dynamic data)
        {
            string value = data.value;
            if (value.Equals("and"))
                return new AndTerm(ParseTerm(data.lhs), ParseTerm(data.rhs));
            if (value.Equals("or"))
                return new OrTerm(ParseTerm(data.lhs), ParseTerm(data.rhs));
            if (value.Equals("if"))
                return new ConditionedTerm(ParseTerm(data.lhs), ParseTerm(data.rhs));
            throw new Exception("Unknown composite purchase term type: " + value);
        }

        private Term ParseProductTerm(dynamic data)
        {
            string type = data.type;
            string action = data.action;
            string value = data.value;
            string productId = data.productId;
            if (type.Equals("p"))
            {
                return ParseProductPriceTerm(action, value, productId);
            }
            if (type.Equals("q"))
            {
                return ParseProductQuantityTerm(action, value, productId);
            }
            if (type.Equals("h"))
            {
                return ParseProductHourTerm(action, value, productId);
            }
            if (type.Equals("d"))
            {
                return ParseProductDateTerm(action, value, productId);
            }
            throw new Exception("Unknown Product term type: " + type);
        }

        private Term ParseProductPriceTerm(string action, string value, string productId)
        {
            double price;
            int ID;
            try
            {
                price = double.Parse(value);
                if (price < 0)
                    throw new Exception("Term OfferedPrice value cannot be negtive number.");
                try
                {

                    ID = int.Parse(productId);
                    
                }catch (Exception)
                {
                    throw new Exception("Invalid Product ID term value: " + productId);
                }
            }catch (Exception)
            {
                throw new Exception("Invalid OfferedPrice term value: " + value);
            }
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                double bag_product_price = 0;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Id == ID)
                        bag_product_price += product.Price * product.Quantity;
                }
                if (action.Equals("<"))
                    return bag_product_price < price;
                if (action.Equals("<="))
                    return bag_product_price <= price;
                if (action.Equals(">"))
                    return bag_product_price > price;
                if (action.Equals(">="))
                    return bag_product_price >= price;
                if (action.Equals("="))
                    return bag_product_price == price;
                return bag_product_price != price;
            };
            return new SimpleTerm(term);
        }

        private Term ParseProductQuantityTerm(string action, string value, string productId)
        {
            int quantity;
            int ID;
            try
            {
                quantity = int.Parse(value);
                
                try
                {

                    ID = int.Parse(productId);
                    
                }
                catch (Exception)
                {
                    throw new Exception("Invalid Product ID term value: " + productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid OfferedPrice term value: " + value);
            }
            if (quantity < 0)
                throw new Exception("Term quantity value cannot be negtive number.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                int bag_product_quantity = 0;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Id == ID)
                        bag_product_quantity += product.Quantity;
                }
                if (action.Equals("<"))
                    return bag_product_quantity < quantity;
                if (action.Equals("<="))
                    return bag_product_quantity <= quantity;
                if (action.Equals(">"))
                    return bag_product_quantity > quantity;
                if (action.Equals(">="))
                    return bag_product_quantity >= quantity;
                if (action.Equals("="))
                    return bag_product_quantity == quantity;
                return bag_product_quantity != quantity;
            };
            return new SimpleTerm(term);
        }

        private Term ParseProductHourTerm(string action, string value, string productId)
        {
            TimeSpan hour;
            int ID;
            try
            {
                hour = DateTime.Parse(value).TimeOfDay;
                try
                {

                    ID = int.Parse(productId);
                    
                }
                catch (Exception)
                {
                    throw new Exception("Invalid Product ID term value: " + productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid hour term value: " + value);
            }
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                bool found = false;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Id == ID)
                    {
                        found = true;
                        break;
                    }
                }
                if (found && action.Equals("<"))
                    return now < hour;
                if (found && action.Equals("<="))
                    return now <= hour;
                if (found && action.Equals(">"))
                    return now > hour;
                if (found && action.Equals(">="))
                    return now >= hour;
                if (found && action.Equals("="))
                    return now == hour;
                if (found && action.Equals("!="))
                    return now != hour;
                return true;
            };
            return new SimpleTerm(term);
        }

        private Term ParseProductDateTerm(string action, string value, string productId)
        {
            DateTime date;
            int ID;
            try
            {
                date = DateTime.Parse(value).Date;
                try
                {
                    ID = int.Parse(productId);
                    
                }
                catch (Exception)
                {
                    throw new Exception("Invalid Product ID term value: " + productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid date term value: " + value);
            }
            if (date < DateTime.Now)
                throw new Exception("Term date must be in the future.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                DateTime now = DateTime.Now.Date;
                bool found = false;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Id == ID)
                    {
                        found = true;
                        break;
                    }
                }
                if (found && action.Equals("<"))
                    return now < date;
                if (found && action.Equals("<="))
                    return now <= date;
                if (found && action.Equals(">"))
                    return now > date;
                if (found && action.Equals(">="))
                    return now >= date;
                if (found && action.Equals("="))
                    return now == date;
                if (found && action.Equals("!="))
                    return now != date;
                return true;
            };
            return new SimpleTerm(term);
        }

        private Term ParseCategoryTerm(dynamic data)
        {
            string type = data.type;
            string action = data.action;
            string value = data.value;
            string category = data.category;
            if (type.Equals("p"))
            {
                return ParseCategoryPriceTerm(action, value, category);
            }
            if (type.Equals("q"))
            {
                return ParseCategoryQuantityTerm(action, value, category);
            }
            if (type.Equals("h"))
            {
                return ParseCategoryHourTerm(action, value, category);
            }
            if (type.Equals("d"))
            {
                return ParseCategoryDateTerm(action, value, category);
            }
            throw new Exception("Unknown Product term type: " + type);
        }

        private Term ParseCategoryPriceTerm(string action, string value, string category)
        {
            double price;
            try
            {
                price = double.Parse(value);
            }
            catch (Exception)
            {
                throw new Exception("Invalid OfferedPrice term value: " + value);
            }
            if (price < 0)
                throw new Exception("Term OfferedPrice value cannot be negtive number.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                double bag_product_price = 0;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Category != null && category.Equals(product.Category))
                        bag_product_price += product.Price * product.Quantity;
                }
                if (action.Equals("<"))
                    return bag_product_price < price;
                if (action.Equals("<="))
                    return bag_product_price <= price;
                if (action.Equals(">"))
                    return bag_product_price > price;
                if (action.Equals(">="))
                    return bag_product_price >= price;
                if (action.Equals("="))
                    return bag_product_price == price;
                return bag_product_price != price;
            };
            return new SimpleTerm(term);
        }

        private Term ParseCategoryQuantityTerm(string action, string value, string category)
        {
            int quantity;
            try
            {
                quantity = int.Parse(value);
            }
            catch (Exception)
            {
                throw new Exception("Invalid OfferedPrice term value: " + value);
            }
            if (quantity < 0)
                throw new Exception("Term quantity value cannot be negtive number.");

            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                int bag_product_quantity = 0;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Category != null && category.Equals(product.Category))
                        bag_product_quantity += product.Quantity;
                }
                if (action.Equals("<"))
                    return bag_product_quantity < quantity;
                if (action.Equals("<="))
                    return bag_product_quantity <= quantity;
                if (action.Equals(">"))
                    return bag_product_quantity > quantity;
                if (action.Equals(">="))
                    return bag_product_quantity >= quantity;
                if (action.Equals("="))
                    return bag_product_quantity == quantity;
                return bag_product_quantity != quantity;
            };
            return new SimpleTerm(term);
        }

        private Term ParseCategoryHourTerm(string action, string value, string category)
        {
            TimeSpan hour;
            try
            {
                hour = DateTime.Parse(value).TimeOfDay;
            }
            catch (Exception)
            {
                throw new Exception("Invalid hour term value: " + value);
            }
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                bool found = false;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Category != null && category.Equals(product.Category))
                    {
                        found = true;
                        break;
                    }
                }
                if (found && action.Equals("<"))
                    return now < hour;
                if (found && action.Equals("<="))
                    return now <= hour;
                if (found && action.Equals(">"))
                    return now > hour;
                if (found && action.Equals(">="))
                    return now >= hour;
                if (found && action.Equals("="))
                    return now == hour;
                if (found && action.Equals("!="))
                    return now != hour;
                return true;
            };
            return new SimpleTerm(term);
        }

        private Term ParseCategoryDateTerm(string action, string value, string category)
        {
            DateTime date;
            try
            {
                date = DateTime.Parse(value).Date;
            }
            catch (Exception)
            {
                throw new Exception("Invalid date term value: " + value);
            }
            if (date < DateTime.Now)
                throw new Exception("Term date must be in the future.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                DateTime now = DateTime.Now.Date;
                bool found = false;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    if (product.Category != null && category.Equals(product.Category))
                    {
                        found = true;
                        break;
                    }
                }
                if (found && action.Equals("<"))
                    return now < date;
                if (found && action.Equals("<="))
                    return now <= date;
                if (found && action.Equals(">"))
                    return now > date;
                if (found && action.Equals(">="))
                    return now >= date;
                if (found && action.Equals("="))
                    return now == date;
                if (found && action.Equals("!="))
                    return now != date;
                return true;
            };
            return new SimpleTerm(term);
        }

        private Term ParseBagTerm(dynamic data)
        {
            string type = data.type;
            string action = data.action;
            string value = data.value;
            if (type.Equals("p"))
            {
                return ParseBagPriceTerm(action, value);
            }
            if (type.Equals("q"))
            {
                return ParseBagQuantityTerm(action, value);
            }
            if (type.Equals("h"))
            {
                return ParseBagHourTerm(action, value);
            }
            if (type.Equals("d"))
            {
                return ParseBagDateTerm(action, value);
            }
            throw new Exception("Unknown Product term type: " + type);
        }

        private Term ParseBagPriceTerm(string action, string value)
        {
            double price;
            try
            {
                price = double.Parse(value);
            }
            catch (Exception)
            {
                throw new Exception("Invalid OfferedPrice term value: " + value);
            }
            if (price < 0)
                throw new Exception("Term OfferedPrice value cannot be negtive number.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                double bag_price = 0;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    bag_price += product.Price * product.Quantity;
                }
                if (action.Equals("<"))
                    return bag_price < price;
                if (action.Equals("<="))
                    return bag_price <= price;
                if (action.Equals(">"))
                    return bag_price > price;
                if (action.Equals(">="))
                    return bag_price >= price;
                if (action.Equals("="))
                    return bag_price == price;
                return bag_price != price;
            };
            return new SimpleTerm(term);
        }

        private Term ParseBagQuantityTerm(string action, string value)
        {
            int quantity;
            try
            {
                quantity = int.Parse(value);
            }
            catch (Exception)
            {
                throw new Exception("Invalid OfferedPrice term value: " + value);
            }
            if (quantity < 0)
                throw new Exception("Term quantity value cannot be negtive number.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                int bag_quantity = 0;
                foreach (ProductDTO product in shoppingBag.products)
                {
                    bag_quantity += product.Quantity;
                }
                if (action.Equals("<"))
                    return bag_quantity < quantity;
                if (action.Equals("<="))
                    return bag_quantity <= quantity;
                if (action.Equals(">"))
                    return bag_quantity > quantity;
                if (action.Equals(">="))
                    return bag_quantity >= quantity;
                if (action.Equals("="))
                    return bag_quantity == quantity;
                return bag_quantity != quantity;
            };
            return new SimpleTerm(term);
        }

        private Term ParseBagHourTerm(string action, string value)
        {
            TimeSpan hour;
            try
            {
                hour = DateTime.Parse(value).TimeOfDay;
            }
            catch (Exception)
            {
                throw new Exception("Invalid hour term value: " + value);
            }
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                if (action.Equals("<"))
                    return now < hour;
                if (action.Equals("<="))
                    return now <= hour;
                if (action.Equals(">"))
                    return now > hour;
                if (action.Equals(">="))
                    return now >= hour;
                if (action.Equals("="))
                    return now == hour;
                return now != hour;
            };
            return new SimpleTerm(term);
        }

        private Term ParseBagDateTerm(string action, string value)
        {
            DateTime date;
            try
            {
                date = DateTime.Parse(value).Date;
            }
            catch (Exception)
            {
                throw new Exception("Invalid date term value: " + value);
            }
            if (date < DateTime.Now)
                throw new Exception("Term date must be in the future.");
            if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                DateTime now = DateTime.Now.Date;
                if (action.Equals("<"))
                    return now < date;
                if (action.Equals("<="))
                    return now <= date;
                if (action.Equals(">"))
                    return now > date;
                if (action.Equals(">="))
                    return now >= date;
                if (action.Equals("="))
                    return now == date;
                return now != date;
            };
            return new SimpleTerm(term);
        }

        private Term ParseUserTerm(dynamic data)
        {
            int age_value;
            try
            {
                age_value = int.Parse(((string)data.age).Trim('{', '}'));
            }
            catch (Exception)
            {
                throw new Exception("Invalid age term value: " + data.age);
            }
            if (age_value < 0)
                throw new Exception("Age value must be bigger that 0.");
            string action = data.action;

            if (!action.Equals(">") && !action.Equals(">=") && !action.Equals("!="))
                throw new Exception("Unknown term operand: " + action);
            SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
            {
                if (action.Equals(">"))
                    return age > age_value;
                if (action.Equals(">="))
                    return age >= age_value;
                return age != age_value;
            };
            return new SimpleTerm(term);
        }
    }
}
