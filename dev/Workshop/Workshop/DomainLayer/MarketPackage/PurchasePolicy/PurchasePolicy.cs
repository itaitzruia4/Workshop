﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.Terms;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage
{
    class PurchasePolicy
    {
        private Dictionary<int, Term> products_terms;
        private Dictionary<string, Term> category_terms;
        private Term user_terms;
        private Term store_terms;

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
                if(category_terms.ContainsKey(product.Category))
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

        public void AddProductTerm(string json_discount, int product_id)
        {
            Term term = ParseTerm(json_discount);
            if (!products_terms.ContainsKey(product_id))
                products_terms.Add(product_id, term);
            else
                products_terms[product_id] = new OrTerm(products_terms[product_id], term);
        }

        public void AddCategoryTerm(string json_discount, string category_name)
        {
            Term term = ParseTerm(json_discount);
            if (!category_terms.ContainsKey(category_name))
                category_terms.Add(category_name, term);
            else
                category_terms[category_name] = new OrTerm(category_terms[category_name], term);
        }

        public void AddStoreTerm(string json_discount)
        {
            Term term = ParseTerm(json_discount);
            if (store_terms == null)
                store_terms = term;
            else
                store_terms = new OrTerm(store_terms, term);
        }

        public void AddUserTerm(string json_discount)
        {
            Term term = ParseTerm(json_discount);
            if (user_terms == null)
                user_terms = term;
            else
                user_terms = new OrTerm(user_terms, term);
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
            string value = data.tag;
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
            throw new Exception("Unknown product term type: " + type);
        }

        private Term ParseProductPriceTerm(string action, string value, string productId)
        {
            try
            {
                double price = double.Parse(value);
                try
                {

                    int ID = int.Parse(productId);
                    if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                        throw new Exception("Unknown term operand: " + action);
                        SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                            {
                                double bag_product_price = 0;
                                foreach (ProductDTO product in shoppingBag.products)
                                {
                                    if(product.Id == ID)
                                        bag_product_price += product.Price;
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
                }catch (Exception)
                {
                    throw new Exception("Invalid product ID term value: " + productId);
                }
            }catch (Exception)
            {
                throw new Exception("Invalid price term value: " + value);
            }
        }

        private Term ParseProductQuantityTerm(string action, string value, string productId)
        {
            try
            {
                int quantity = int.Parse(value);
                try
                {

                    int ID = int.Parse(productId);
                    if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                        throw new Exception("Unknown term operand: " + action);
                    SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                    {
                        int bag_product_quantity = 0;
                        foreach (ProductDTO product in shoppingBag.products)
                        {
                            if(product.Id == ID)
                                bag_product_quantity++;
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
                catch (Exception)
                {
                    throw new Exception("Invalid product ID term value: " + productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid price term value: " + value);
            }
        }

        private Term ParseProductHourTerm(string action, string value, string productId)
        {
            try
            {
                TimeSpan hour = DateTime.Parse(value).TimeOfDay;
                try
                {

                    int ID = int.Parse(productId);
                    if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                        throw new Exception("Unknown term operand: " + action);
                    SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                    {
                        TimeSpan now = DateTime.Now.TimeOfDay;
                        bool found = false;
                        foreach (ProductDTO product in shoppingBag.products)
                        {
                            if(product.Id == ID)
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
                catch (Exception)
                {
                    throw new Exception("Invalid product ID term value: " + productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid hour term value: " + value);
            }
        }

        private Term ParseProductDateTerm(string action, string value, string productId)
        {
            try
            {
                DateTime date = DateTime.Parse(value).Date;
                try
                {
                    int ID = int.Parse(productId);
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
                catch (Exception)
                {
                    throw new Exception("Invalid product ID term value: " + productId);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid date term value: " + value);
            }
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
            throw new Exception("Unknown product term type: " + type);
        }

        private Term ParseCategoryPriceTerm(string action, string value, string category)
        {
            try
            {
                double price = double.Parse(value);

                if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                    throw new Exception("Unknown term operand: " + action);
                SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                {
                    double bag_product_price = 0;
                    foreach (ProductDTO product in shoppingBag.products)
                    {
                        if (product.Category != null && category.Equals(product.Category))
                            bag_product_price += product.Price;
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
            catch (Exception)
            {
                throw new Exception("Invalid price term value: " + value);
            }
        }

        private Term ParseCategoryQuantityTerm(string action, string value, string category)
        {
            try
            {
                int quantity = int.Parse(value);

                if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                    throw new Exception("Unknown term operand: " + action);
                SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                {
                    int bag_product_quantity = 0;
                    foreach (ProductDTO product in shoppingBag.products)
                    {
                        if (product.Category != null && category.Equals(product.Category))
                            bag_product_quantity++;
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
            catch (Exception)
            {
                throw new Exception("Invalid price term value: " + value);
            }
        }

        private Term ParseCategoryHourTerm(string action, string value, string category)
        {
            try
            {
                TimeSpan hour = DateTime.Parse(value).TimeOfDay;

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
            catch (Exception)
            {
                throw new Exception("Invalid hour term value: " + value);
            }
        }

        private Term ParseCategoryDateTerm(string action, string value, string category)
        {
            try
            {
                DateTime date = DateTime.Parse(value).Date;
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
            catch (Exception)
            {
                throw new Exception("Invalid date term value: " + value);
            }
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
            throw new Exception("Unknown product term type: " + type);
        }

        private Term ParseBagPriceTerm(string action, string value)
        {
            try
            {
                double price = double.Parse(value);

                if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                    throw new Exception("Unknown term operand: " + action);
                SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                {
                    double bag_price = 0;
                    foreach (ProductDTO product in shoppingBag.products)
                    {
                        bag_price += product.Price;
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
            catch (Exception)
            {
                throw new Exception("Invalid price term value: " + value);
            }
        }

        private Term ParseBagQuantityTerm(string action, string value)
        {
            try
            {
                int quantity = int.Parse(value);

                if (!action.Equals("<") && !action.Equals("<=") && !action.Equals(">") && !action.Equals(">=") && !action.Equals("=") && !action.Equals("!="))
                    throw new Exception("Unknown term operand: " + action);
                SimpleTerm.TermSimple term = (ShoppingBagDTO shoppingBag, int age) =>
                {
                    int bag_quantity = shoppingBag.products.Count();
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
            catch (Exception)
            {
                throw new Exception("Invalid price term value: " + value);
            }
        }

        private Term ParseBagHourTerm(string action, string value)
        {
            try
            {
                TimeSpan hour = DateTime.Parse(value).TimeOfDay;

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
            catch (Exception)
            {
                throw new Exception("Invalid hour term value: " + value);
            }
        }

        private Term ParseBagDateTerm(string action, string value)
        {
            try
            {
                DateTime date = DateTime.Parse(value).Date;
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
            catch (Exception)
            {
                throw new Exception("Invalid date term value: " + value);
            }
        }

        private Term ParseUserTerm(dynamic data)
        {
            try
            {
                int age_value = int.Parse(data.age);
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
            catch (Exception)
            {
                throw new Exception("Invalid age term value: " + data.age);
            }
        }
    }
}