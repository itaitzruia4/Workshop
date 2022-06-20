using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage;
using System.Threading;
using StoreDAL = Workshop.DataLayer.DataObjects.Market.Store;
using DALObject = Workshop.DataLayer.DALObject;
using ProductDAL = Workshop.DataLayer.DataObjects.Market.Product;
using DiscountPolicyDAL = Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy;
using PurchasePolicyDAL = Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy;
using DataHandler = Workshop.DataLayer.DataHandler;

using Member = Workshop.DomainLayer.UserPackage.Permissions.Member;
namespace Workshop.DomainLayer.MarketPackage
{
    public class Store : IPersistentObject<StoreDAL>
    {
        private bool open { get; set; }
        private int id { get; set; }
        private string name { get; set; }
        private Dictionary<int, Product> products { get; set; }
        private DiscountPolicy discountPolicy { get; set; }
        private PurchasePolicy purchasePolicy { get; set; }

        private ReaderWriterLock rwl;

        private StoreDAL storeDAL;


        //public Store(int id, string name)
        public HashSet<Member> owners { get; }
        public ConcurrentDictionary<Member, HashSet<Member>> owner_voting { get; }
        public Store(int id, string name, Member founder)
        {
            if (name == null || name.Equals(""))
                throw new ArgumentException("Store name cannot be empty.");
            this.id = id;
            this.name = name;
            products = new Dictionary<int, Product>();
            this.open = true; //TODO: check if on init store supposed to be open or closed.
            this.rwl = new ReaderWriterLock();
            this.discountPolicy = new DiscountPolicy(this);
            this.purchasePolicy = new PurchasePolicy(this);

            List<ProductDAL> productsDAL = new List<ProductDAL>();
            DiscountPolicyDAL dpDAL = (DiscountPolicyDAL)discountPolicy.ToDAL();
            PurchasePolicyDAL ppDAL = (PurchasePolicyDAL)purchasePolicy.ToDAL();
            storeDAL = new StoreDAL(id, open, name, dpDAL, ppDAL, productsDAL);
            DataHandler.getDBHandler().save(storeDAL);
            open = true; //TODO: check if on init store supposed to be open or closed.
            discountPolicy = new DiscountPolicy(this);
            purchasePolicy = new PurchasePolicy(this);
            owners = new HashSet<Member>();
            owners.Add(founder);
            owner_voting = new ConcurrentDictionary<Member, HashSet<Member>>();
        }

        public bool AddOwner(Member owner)
        {
            return owners.Add(owner);
        }

        public Store(StoreDAL storeDAL)
        {
            this.id = storeDAL.Id;
            this.name = name;
            this.products = new Dictionary<int, Product>();
            this.open = true; //TODO: check if on init store supposed to be open or closed.
            this.rwl = new ReaderWriterLock();
            this.discountPolicy = new DiscountPolicy(storeDAL.DiscountPolicy, this);
            this.purchasePolicy = new PurchasePolicy(storeDAL.PurchasePolicy);

            foreach(ProductDAL product in storeDAL.Products)
            {
                products.Add(product.Id, new Product(product));
            }
        }

        public StoreDAL ToDAL()
        {
            return storeDAL;
        }

        public ReaderWriterLock getLock()
        {
            return this.rwl;
        }
        public bool VoteForStoreOwnerNominee(Member voter, Member nominee)
        {
            if (!owners.Contains(voter))
            {
                throw new ArgumentException($"{voter.Username} is not a store owner of store {id}, hence he can not vote to nominate {nominee.Username} as a store owner.");
            }
            if (owners.Contains(nominee))
            {
                throw new ArgumentException($"{nominee.Username} is already a store owner of store {id}, and you can not vote to nominate him again.");
            }
            HashSet<Member> voters;
            if (owner_voting.TryGetValue(nominee, out voters))
            {
                if (voters.Add(voter))
                {
                    return voters.Count == owners.Count;
                }
                return false;
            }
            else
            {
                HashSet<Member> temp = new HashSet<Member>();
                temp.Add(voter);
                owner_voting.TryAdd(nominee, temp);
                return 1 == owners.Count;
            }
        }

        public int GetId()
        {
            return this.id;
        }

        public bool IsOpen()
        {
            return this.open;
        }

        public IReadOnlyDictionary<int, Product> GetProducts()
        {
            return products;
        }

        public String GetStoreName()
        {
            return name;
        }

        public void OpenStore()
        {
            this.open = true;
            storeDAL.Open = true;
            DataHandler.getDBHandler().update(storeDAL);
        }

        public void CloseStore()
        {
            this.open = false;
            storeDAL.Open = false;
            DataHandler.getDBHandler().update(storeDAL);
        }

        public Product AddProduct(string name, int productId, string description, double price, int quantity, string category)
        {
            ValidateName(name);
            ValidatePrice(price);
            ValidateQuantity(quantity);
            ValidateCategory(category);
            Product newProd = new Product(productId, name, description, price, quantity, category, id);
            products.Add(productId, newProd);
            storeDAL.Products.Add(newProd.ToDAL());
            DataHandler.getDBHandler().update(storeDAL);
            return newProd;
        }

        public void RemoveProduct(int productID)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            products.Remove(productID);
            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            storeDAL.Products.Remove(pDAL);
            DataHandler.getDBHandler().update(storeDAL);
            DataHandler.getDBHandler().remove(pDAL);
        }

        public void ChangeProductName(int productID, string name)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateName(name);
            products[productID].Name = name;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Name = name;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void ChangeProductDescription(int productID, string description)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            products[productID].Description = description;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Description = description;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void ChangeProductPrice(int productID, double price)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidatePrice(price);
            products[productID].Price = price;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Price = price;
            DataHandler.getDBHandler().update(pDAL);
        }
        public void ChangeProductQuantity(int productID, int quantity)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateQuantity(quantity);
            products[productID].Quantity = quantity;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Quantity = quantity;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void ChangeProductCategory(int productID, string category)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateCategory(category);
            products[productID].Category = category;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Category = category;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void AddProductDiscount(string json_discount, int product_id)
        {
            if (!products.ContainsKey(product_id))
                throw new Exception("Product ID: " + product_id + " does not exist in store.");
            discountPolicy.AddProductDiscount(json_discount, product_id);
        }

        public void AddCategoryDiscount(string json_discount, string category_name)
        {
            ValidateCategory(category_name);
            discountPolicy.AddCategoryDiscount(json_discount, category_name);
        }

        public void AddStoreDiscount(string json_discount)
        {
            discountPolicy.AddStoreDiscount(json_discount);
        }

        public void AddProductTerm(string json_term, int product_id)
        {
            if (!products.ContainsKey(product_id))
                throw new Exception("Product with ID: " + product_id + " does not exist in store.");
            purchasePolicy.AddProductTerm(json_term, product_id);
        }

        public void AddCategoryTerm(string json_term, string category_name)
        {
            purchasePolicy.AddCategoryTerm(json_term, category_name);
        }

        public void AddStoreTerm(string json_term)
        {
            purchasePolicy.AddStoreTerm(json_term);
        }

        public void AddUserTerm(string json_term)
        {
            purchasePolicy.AddUserTerm(json_term);
        }

        private void ValidateID(int ID)
        {
            if (ID < 0)
                throw new ArgumentOutOfRangeException("Product ID must be bigger than zero.");
        }

        private void ValidateProductExist(int ID)
        {
            if (!products.ContainsKey(ID))
                throw new Exception("Product ID does not exist in the store.");
        }

        private void ValidateName(string name)
        {
            if (name == null || name.Equals(""))
                throw new ArgumentException("Name cannot be empty.");
        }

        private void ValidatePrice(double price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException("Price must be bigger than zero.");
        }

        private void ValidateQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException("Quntity must be zero or above.");
        }

        private void ValidateCategory(string category)
        {
            if (category == null || category.Equals(""))
                throw new ArgumentException("Category must be non-empty.");
        }

        public Product GetProduct(int productId)
        {
            if (!products.ContainsKey(productId))
                throw new ArgumentException($"Product with ID {productId} does not exist in the store.");
            return products[productId];
        }

        public Product GetProductForSale(int productId, int quantity)
        {
            if (!products.ContainsKey(productId))
                throw new ArgumentException($"Product with ID {productId} does not exist in the store.");
            if (products[productId].Quantity<quantity)
                throw new ArgumentException($"Product with ID {productId} does not has enough quantity.");
            products[productId].Quantity -= quantity;
            return products[productId];
        }
        internal ShoppingBagDTO validateBagInStockAndGet(ShoppingBagDTO shoppingBag)
        {
            
            foreach (ProductDTO product in shoppingBag.products)
            {
                if(products.ContainsKey(product.Id) && products[product.Id].Quantity >= product.Quantity)
                {
                    //products[product.Id].Quantity -= product.Quantity;
                    //todo does it impact thred
                }
                else {
                    throw new ArgumentException($"store {id} doesn't has enough {product.Name} in stock");
                }
                        
            }  
            return shoppingBag;     
        }
        

        internal StoreDTO GetStoreDTO()
        {
            //throw new NotImplementedException();
            return new StoreDTO(id, name, products,open);
        }

        public virtual bool ProductExists(int product_id)
        {
            return products.ContainsKey(product_id);
        }

        internal double CalaculatePrice(ShoppingBagDTO shoppingBag)
        {
            double discount = discountPolicy.CalculateDiscount(shoppingBag);
            double price = 0;
            foreach (ProductDTO productDTO in shoppingBag.products)
            {
                price += productDTO.Price*productDTO.Quantity;
            }
            return Math.Max(price - discount, 0);
        }

        internal void CheckPurchasePolicy(ShoppingBagDTO shoppingBag, int age)
        {
            double price = CalaculatePrice(shoppingBag);
            if (!purchasePolicy.CanPurchase(shoppingBag, age))
                throw new Exception("Cannot purchase shopping bag because it violates our purchase policy.");
        }
    }
}
