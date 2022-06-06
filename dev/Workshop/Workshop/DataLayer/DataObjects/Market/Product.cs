using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class Product : DALObject
    {
        public int Id { get; set; }
        public int Store { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        public Product()
        {
        }

        public Product(int id, int store, string name, string description, double price, int quantity, string category)
        {
            Id = id;
            Store = store;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
        }
    }
}
