using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    class Store
    {
        private int id;
        private Dictionary<int, Product> products;

        public Store(int id)
        {
            this.id = id;
            products = new Dictionary<int, Product>();
        }

        public int getID()
        {
            return this.id;
        }

        public void AddProduct(int productID, string name, int price, int quantity)
        {
            ValidateID(productID);
            if (products.ContainsKey(productID))
                throw new ArgumentException("Product with the same ID already exists.");
            ValidateName(name);
            ValidatePrice(price);
            ValidateQuantity(quantity);
            products.Add(productID, new Product(productID, name, price, quantity));
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
            products[productID].SetName(name);
        }

        public void ChangeProductPrice(int productID, int price)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidatePrice(price);
            products[productID].SetPrice(price);
        }
        public void ChangeProductQuantity(int productID, int quantity)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateQuantity(quantity);
            products[productID].SetQuantity(quantity);
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

        private void ValidatePrice(int price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException("Price must be bigger than zero.");
        }

        private void ValidateQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException("Quntity must be zero or above.");
        }
    }
}
