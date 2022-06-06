using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ShoppingBagProduct : DALObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public ShoppingBagProduct()
        {
        }

        public ShoppingBagProduct(string name, double price, int quantity, string description, string category)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            Description = description;
            Category = category;
        }
    }
}
