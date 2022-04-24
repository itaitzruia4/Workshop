using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Store
    {
        private bool open;
        private int id;
        private string name;
        private Dictionary<int, Product> products;

        public Store(int id, string name)
        {
            this.id = id;
            if (name == null || name.Equals(""))
                throw new ArgumentException("Store name cannot be empty.");
            this.name = name;
            products = new Dictionary<int, Product>();
            this.open = true; //TODO: check if on init store supposed to be open or closed.
        }
        public int getID()
        {
            return this.id;
        }

        public bool isOpen()
        {
            return this.open;
        }

        public void openStore()
        {
            throw new NotImplementedException();
        }

        public void closeStore()
        {
            this.open = false;
        }

        public Product AddProduct(int productID, string name, string description, double price, int quantity)
        {
            ValidateID(productID);
            if (products.ContainsKey(productID))
                throw new ArgumentException("Product with the same ID already exists.");
            ValidateName(name);
            ValidatePrice(price);
            ValidateQuantity(quantity);

            Product newProd = new Product(productID, name, description, price, quantity);
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

        public Product GetProduct(int productId)
        {
            if (!products.ContainsKey(productId))
                throw new ArgumentException($"Product with ID {productId} does not exist in the store.");
            return products[productId];
        }

        internal StoreDTO ToDTO()
        {
            throw new NotImplementedException();
        }
    }
}
