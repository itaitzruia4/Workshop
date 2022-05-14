using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage;
using System.Threading;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Store
    {
        private bool open { get; set; }
        private int id { get; set; }
        private string name { get; set; }
        private Dictionary<int, Product> products { get; set; }
        private DiscountPolicy discountPolicy { get; set; }
        private PurchasePolicy purchasePolicy { get; set; }

        private ReaderWriterLock rwl;

        public Store(int id, string name)
        {
            this.id = id;
            if (name == null || name.Equals(""))
                throw new ArgumentException("Store name cannot be empty.");
            this.name = name;
            products = new Dictionary<int, Product>();
            this.open = true; //TODO: check if on init store supposed to be open or closed.
            this.rwl = new ReaderWriterLock();
            this.discountPolicy = new DiscountPolicy(this);
            this.purchasePolicy = new PurchasePolicy(this);
        }

        public ReaderWriterLock getLock()
        {
            return this.rwl;
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

        public void openStore()
        {
            throw new NotImplementedException();
        }

        public void closeStore()
        {
            this.open = false;
        }

        public Product AddProduct(int productID, string name, string description, double price, int quantity, string category)
        {
            ValidateID(productID);
            if (products.ContainsKey(productID))
                throw new ArgumentException("Product with the same ID already exists.");
            ValidateName(name);
            ValidatePrice(price);
            ValidateQuantity(quantity);
            ValidateCategory(category);
            Product newProd = new Product(productID, name, description, price, quantity, category);
            products.Add(productID, newProd);
            return newProd;
        }

        public void RemoveProduct(int productID)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            products.Remove(productID);
        }

        public void ChangeProductName(int productID, string name)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateName(name);
            products[productID].Name = name;
        }

        public void ChangeProductDescription(int productID, string description)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            products[productID].Description = description;
        }

        public void ChangeProductPrice(int productID, double price)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidatePrice(price);
            products[productID].Price = price;
        }
        public void ChangeProductQuantity(int productID, int quantity)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateQuantity(quantity);
            products[productID].Quantity = quantity;
        }

        public void ChangeProductCategory(int productID, string category)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateCategory(category);
            products[productID].Category = category;
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
        internal ShoppingBagDTO validateBagInStockAndGet(ShoppingBagDTO shoppingBag)
        {
            
            foreach (ProductDTO product in shoppingBag.products)
            {
                if(products.ContainsKey(product.Id) && products[product.Id].Quantity >= product.Quantity)
                {
                    products[product.Id].Quantity -= product.Quantity;
                }
                else {
                    throw new ArgumentException($"store {id} doesn't has enough {product.Name} in stock");
                }
                        
            }  
            return shoppingBag;     
        }
        internal void restoreProduct(ProductDTO product)
        {
            if(products.ContainsKey(product.Id))
            {
                products[product.Id].Quantity += product.Quantity;
            }
            else
            {
                products.Add(product.Id,new Product(product.Id,product.Name,product.Description,product.Price,product.Quantity, product.Category));
            }
        }

        internal StoreDTO GetStoreDTO()
        {
            //throw new NotImplementedException();
            return new StoreDTO(id, name, products,open);
        }

        internal bool ProductExists(int product_id)
        {
            return products.ContainsKey(product_id);
        }

        internal double CalaculatePrice(ShoppingBagDTO shoppingBag)
        {
            double discount = discountPolicy.CalculateDiscount(shoppingBag);
            double price = 0;
            foreach (ProductDTO productDTO in shoppingBag.products)
            {
                price += productDTO.Price;
            }
            return price - discount;
        }

        internal void CheckPurchasePolicy(ShoppingBagDTO shoppingBag, int age)
        {
            double price = CalaculatePrice(shoppingBag);
            if (!purchasePolicy.CanPurchase(shoppingBag, age))
                throw new Exception("Cannot purchase shopping bag because it violates our purchase policy.");
        }
    }
}
