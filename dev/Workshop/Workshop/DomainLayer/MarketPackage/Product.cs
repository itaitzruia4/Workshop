using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    class Product
    {
        private int id;
        private string name;
        private int price;
        private int quantity;

        public Product(int id, string name, int price, int quantity)
        {
            this.id = id;
            this.name = name;
            this.price = price;
            this.quantity = quantity;
        }

        public int GetId()
        {
            return id;  
        }

        public string GetName()
        {
            return name;
        }

        public int GetPrice()
        {
            return price;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetPrice(int price)
        {
            this.price = price;
        }

        public void SetQuantity(int quantity)
        {
            this.quantity = quantity;
        }
    }
}
